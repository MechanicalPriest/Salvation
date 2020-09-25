using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Modelling.HolyPriest;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Salvation.Explorer.Modelling
{
    /// <summary>
    /// TODO: Replace all of these calls with the new modelling service
    /// </summary>
    class CovenantComparisons : IComparisonModeller<CovenantComparisons>
    {
        private readonly IProfileGenerationService profileGenerationService;
        private readonly IModellingService modellingService;
        private readonly IConstantsService constantsService;
        private readonly IGameStateService gameStateService;

        // The goal for this class is to build a bunch of different profiles to 
        // show how the covenant abilities work in various scenarios
        // For each profile:
        // 1. Add covenant
        // 2. Update the efficiency usage

        public CovenantComparisons(IProfileGenerationService profileGenerationService,
            IModellingService modellingService,
            IConstantsService constantsService,
            IGameStateService gameStateService)
        {
            this.profileGenerationService = profileGenerationService;
            this.modellingService = modellingService;
            this.constantsService = constantsService;
            this.gameStateService = gameStateService;
        }

        public object RunComparison()
        {
            var results = new Dictionary<string, BaseModelResults>();

            // Generate all the states to run
            var states = new List<GameState>();

            states.Add(GetBaseState());
            states.AddRange(GetBoonStates());
            states.AddRange(GetUnholyNovaStates());
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
                var modelResult = modellingService.GetResults(state);

                results.Add(modelResult.Profile.Name, modelResult);
            }

            var baselineResults = results.Where(a => a.Key == "Baseline").FirstOrDefault().Value;
            if(baselineResults != null)
            {
                foreach(var result in results)
                {
                    Console.WriteLine($"[{result.Key}] " +
                        $"RawHPS: {result.Value.TotalRawHPS - baselineResults.TotalRawHPS:0.##} " +
                        $"ActualHPS: {result.Value.TotalActualHPS - baselineResults.TotalActualHPS:0.##} ");
                }
            }

            foreach (var result in results)
            {
                Console.WriteLine($"{result.Key}, {result.Value.TotalRawHPS - baselineResults.TotalRawHPS:0.##}");
            }

            return results;
        }

        public PlayerProfile GetBaseProfile()
        {
            var baseProfile = profileGenerationService.GetDefaultProfile(Spec.HolyPriest);

            return baseProfile;
        }

        public GameState GetBaseState()
        {
            var profile = GetBaseProfile();
            var constants = constantsService.LoadConstantsFromFile();

            profileGenerationService.SetProfileName(profile, "Baseline");

            var state = new GameState(profile, constants);

            return state;
        }

        public GameState GetMindgamesState()
        {
            var profile = GetBaseProfile();
            var constants = constantsService.LoadConstantsFromFile();

            profileGenerationService.SetCovenant(profile, Covenant.Venthyr);
            profileGenerationService.SetProfileName(profile, "Mindgames on CD");
            profileGenerationService.SetSpellCastProfile(profile, new CastProfile()
            {
                SpellId = (int)SpellIds.Mindgames,
                Efficiency = 1m,
                OverhealPercent = 0m
            });

            var state = new GameState(profile, constants);

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
            var enemies = new List<decimal>() { 1, 5, 10 };
            var friendlies = new List<decimal>() { 1, 5, 10, 20 };

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

        public GameState GetBoonState(string profileName, decimal abEfficiency, decimal anEfficiency,
            decimal enemyTargets, decimal friendlyTargets)
        {
            var profile = GetBaseProfile();
            var constants = constantsService.LoadConstantsFromFile();

            profileGenerationService.SetCovenant(profile, Covenant.Kyrian);
            profileGenerationService.SetProfileName(profile, profileName);
            profileGenerationService.SetSpellCastProfile(profile, new CastProfile()
            {
                SpellId = (int)SpellIds.AscendedBlast,
                Efficiency = abEfficiency,
                OverhealPercent = 0m
            });
            profileGenerationService.SetSpellCastProfile(profile, new CastProfile()
            {
                SpellId = (int)SpellIds.AscendedNova,
                Efficiency = anEfficiency,
                OverhealPercent = 0m
            });
            profileGenerationService.SetSpellCastProfile(profile, new CastProfile()
            {
                SpellId = (int)SpellIds.AscendedEruption,
                Efficiency = 1m,
                OverhealPercent = 0m
            });

            var state = new GameState(profile, constants);

            var anData = gameStateService.GetSpellData(state, SpellIds.AscendedNova);

            anData.NumberOfHealingTargets = friendlyTargets;
            anData.NumberOfDamageTargets = enemyTargets;

            var aeData = gameStateService.GetSpellData(state, SpellIds.AscendedEruption);

            aeData.NumberOfHealingTargets = friendlyTargets;
            aeData.NumberOfDamageTargets = enemyTargets;

            gameStateService.OverrideSpellData(state, anData);
            gameStateService.OverrideSpellData(state, aeData);

            return state;
        }

        public GameState GetFaeGuardianState(string profileName, decimal guardianDTPS, 
            decimal selfCDRUsage)
        {
            var profile = GetBaseProfile();
            var constants = constantsService.LoadConstantsFromFile();

            profileGenerationService.SetCovenant(profile, Covenant.NightFae);
            profileGenerationService.SetProfileName(profile, profileName);
            profileGenerationService.SetSpellCastProfile(profile, new CastProfile()
            {
                SpellId = (int)SpellIds.FaeGuardians,
                Efficiency = 1m,
                OverhealPercent = 0m
            });

            var state = new GameState(profile, constants);

            var dtpsModifier = gameStateService.GetModifier(state, "FaeGuardianFaerieDTPS");
            dtpsModifier.Value = guardianDTPS;

            gameStateService.OverrideModifier(state, dtpsModifier);
            var selfCDRUsageModifier = gameStateService.GetModifier(state, "FaeBenevolentFaerieSelfUptime");
            selfCDRUsageModifier.Value = selfCDRUsage;

            gameStateService.OverrideModifier(state, selfCDRUsageModifier);

            return state;
        }
        public List<GameState> GetUnholyNovaStates()
        {
            var results = new List<GameState>();
            // Combinations:
            // F: 1, 5, 10, 20, 30

            var friendlies = new List<decimal>() { 1, 5, 10, 20, 30 };

            foreach (var numFriendly in friendlies)
            {
                var profileName = $"UnholyNova F{numFriendly}";
                var unState = GetUnholyNovaState(profileName, numFriendly);

                results.Add(unState);
            }

            return results;
        }

        public GameState GetUnholyNovaState(string profileName, decimal friendlyTargets)
        {
            var profile = GetBaseProfile();
            var constants = constantsService.LoadConstantsFromFile();

            profileGenerationService.SetCovenant(profile, Covenant.Necrolord);
            profileGenerationService.SetProfileName(profile, profileName);
            profileGenerationService.SetSpellCastProfile(profile, new CastProfile()
            {
                SpellId = (int)SpellIds.UnholyNova,
                Efficiency = 1,
                OverhealPercent = 0m
            });

            var state = new GameState(profile, constants);

            var unData = gameStateService.GetSpellData(state, SpellIds.UnholyNova);

            unData.NumberOfHealingTargets = friendlyTargets;

            var utData = gameStateService.GetSpellData(state, SpellIds.UnholyTransfusion);

            utData.NumberOfHealingTargets = friendlyTargets;

            gameStateService.OverrideSpellData(state, unData);
            gameStateService.OverrideSpellData(state, utData);

            return state;
        }
    }
}
