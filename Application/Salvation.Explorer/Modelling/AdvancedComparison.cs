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

            return new AdvancedComparisonResult(results
                .OrderByDescending(r => r.Key.Split('_')[0])
                .ThenByDescending(r => r.Value.TotalActualHPS)
                .ToDictionary(r => r.Key, r => r.Value));
        }

        public List<GameState> GetKyrianStates(GameState baseState)
        {
            var states = new List<GameState>();

            var baseKyrianState = _gameStateService.CloneGameState(baseState);

            _gameStateService.SetCovenant(baseKyrianState, new CovenantProfile()
            {
                Covenant = Covenant.Kyrian,
            });
            _gameStateService.SetProfileName(baseKyrianState, "cov_kyrian_base");

            states.Add(baseKyrianState);
            states.Add(SetSingleConduit(baseKyrianState, "cov_kyrian_courageous_ascension", Conduit.CourageousAscension, 7));
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
            _gameStateService.SetProfileName(baseNecroState, "cov_necro_base");

            states.Add(baseNecroState);
            states.Add(SetSingleConduit(baseNecroState, "cov_necro_festering_transfusion", Conduit.FesteringTransfusion, 7));
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
            _gameStateService.SetProfileName(baseNightFaeState, "cov_nf_base");

            states.Add(baseNightFaeState);
            states.Add(SetSingleConduit(baseNightFaeState, "cov_nf_fae_fermata", Conduit.FaeFermata, 7));
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
            _gameStateService.SetProfileName(baseVenthyrState, "cov_venthyr_base");

            states.Add(baseVenthyrState);
            states.Add(SetSingleConduit(baseVenthyrState, "cov_venthyr_shattered_perceptions", Conduit.ShatteredPerceptions, 7));
            states.AddRange(GetKyrianPelagosStates(baseVenthyrState));

            return states;
        }

        private IEnumerable<GameState> GetSingleComparisons(GameState baseState)
        {
            var states = new List<GameState>
            {

                // --------------------- Soulbinds ---------------------
                SetSingleSoulbindTrait(baseState, "sb_brons_call_to_action", SoulbindTrait.BronsCalltoAction),
                SetSingleSoulbindTrait(baseState, "sb_combat_meditation", SoulbindTrait.CombatMeditation),
                SetSingleSoulbindTrait(baseState, "sb_field_of_blossoms", SoulbindTrait.FieldofBlossoms),
                SetSingleSoulbindTrait(baseState, "sb_grove_invigoration", SoulbindTrait.GroveInvigoration),
                SetSingleSoulbindTrait(baseState, "sb_let_go_of_the_past", SoulbindTrait.LetGoofthePast),
                SetSingleSoulbindTrait(baseState, "sb_marrowed_gemstone", SoulbindTrait.HeirmirsArsenalMarrowedGemstone),
                SetSingleSoulbindTrait(baseState, "sb_niyas_tools_herbs", SoulbindTrait.NiyasToolsHerbs),
                SetSingleSoulbindTrait(baseState, "sb_pointed_courage", SoulbindTrait.PointedCourage),
                SetSingleSoulbindTrait(baseState, "sb_resonant_accolades", SoulbindTrait.ResonantAccolades),
                SetSingleSoulbindTrait(baseState, "sb_soothing_shade", SoulbindTrait.SoothingShade),
                SetSingleSoulbindTrait(baseState, "sb_thrill_seeker", SoulbindTrait.ThrillSeeker),
                SetSingleSoulbindTrait(baseState, "sb_ultimate_form", SoulbindTrait.UltimateForm),
                SetSingleSoulbindTrait(baseState, "sb_valiant_strikes", SoulbindTrait.ValiantStrikes),
                SetSingleSoulbindTrait(baseState, "sb_volatile_solvent", SoulbindTrait.VolatileSolvent),
                SetSingleSoulbindTrait(baseState, "sb_lead_by_example", SoulbindTrait.LeadByExample),

                // --------------------- Conduits ---------------------
                SetSingleConduit(baseState, "cn_charitable_soul", Conduit.CharitableSoul, 7),
                SetSingleConduit(baseState, "cn_focused_mending", Conduit.FocusedMending, 7),
                SetSingleConduit(baseState, "cn_holy_oration", Conduit.HolyOration, 7),
                SetSingleConduit(baseState, "cn_resonant_words", Conduit.ResonantWords, 7),
                SetSingleConduit(baseState, "cn_lasting_spirit", Conduit.LastingSpirit, 7)
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

        class TrinketGenerationTemplate
        {
            internal string Name { get; set; }
            internal SimcItemOptions Options { get; set; }
            internal List<int> ItemLevels { get; set; }

            public TrinketGenerationTemplate()
            {
                ItemLevels = new List<int>();
            }

            public TrinketGenerationTemplate(string name, SimcItemOptions options, params int[] itemLevels)
                : this()
            {
                Name = name;
                Options = options;
                ItemLevels = itemLevels.ToList();
            }
        }

        private async Task<IEnumerable<GameState>> GetTrinketStates(GameState baseState)
        {
            var states = new List<GameState>();

            // Common item levels:
            // Mythic +15 9/12: 236
            // 10/12: 239
            // 11/12: 242
            // 12/12: 246
            // Vault: 252
            // LFRaid raid (8/2): 213/220
            // Normal raid (8/2): 226/233
            // Heroic raid (8/2): 239/246
            // Mythic raid (8/2): 252/259
            // CN Heroic (8/2): 213/220
            // CN Mythic (8/2): 226/233
            // World boss trinket: 233
            // Crafted (Alch): 230

            // Dungeon trinkets: 236, 239, 242, 246, 252
            // Raid trinkets (8): 213, 226, 239, 252
            // Raid trinkets (2): 220, 233, 246, 259
            // World boss: 233
            // Crafted: 230
            // PVP: 220, 226, 233, 239, 246

            var trinkets = new List<TrinketGenerationTemplate>()
            {
                // Castle Nathria
                { new TrinketGenerationTemplate("cabalists_hymnal", new SimcItemOptions() { ItemId = 184028 }, 220, 233) },
                { new TrinketGenerationTemplate("manabound_mirror", new SimcItemOptions() { ItemId = 184029 }, 220, 233) },
                { new TrinketGenerationTemplate("macabre_sheet_music", new SimcItemOptions() { ItemId = 184024 }, 213, 226) },
                { new TrinketGenerationTemplate("consumptive_infusion", new SimcItemOptions() { ItemId = 184022 }, 213, 226) },
                { new TrinketGenerationTemplate("tuft_of_smoldering_plumage", new SimcItemOptions() { ItemId = 184020 }, 213, 226) },

                // Sanctum of Domination
                { new TrinketGenerationTemplate("titanic_ocular_gland", new SimcItemOptions() { ItemId = 186423 }, 213, 226, 239, 252) },
                { new TrinketGenerationTemplate("scrawled_word_of_recall", new SimcItemOptions() { ItemId = 186425 }, 213, 226, 239, 252) },
                { new TrinketGenerationTemplate("shadowed_orb_of_torment", new SimcItemOptions() { ItemId = 186428 }, 213, 226, 239, 252) },
                { new TrinketGenerationTemplate("carved_ivory_keepsake", new SimcItemOptions() { ItemId = 186435 }, 213, 226, 239, 252) },
                { new TrinketGenerationTemplate("resonant_silver_bell", new SimcItemOptions() { ItemId = 186436 }, 220, 233, 246, 259) },

                // Dungeon trinkets
                { new TrinketGenerationTemplate("unbound_changeling_haste", new SimcItemOptions() { ItemId = 178708, BonusIds = new List<int>() { 6917 } }, 236, 239, 242, 246, 252) },
                { new TrinketGenerationTemplate("unbound_changeling_crit", new SimcItemOptions() { ItemId = 178708, BonusIds = new List<int>() { 6916 } }, 236, 239, 242, 246, 252) },
                { new TrinketGenerationTemplate("unbound_changeling_mastery", new SimcItemOptions() { ItemId = 178708, BonusIds = new List<int>() { 6918 } }, 236, 239, 242, 246, 252) },
                { new TrinketGenerationTemplate("unbound_changeling_triple", new SimcItemOptions() { ItemId = 178708, BonusIds = new List<int>() { 6915 } }, 236, 239, 242, 246, 252) },
                { new TrinketGenerationTemplate("soulletting_ruby", new SimcItemOptions() { ItemId = 178809 }, 236, 239, 242, 246, 252) },
                { new TrinketGenerationTemplate("overflowing_anima_cage", new SimcItemOptions() { ItemId = 178849 }, 236, 239, 242, 246, 252) },
                { new TrinketGenerationTemplate("vial_of_spectral_essence", new SimcItemOptions() { ItemId = 178810 }, 236, 239, 242, 246, 252) },
                { new TrinketGenerationTemplate("siphoning_phylactery_shard", new SimcItemOptions() { ItemId = 178783 }, 236, 239, 242, 246, 252) },
                // IQD
                // Sunblood Amethyst
                // Lingering Sunmote
                // Boon of the Archon
                // First Class Healing Distributor 
                // So'leah's Secret Technique
                // 

                // Crafted
                //{ new TrinketGenerationTemplate("darkmoon_deck_repose", new SimcItemOptions() { ItemId = 173078 }, 200) },
                { new TrinketGenerationTemplate("spiritual_alchemy_stone", new SimcItemOptions() { ItemId = 171323 }, 200, 230) },
            };

            foreach (var trinket in trinkets)
            {
                foreach (var itemLevel in trinket.ItemLevels)
                {
                    var newState = _gameStateService.CloneGameState(baseState);

                    trinket.Options.ItemLevel = itemLevel;

                    var trinketItem = await _simcGenerationService.GenerateItemAsync(trinket.Options);

                    var newItem = _simcProfileService.CreateItem(trinketItem);
                    newItem.Equipped = true;

                    newState.Profile.Items.Add(newItem);
                    _gameStateService.SetProfileName(newState, $"tr_{trinket.Name}_{itemLevel}");

                    states.Add(newState);
                }
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
