﻿# Salvation Core

Core contains all of the modelling logic. It is responsible for building up the model then executing it, then providing results.

This is based heavily on the Holy Priest spreadsheet models used and refined throughout WoD, Legion, BfA and Shadowlands.

## Basic Usage

A modelling run generates results based on a `GameState`.

The **GameState** has 2 main components:

1. **Profile** - A profile consists of information describing the actor and their playstyle. The important parts:
   - **Casts** describe the usage efficiency, overheal and targets for spell casts (including items).
   - **Items** is a list of gear the actor is wearing.
   - **Talents** is a list of the actor's talents and their ranks.
   - **PlaystyleEntries** are a list of additional configuration parameters applied to the behaviour of the model, such as forcing stats and spell usage assumptions.
1. **Constants** - this is game data that describes the game such as stat costs and spell data.

The Profile's **Casts** and **PlaystyleEntries** define how the actor plays the game, with **Talents** selected while equipping **Items**.

```csharp
// Create a default profile using a supported Spec
PlayerProfile profile = IProfileService.GetDefaultProfile(Spec.HolyPriest);

// Create a valid game state
// This will check the game state and remove data that can't be processed.
// Note: This doesn't follow game rules, such as too many talent points spent...
// ... it makes sure the input won't generate an error when the results are generated.
GameState gameState = _gameStateService.CreateValidatedGameState(profile);

// Generate results by running the gamestate against the desired modelling service.
// For Holy Priest this is HolyPriestModellingService
BaseModelResults results = IModellingService.GetResults(gameState);
```

#### GetResults

The `HolyPriestModellingService` implementation of `IModellingService.GetResults` generates results following these basic steps:

1. Build a results object
1. Register the base spells a Holy Priest has with `IGameStateService.RegisterSpells`
1. Loop through each registered spell from `IGameStateService.GetRegisteredSpells` and get the cast results from its associated SpellService using `ISpellService.GetCastResults`
1. Collate and summarise the results into the results object

### Applying a /simc import string

Before creating a gamestate, a profile can be modified by applying a `/simc` import string from the SimulationCraft in-game addon.

```csharp

// Apply additional info to the profile
// This can be done by throwing a /simc import string at it using the SimcProfileParser package
var simcImportString = "This comes from the /simc addon in game";
profile = await ISimcProfileService.ApplySimcProfileAsync(simcImportString, profile);
```

### Deep Cloning a GameState

To "deep clone" a game state, `IGameStateService.CloneGameState` will serialise to JSON then deserialise.

```csharp
var newGameState = IGameStateService.CloneGameState(oldGameState);
```

This can be useful for cloning a game state to then make minor modifications and comparing results.

### Deep Cloning a Profile

To "deep clone" a game state, `IProfileService.CloneProfile` will serialise to JSON then deserialise.

```csharp
var newProfile = IProfileService.CloneProfile(oldProfile);
```

This can be useful for cloning a profile to then make minor modifications and comparing results.

## Implementing a new spell

An example of implementation using a basic passive talent, Lightwell.

1. Make sure the spell is in the `Spell` enum in `Spell.cs` with the correct SpellId

```csharp
...
Lightwell = 372835,
...
/// <summary>
/// The heal portion of Lightwell
/// </summary>
LightwellHeal = 372847,
/// <summary>
/// This is the buff the lightwell has, which stores remaining charges
/// </summary>
LightwellCharges = 372838,
/// <summary>
/// Stores how often it tries to heal someone
/// </summary>
LightwellTrigger = 372845,
...
```

2. Make sure that `constants.json` contains the appropriate spelldata. 

To do this, check `Salvation.Utility.HolyPriestSpellDataService.cs` and ensure it's in the `_spells` list.

```csharp
_spells = new List<uint>()
{
    ...
    (uint)Spell.Lightwell,
    ...
    (uint)Spell.LightwellHeal,
    (uint)Spell.LightwellTrigger,
    (uint)Spell.LightwellCharges,
    ...
}
```

Then run `Salvation.Explorer` with the command line argument `-updatespelldata`. This should include any missing spell data in `constants.json`.

3. Create a new interface for the spell implementation in the appropriate `Salvation.Core.Interfaces` namespace.

`ILightwellSpellService.cs`

```csharp
namespace Salvation.Core.Interfaces.Modelling.HolyPriest.Spells
{
    public interface ILightwellSpellService : ISpellService
    {

    }
}
```

4. Create a new spell in the appropriate `Salvation.Core.Modelling` namespace. This should inherit `SpellService` and implement `ISpellService<T>`

`Lightwell.cs`

```csharp
namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class Lightwell : SpellService, ISpellService<ILightwellSpellService>
    {
        public Lightwell(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.Lightwell;
        }
    }
}
```

5. Create a test for it in `Salvation.CoreTests.HolyPriest.Spells` to test relevant methods. 

Casted healing spells can have most of their basic tests put in `SpellServiceTests.cs`, just add an entry for each test for the new spell.

```csharp
    ...
    yield return new TestCaseData(typeof(Lightwell)).Returns(false);
    ...
```

For any additional tests that don't fit inside this, create a new file named after the spell and include any 
tests required. Use one of the existing spell test sets as a template, it should inherit `BaseTest` to 
save on scaffolding.

You can then run the tests and watch it fail.

6. From here, implement the spell inside `Lightwell.cs`, making sure to supply a relevant test for each method 
(and any variants to improve coverage). Override each of the `SpellService` methods that make sense to give valid 
output for each.

For Lightwell this would include methods such as:

- GetAverageRawHealing
- GetMinimumHealTargets
- GetMaximumHealTargets
- GetMaximumCastsPerMinute

Sometimes the data needed won't be in the spelldata for the defined spell, and is contained in additional spells. 
Add these in steps 1 & 2. Then you can pull the data as required.

```csharp
var lightwellRenewSpelldata = _gameStateService.GetSpellData(gameState, Spell.LightwellHeal);

var healingSp = lightwellRenewSpelldata.GetEffect(997691).SpCoefficient;
```

7. Include the spell in the `DependencyInjectionExtensions.cs` which loads up the DI via an extension method.

```csharp
public static IServiceCollection AddHolyPriestSpells(this IServiceCollection services)
{
    ...
    // Talents
    services.AddSingleton<ISpellService<ILightwellSpellService>, Lightwell>();
    ...
}
```

8. Include the spell in `SpellServiceFactory.cs:GetSpellService` to map `Spell.Lightwell` to `ILightwellSpellService`.

```csharp
Spell.Lightwell => typeof(ILightwellSpellService),
```