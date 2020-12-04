using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using SimcProfileParser.Interfaces;
using SimcProfileParser.Model.Generated;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Explorer.Modelling
{
    public class AdvancedComparisonResult
    {
        public Dictionary<string, BaseModelResults> Results { get; set; }

        public AdvancedComparisonResult() { Results = new Dictionary<string, BaseModelResults>(); }
        public AdvancedComparisonResult(Dictionary<string, BaseModelResults> results)
        {
            Results = results;
        }
    }

    public class AdvancedComparison : IComparisonModeller<AdvancedComparisonResult>
    {
        private readonly IProfileService _profileService;
        private readonly IModellingService _modellingService;
        private readonly IGameStateService _gameStateService;
        private readonly ISimcProfileService _simcProfileService;
        private readonly ISimcGenerationService _simcGenerationService;

        public AdvancedComparison(IProfileService profileService,
            IModellingService modellingService,
            IGameStateService gameStateService,
            ISimcProfileService simcProfileService,
            ISimcGenerationService simcGenerationService)
        {
            _profileService = profileService;
            _modellingService = modellingService;
            _gameStateService = gameStateService;
            _simcProfileService = simcProfileService;
            _simcGenerationService = simcGenerationService;
        }

        public async Task<AdvancedComparisonResult> RunComparison(GameState baseState)
        {
            var results = new Dictionary<string, BaseModelResults>();
            _gameStateService.SetProfileName(baseState, "base");

            // Generate all the states to run
            var states = new List<GameState>
            {
                baseState
            };

            states.AddRange(GetSingleComparisons(baseState));
            states.AddRange(GetKyrianStates(baseState));
            states.AddRange(GetNecrolordStates(baseState));
            states.AddRange(GetNightFaeStates(baseState));
            states.AddRange(GetVenthyrStates(baseState));
            states.AddRange(GetLegendaryStates(baseState));
            states.AddRange(await GetTrinketStates(baseState));

            // Run them
            foreach (var state in states)
            {
                await Task.Run(() =>
                {
                    var modelResult = _modellingService.GetResults(state);

                    results.Add(modelResult.Profile.Name, modelResult);
                });
            }

            return new AdvancedComparisonResult(results);
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
            var states = new List<GameState>
            {

                // --------------------- Soulbinds ---------------------
                SetSingleSoulbindTrait(baseState, "base_sb_brons_call_to_action", SoulbindTrait.BronsCalltoAction),
                SetSingleSoulbindTrait(baseState, "base_sb_combat_meditation", SoulbindTrait.CombatMeditation),
                SetSingleSoulbindTrait(baseState, "base_sb_field_of_blossoms", SoulbindTrait.FieldofBlossoms),
                SetSingleSoulbindTrait(baseState, "base_sb_grove_invigoration", SoulbindTrait.GroveInvigoration),
                SetSingleSoulbindTrait(baseState, "base_sb_let_go_of_the_past", SoulbindTrait.LetGoofthePast),
                SetSingleSoulbindTrait(baseState, "base_sb_marrowed_gemstone", SoulbindTrait.HeirmirsArsenalMarrowedGemstone),
                SetSingleSoulbindTrait(baseState, "base_sb_niyas_tools_herbs", SoulbindTrait.NiyasToolsHerbs),
                SetSingleSoulbindTrait(baseState, "base_sb_pointed_courage", SoulbindTrait.PointedCourage),
                SetSingleSoulbindTrait(baseState, "base_sb_resonant_accolades", SoulbindTrait.ResonantAccolades),
                SetSingleSoulbindTrait(baseState, "base_sb_soothing_shade", SoulbindTrait.SoothingShade),
                SetSingleSoulbindTrait(baseState, "base_sb_thrill_seeker", SoulbindTrait.ThrillSeeker),
                SetSingleSoulbindTrait(baseState, "base_sb_ultimate_form", SoulbindTrait.UltimateForm),
                SetSingleSoulbindTrait(baseState, "base_sb_valiant_strikes", SoulbindTrait.ValiantStrikes),
                SetSingleSoulbindTrait(baseState, "base_sb_volatile_solvent", SoulbindTrait.VolatileSolvent),
                SetSingleSoulbindTrait(baseState, "base_sb_lead_by_example", SoulbindTrait.LeadByExample),

                // --------------------- Conduits ---------------------
                SetSingleConduit(baseState, "base_cn_charitable_soul", Conduit.CharitableSoul, 7),
                SetSingleConduit(baseState, "base_cn_focused_mending", Conduit.FocusedMending, 7),
                SetSingleConduit(baseState, "base_cn_holy_oration", Conduit.HolyOration, 7),
                SetSingleConduit(baseState, "base_cn_resonant_words", Conduit.ResonantWords, 7),
                SetSingleConduit(baseState, "base_cn_lasting_spirit", Conduit.LastingSpirit, 7)
            };

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

                var legendaryItem = new Item()
                {
                    Equipped = true
                };

                legendaryItem.Effects.Add(new ItemEffect()
                {
                    Spell = new Core.Constants.BaseSpellData()
                    {
                        Id = legendary.Value
                    }
                });

                newState.Profile.Items.Add(legendaryItem);
                _gameStateService.SetProfileName(newState, $"le_{legendary.Key}");

                states.Add(newState);
            }

            return states;
        }

        private async Task<IEnumerable<GameState>> GetTrinketStates(GameState baseState)
        {
            var states = new List<GameState>();

            var trinkets = new Dictionary<string, SimcItemOptions>()
            {
                { "cabalists_hymnal", new SimcItemOptions() { ItemId = 184028, ItemLevel = 226 } },
                { "unbound_changeling", new SimcItemOptions() { ItemId = 178708, ItemLevel = 226, BonusIds = new List<int>() { 6917 } } },
                { "soulletting_ruby", new SimcItemOptions() { ItemId = 178809, ItemLevel = 226 } },
            };

            foreach (var trinket in trinkets)
            {
                var newState = _gameStateService.CloneGameState(baseState);

                var trinketItem = await _simcGenerationService.GenerateItemAsync(trinket.Value);

                var newItem = _simcProfileService.CreateItem(trinketItem);
                newItem.Equipped = true;

                newState.Profile.Items.Add(newItem);
                _gameStateService.SetProfileName(newState, $"tr_{trinket.Key}");

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
