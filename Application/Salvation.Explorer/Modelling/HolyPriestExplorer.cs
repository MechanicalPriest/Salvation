using Newtonsoft.Json;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Explorer.Modelling
{
    public interface IHolyPriestExplorer
    {
        public Task GenerateStatWeights();
        public Task TestHolyPriestModelAsync();
    }

    class HolyPriestExplorer : IHolyPriestExplorer
    {
        private readonly IModellingService _modellingService;
        private readonly IProfileService _profileService;
        private readonly IStatWeightGenerationService _statWeightGenerationService;
        private readonly IGameStateService _gameStateService;
        private readonly ISimcProfileService _simcProfileService;
        private readonly IComparisonModeller<AdvancedComparisonResult> _comparisonModellerCovenantAdv;

        public HolyPriestExplorer(IModellingService modellingService,
            IProfileService profileService,
            IStatWeightGenerationService statWeightGenerationService,
            IGameStateService gameStateService,
            ISimcProfileService simcProfileService,
            IComparisonModeller<AdvancedComparisonResult> comparisonModellerCovenantAdv)
        {
            _modellingService = modellingService;
            _profileService = profileService;
            _statWeightGenerationService = statWeightGenerationService;
            _gameStateService = gameStateService;
            _simcProfileService = simcProfileService;
            _comparisonModellerCovenantAdv = comparisonModellerCovenantAdv;
        }

        public async Task GenerateStatWeights()
        {
            // Get default profile
            var profile = _profileService.GetDefaultProfile(Spec.HolyPriest);

            // Apply a simc profile to it
            var profileData = File.ReadAllText(Path.Combine("Profile", "HolyPriest", "dragonflight_fresh.simc"));
            profile = await _simcProfileService.ApplySimcProfileAsync(profileData, profile);

            // Create the gamestate
            GameState state = _gameStateService.CreateValidatedGameState(profile);

            // Run stat weights
            var results = _statWeightGenerationService.Generate(state, 100,
                StatWeightGenerator.StatWeightType.EffectiveHealing);

            Console.WriteLine($"[Stats] Int: {_gameStateService.GetIntellect(state)} " +
                $"Crit: {_gameStateService.GetCriticalStrikeRating(state)} " +
                $"Haste: {_gameStateService.GetHasteRating(state)} " +
                $"Vers: {_gameStateService.GetVersatilityRating(state)} " +
                $"Mastery: {_gameStateService.GetMasteryRating(state)} ");

            Console.WriteLine($"{results.Name} weights:");

            foreach(var result in results.Results)
            {
                Console.WriteLine($"[{result.Stat}] {result.Weight:0.##}");
            }

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
        }

        public async Task TestHolyPriestModelAsync()
        {
            // Get default profile
            var profile = _profileService.GetDefaultProfile(Spec.HolyPriest);

            // Apply a simc profile to it
            var profileData = File.ReadAllText(Path.Combine("Profile", "HolyPriest", "dragonflight_fresh.simc"));
            profile = await _simcProfileService.ApplySimcProfileAsync(profileData, profile);

            // Create the gamestate
            GameState state = _gameStateService.CreateValidatedGameState(profile);

            // Make some other modifications if needed
            // _profileService.UpdateTalent(profile, Spell.ImprovedFlashHeal, 0);

            // Kick off modelling against it.
            var results = _modellingService.GetResults(state);
            File.WriteAllText("hpriest_model_results.json",
                JsonConvert.SerializeObject(results, Formatting.Indented));
        }
    }
}
