using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Explorer.Modelling
{
    public class AdvancedCovenantComparisonResult
    {
        public Dictionary<string, BaseModelResults> Results { get; set; }

        public AdvancedCovenantComparisonResult() { Results = new Dictionary<string, BaseModelResults>(); }
        public AdvancedCovenantComparisonResult(Dictionary<string, BaseModelResults> results)
        {
            Results = results;
        }
    }

    public class AdvancedCovenantComparison : IComparisonModeller<AdvancedCovenantComparisonResult>
    {
        private readonly IProfileService _profileService;
        private readonly IModellingService _modellingService;
        private readonly IGameStateService _gameStateService;
        private readonly ISimcProfileService _simcProfileService;

        public AdvancedCovenantComparison(IProfileService profileService,
            IModellingService modellingService,
            IGameStateService gameStateService,
            ISimcProfileService simcProfileService)
        {
            _profileService = profileService;
            _modellingService = modellingService;
            _gameStateService = gameStateService;
            _simcProfileService = simcProfileService;
        }

        public async Task<AdvancedCovenantComparisonResult> RunComparison(GameState baseState)
        {
            var results = new Dictionary<string, BaseModelResults>();
            _gameStateService.SetProfileName(baseState, "base");

            // Generate all the states to run
            var states = new List<GameState>();

            states.Add(baseState);
            states.AddRange(GetSingleComparisons(baseState));
            states.AddRange(GetKyrianStates(baseState));

            // Run them
            foreach (var state in states)
            {
                await Task.Run(() =>
                {
                    var modelResult = _modellingService.GetResults(state);

                    results.Add(modelResult.Profile.Name, modelResult);
                });
            }

            return new AdvancedCovenantComparisonResult(results);
        }

        public List<GameState> GetKyrianStates(GameState baseState)
        {
            var states = new List<GameState>();

            var baseKyrianState = _gameStateService.CloneGameState(baseState);

            _gameStateService.SetCovenant(baseKyrianState, new CovenantProfile()
            {
                Covenant = Covenant.Kyrian,
            });
            _gameStateService.SetProfileName(baseKyrianState, "kyrian_base");

            states.Add(baseKyrianState);
            states.AddRange(GetKyrianPelagosStates(baseKyrianState));

            return states;
        }

        private IEnumerable<GameState> GetKyrianPelagosStates(GameState baseKyrianState)
        {
            var states = new List<GameState>();

            return states;
        }

        private IEnumerable<GameState> GetSingleComparisons(GameState baseState)
        {
            var states = new List<GameState>();

            // --------------------- Soulbinds ---------------------
            states.Add(AddSoulbind(baseState, "base_brons_call_to_action", SoulbindTrait.BronsCalltoAction));
            states.Add(AddSoulbind(baseState, "base_combat_meditation", SoulbindTrait.CombatMeditation));
            states.Add(AddSoulbind(baseState, "base_let_go_of_the_past", SoulbindTrait.LetGoofthePast));
            states.Add(AddSoulbind(baseState, "base_pointed_courage", SoulbindTrait.PointedCourage));
            states.Add(AddSoulbind(baseState, "base_resonant_accolades", SoulbindTrait.ResonantAccolades));
            states.Add(AddSoulbind(baseState, "base_ultimate_form", SoulbindTrait.UltimateForm));
            states.Add(AddSoulbind(baseState, "base_valiant_strikes", SoulbindTrait.ValiantStrikes));
            states.Add(AddSoulbind(baseState, "base_volatile_solvent", SoulbindTrait.VolatileSolvent));

            return states;
        }

        private GameState AddSoulbind(GameState baseState, string profileName, SoulbindTrait trait)
        {
            var newState = _gameStateService.CloneGameState(baseState);

            var covenant = newState.Profile.Covenant;

            if (covenant == null)
                covenant = new CovenantProfile();

            covenant.Soulbinds.Add(
                new SoulbindProfile()
                {
                    IsActive = true,
                    ActiveTraits = new List<SoulbindTrait>()
                    {
                        trait
                    }
                });

            _gameStateService.SetProfileName(newState, profileName);

            return newState;
        }
    }
}
