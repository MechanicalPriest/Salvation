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
            states.AddRange(GetNecrolordStates(baseState));
            states.AddRange(GetNightFaeStates(baseState));
            states.AddRange(GetVenthyrStates(baseState));
            states.AddRange(GetLegendaryStates(baseState));

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
            states.Add(SetSingleConduit(baseKyrianState, "kyrian_courageous_ascension", Conduit.CourageousAscension, 7));
            states.AddRange(GetKyrianPelagosStates(baseKyrianState));

            return states;
        }

        private IEnumerable<GameState> GetKyrianPelagosStates(GameState baseKyrianState)
        {
            var states = new List<GameState>();



            return states;
        }

        public List<GameState> GetNecrolordStates(GameState baseState)
        {
            var states = new List<GameState>();

            var baseNecroState = _gameStateService.CloneGameState(baseState);

            _gameStateService.SetCovenant(baseNecroState, new CovenantProfile()
            {
                Covenant = Covenant.Necrolord,
            });
            _gameStateService.SetProfileName(baseNecroState, "necro_base");

            states.Add(baseNecroState);
            states.Add(SetSingleConduit(baseNecroState, "necro_festering_transfusion", Conduit.FesteringTransfusion, 7));
            states.AddRange(GetKyrianPelagosStates(baseNecroState));

            return states;
        }

        public List<GameState> GetNightFaeStates(GameState baseState)
        {
            var states = new List<GameState>();

            var baseNightFaeState = _gameStateService.CloneGameState(baseState);

            _gameStateService.SetCovenant(baseNightFaeState, new CovenantProfile()
            {
                Covenant = Covenant.NightFae,
            });
            _gameStateService.SetProfileName(baseNightFaeState, "nf_base");

            states.Add(baseNightFaeState);
            states.Add(SetSingleConduit(baseNightFaeState, "nf_fae_fermata", Conduit.FaeFermata, 7));
            states.AddRange(GetKyrianPelagosStates(baseNightFaeState));

            return states;
        }

        public List<GameState> GetVenthyrStates(GameState baseState)
        {
            var states = new List<GameState>();

            var baseVenthyrState = _gameStateService.CloneGameState(baseState);

            _gameStateService.SetCovenant(baseVenthyrState, new CovenantProfile()
            {
                Covenant = Covenant.Venthyr,
            });
            _gameStateService.SetProfileName(baseVenthyrState, "venthyr_base");

            states.Add(baseVenthyrState);
            states.Add(SetSingleConduit(baseVenthyrState, "venthyr_shattered_perceptions", Conduit.ShatteredPerceptions, 7));
            states.AddRange(GetKyrianPelagosStates(baseVenthyrState));

            return states;
        }

        private IEnumerable<GameState> GetSingleComparisons(GameState baseState)
        {
            var states = new List<GameState>();

            // --------------------- Soulbinds ---------------------
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_brons_call_to_action", SoulbindTrait.BronsCalltoAction));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_combat_meditation", SoulbindTrait.CombatMeditation));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_field_of_blossoms", SoulbindTrait.FieldofBlossoms));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_grove_invigoration", SoulbindTrait.GroveInvigoration));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_let_go_of_the_past", SoulbindTrait.LetGoofthePast));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_marrowed_gemstone", SoulbindTrait.HeirmirsArsenalMarrowedGemstone));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_niyas_tools_herbs", SoulbindTrait.NiyasToolsHerbs));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_pointed_courage", SoulbindTrait.PointedCourage));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_resonant_accolades", SoulbindTrait.ResonantAccolades));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_soothing_shade", SoulbindTrait.SoothingShade));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_thrill_seeker", SoulbindTrait.ThrillSeeker));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_ultimate_form", SoulbindTrait.UltimateForm));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_valiant_strikes", SoulbindTrait.ValiantStrikes));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_volatile_solvent", SoulbindTrait.VolatileSolvent));
            states.Add(SetSingleSoulbindTrait(baseState, "base_sb_lead_by_example", SoulbindTrait.LeadByExample));

            // --------------------- Conduits ---------------------
            states.Add(SetSingleConduit(baseState, "base_cn_charitable_soul", Conduit.CharitableSoul, 7));
            states.Add(SetSingleConduit(baseState, "base_cn_focused_mending", Conduit.FocusedMending, 7));
            states.Add(SetSingleConduit(baseState, "base_cn_holy_oration", Conduit.HolyOration, 7));
            states.Add(SetSingleConduit(baseState, "base_cn_resonant_words", Conduit.ResonantWords, 7));
            states.Add(SetSingleConduit(baseState, "base_cn_lasting_spirit", Conduit.LastingSpirit, 7));

            return states;
        }

        private IEnumerable<GameState> GetLegendaryStates(GameState baseState)
        {
            var states = new List<GameState>();

            var legendaries = new Dictionary<string, uint>()
            {
                { "harmonious_apparatus", (uint)Spell.HarmoniousApparatus },
                { "echo_of_eonar", (uint)Spell.EchoOfEonar },
                { "flash_concentration", (uint)Spell.FlashConcentration },
                { "divine_image", (uint)Spell.DivineImage }
            };

            foreach(var legendary in legendaries)
            {
                var newState = _gameStateService.CloneGameState(baseState);

                var harmoniousApparatusItem = new Item()
                {
                    Equipped = true
                };
                harmoniousApparatusItem.Effects.Add(new ItemEffect()
                {
                    Spell = new Core.Constants.BaseSpellData()
                    {
                        Id = legendary.Value
                    }
                });
                newState.Profile.Items.Add(harmoniousApparatusItem);
                _gameStateService.SetProfileName(newState, $"le_{legendary.Key}");

                states.Add(newState);
            }

            return states;
        }

        private GameState SetSingleSoulbindTrait(GameState baseState, string profileName, SoulbindTrait trait)
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

        private GameState SetSingleConduit(GameState baseState, string profileName, Conduit conduit, uint rank)
        {
            var newState = _gameStateService.CloneGameState(baseState);

            var covenant = newState.Profile.Covenant;

            if (covenant == null)
                covenant = new CovenantProfile();

            covenant.Soulbinds.Add(
                new SoulbindProfile()
                {
                    IsActive = true,
                    ActiveConduits = new Dictionary<Conduit, uint>()
                    {
                        { conduit, rank }
                    }
                });

            _gameStateService.SetProfileName(newState, profileName);

            return newState;
        }
    }
}
