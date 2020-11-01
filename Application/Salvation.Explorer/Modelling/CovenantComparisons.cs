using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System.Collections.Generic;
using System.Linq;

namespace Salvation.Explorer.Modelling
{
    public class CovenantComparisonsResult
    {
        public Dictionary<string, BaseModelResults> Results { get; set; }

        public CovenantComparisonsResult() { Results = new Dictionary<string, BaseModelResults>(); }
        public CovenantComparisonsResult(Dictionary<string, BaseModelResults> results)
        {
            Results = results;
        }
    }

    /// <summary>
    /// TODO: Replace all of these calls with the new modelling service
    /// </summary>
    class CovenantComparisons : IComparisonModeller<CovenantComparisonsResult>
    {
        private readonly IProfileService _profileService;
        private readonly IModellingService _modellingService;
        private readonly IConstantsService _constantsService;
        private readonly IGameStateService _gameStateService;

        // The goal for this class is to build a bunch of different profiles to 
        // show how the covenant abilities work in various scenarios
        // For each profile:
        // 1. Add covenant
        // 2. Update the efficiency usage

        public CovenantComparisons(IProfileService profileService,
            IModellingService modellingService,
            IConstantsService constantsService,
            IGameStateService gameStateService)
        {
            _profileService = profileService;
            _modellingService = modellingService;
            _constantsService = constantsService;
            _gameStateService = gameStateService;
        }

        public CovenantComparisonsResult RunComparison()
        {
            var results = new Dictionary<string, BaseModelResults>();

            // Generate all the states to run
            var states = new List<GameState>();

            states.AddRange(GetBoonStates());
            states.AddRange(GetUnholyNovaStates());
            states.Add(GetBaseState());
            states.Add(GetMindgamesState());
            states.Add(GetFaeGuardianState("Fae 4k DTPS Only", 4000, 0));
            states.Add(GetFaeGuardianState("Fae 5k DTPS Only", 5000, 0));
            states.Add(GetFaeGuardianState("Fae 6k DTPS Only", 6000, 0));
            states.Add(GetFaeGuardianState("Fae Hymn Only", 0, 1));
            states.Add(GetFaeGuardianState("Fae 4k DTPS + Hymn", 4000, 1));
            states.Add(GetFaeGuardianState("Fae 5k DTPS + Hymn", 5000, 1));
            states.Add(GetFaeGuardianState("Fae 6k DTPS + Hymn", 6000, 1));

            // Run them
            foreach (var state in states)
            {
                var modelResult = _modellingService.GetResults(state);

                results.Add(modelResult.Profile.Name, modelResult);
            }

            var baselineResults = results.Where(a => a.Key == "Baseline").FirstOrDefault().Value;

            return new CovenantComparisonsResult(results);
        }

        public PlayerProfile GetBaseProfile()
        {
            var baseProfile = _profileService.GetDefaultProfile(Spec.HolyPriest);

            return baseProfile;
        }

        public GameState GetBaseState()
        {
            var profile = GetBaseProfile();

            var state = _gameStateService.CreateValidatedGameState(profile);

            _gameStateService.SetProfileName(state, "Baseline");

            return state;
        }

        public GameState GetMindgamesState()
        {
            var profile = GetBaseProfile();

            _profileService.SetCovenant(profile, 
                new CovenantProfile() { Covenant = Covenant.Venthyr });

            var state = _gameStateService.CreateValidatedGameState(profile);

            _gameStateService.SetProfileName(state, "Mindgames on CD");

            _gameStateService.SetSpellCastProfile(state, new CastProfile()
            {
                SpellId = (int)Spell.Mindgames,
                Efficiency = 1d,
                OverhealPercent = 0d
            });

            return state;
        }

        public List<GameState> GetBoonStates()
        {
            var results = new List<GameState>();
            // Combinations:
            // AB + AN | AB | AN
            // F: 1, 5, 10, 20, 30
            // E: 1, 2, 3, 4, 5, 10

            var casts = new int[3, 2] { { 1, 1 }, { 1, 0 }, { 0, 1 } };
            var enemies = new List<double>() { 1, 5, 10 };
            var friendlies = new List<double>() { 1, 5, 10, 20 };

            for (var i = 0; i < casts.GetLength(0); i++)
            {
                foreach (var numEnemy in enemies)
                {
                    foreach (var numFriendly in friendlies)
                    {
                        var profileName = "Boon ";
                        profileName += casts[i, 0] == 1 ? "AB " : "";
                        profileName += casts[i, 1] == 1 ? "AN " : "";
                        profileName += $"F{numFriendly}";
                        profileName += $"E{numEnemy}";
                        var boonState = GetBoonState(profileName, casts[i, 0],
                            casts[i, 1], numEnemy, numFriendly);

                        results.Add(boonState);
                    }
                }
            }

            return results;
        }

        public GameState GetBoonState(string profileName, double abEfficiency, double anEfficiency,
            double enemyTargets, double friendlyTargets)
        {
            var profile = GetBaseProfile();

            _profileService.SetCovenant(profile,
                new CovenantProfile() { Covenant = Covenant.Kyrian });

            var state = _gameStateService.CreateValidatedGameState(profile);

            _gameStateService.SetSpellCastProfile(state, new CastProfile()
            {
                SpellId = (int)Spell.AscendedBlast,
                Efficiency = abEfficiency,
                OverhealPercent = 0d
            });
            _gameStateService.SetSpellCastProfile(state, new CastProfile()
            {
                SpellId = (int)Spell.AscendedNova,
                Efficiency = anEfficiency,
                OverhealPercent = 0d
            });
            _gameStateService.SetSpellCastProfile(state, new CastProfile()
            {
                SpellId = (int)Spell.AscendedEruption,
                Efficiency = 1d,
                OverhealPercent = 0d
            });

            _gameStateService.SetProfileName(state, profileName);

            var anData = _gameStateService.GetSpellData(state, Spell.AscendedNova);

            anData.Overrides[Override.NumberOfHealingTargets] = friendlyTargets;
            anData.Overrides[Override.NumberOfDamageTargets] = enemyTargets;

            var aeData = _gameStateService.GetSpellData(state, Spell.AscendedEruption);

            aeData.Overrides[Override.NumberOfHealingTargets] = friendlyTargets;
            aeData.Overrides[Override.NumberOfDamageTargets] = enemyTargets;

            _gameStateService.OverrideSpellData(state, anData);
            _gameStateService.OverrideSpellData(state, aeData);

            return state;
        }

        public GameState GetFaeGuardianState(string profileName, double guardianDTPS,
            double selfCDRUsage)
        {
            var profile = GetBaseProfile();

            _profileService.SetCovenant(profile,
                new CovenantProfile() { Covenant = Covenant.NightFae });

            var state = _gameStateService.CreateValidatedGameState(profile);

            _gameStateService.SetSpellCastProfile(state, new CastProfile()
            {
                SpellId = (int)Spell.FaeGuardians,
                Efficiency = 1d,
                OverhealPercent = 0d
            });

            _gameStateService.SetProfileName(state, profileName);

            var dtpsModifier = _gameStateService.GetPlaystyle(state, "FaeGuardianFaerieDTPS");
            dtpsModifier.Value = guardianDTPS;

            _gameStateService.OverridePlaystyle(state, dtpsModifier);
            var selfCDRUsageModifier = _gameStateService.GetPlaystyle(state, "FaeBenevolentFaerieSelfUptime");
            selfCDRUsageModifier.Value = selfCDRUsage;

            _gameStateService.OverridePlaystyle(state, selfCDRUsageModifier);

            return state;
        }
        public List<GameState> GetUnholyNovaStates()
        {
            var results = new List<GameState>();
            // Combinations:
            // F: 1, 5, 10, 20, 30

            var friendlies = new List<double>() { 1, 5, 10, 20, 30 };

            foreach (var numFriendly in friendlies)
            {
                var profileName = $"UnholyNova F{numFriendly}";
                var unState = GetUnholyNovaState(profileName, numFriendly);

                results.Add(unState);
            }

            return results;
        }

        public GameState GetUnholyNovaState(string profileName, double friendlyTargets)
        {
            var profile = GetBaseProfile();

            _profileService.SetCovenant(profile,
                new CovenantProfile() { Covenant = Covenant.Necrolord });

            var state = _gameStateService.CreateValidatedGameState(profile);

            _gameStateService.SetSpellCastProfile(state, new CastProfile()
            {
                SpellId = (int)Spell.UnholyNova,
                Efficiency = 1,
                OverhealPercent = 0d
            });

            _gameStateService.SetProfileName(state, profileName);

            var unData = _gameStateService.GetSpellData(state, Spell.UnholyNova);

            unData.Overrides[Override.NumberOfHealingTargets] = friendlyTargets;

            var utData = _gameStateService.GetSpellData(state, Spell.UnholyTransfusion);

            utData.Overrides[Override.NumberOfHealingTargets] = friendlyTargets;

            _gameStateService.OverrideSpellData(state, unData);
            _gameStateService.OverrideSpellData(state, utData);

            return state;
        }
    }
}
