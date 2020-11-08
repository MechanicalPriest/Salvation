using Newtonsoft.Json;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile;
using Salvation.Core.Profile.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Salvation.Core.State
{
    public class GameStateService : IGameStateService
    {
        private readonly IProfileService _profileService;
        private readonly IConstantsService _constantsService;
        private readonly ISpellServiceFactory _spellServiceFactory;

        public GameStateService(IProfileService profileService,
            IConstantsService constantsService,
            ISpellServiceFactory spellServiceFactory)
        {
            _profileService = profileService;
            _constantsService = constantsService;
            _spellServiceFactory = spellServiceFactory;
        }

        public GameStateService()
            : this(new ProfileService(), new ConstantsService(), null)
        {

        }

        public double GetBaseManaAmount(GameState state)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.Spec).FirstOrDefault();

            return specData.ManaBase;
        }

        public void SetSpellCastProfile(GameState state, CastProfile castProfile)
        {
            _profileService.SetSpellCastProfile(state.Profile, castProfile);
        }

        public CastProfile GetSpellCastProfile(GameState state, int spellId)
        {
            var castProfile = state.Profile.Casts?
                .Where(c => c.SpellId == spellId)
                .FirstOrDefault();

            castProfile = JsonConvert.DeserializeObject<CastProfile>(JsonConvert.SerializeObject(castProfile));

            return castProfile;
        }

        public void SetProfileName(GameState state, string profileName)
        {
            _profileService.SetProfileName(state.Profile, profileName);
        }

        public double GetGCDFloor(GameState state)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.Spec).FirstOrDefault();

            return specData.GCDFloor;
        }
        /// <summary>
        /// Returns diminished rating based on percentage of stat, as changed in 9.0.1
        /// Calcs each individual point one at a time for its value based on the percent bracket of the point
        /// this val * scaling = percent, important note is mastery operates from the point scaling
        /// </summary>
        /// <param name="rating"> Total value of stat on a character prior to DR</param>
        /// <param name="cost"> Cost of individual stat </param>
        /// <returns> diminished returned value based on percent </returns>
        public double GetDrRating(double rating, double cost)
        {
            double result = 0;
            for (int i = 0; i < rating; i++)
            {
                var percent = result / cost;
                if (percent <= 30)
                {
                    result++;
                }
                else if (percent <= 39)
                {
                    result += .9;
                }
                else if (percent <= 47)
                {
                    result += .8;
                }
                else if (percent <= 54)
                {
                    result += .7;
                }
                else if (percent <= 66)
                {
                    result += .6;
                }
                else
                {
                    result += .5;
                }
            }
            return result;
        }

        public double GetCriticalStrikeRating(GameState state)
        {
            var critRating = GetBaseCriticalStrikeRating(state);

            // Add any forced additional stats if specified (stat weights)
            var bonusCriticalStrike = GetPlaystyle(state, "GrantAdditionalStatCriticalStrike");
            if (bonusCriticalStrike != null)
                critRating += bonusCriticalStrike.Value;

            // Get average crit from effects
            foreach (var spell in state.RegisteredSpells.Where(s => s.SpellService != null))
            {
                critRating += spell.SpellService.GetAverageCriticalStrike(state, spell.SpellData);
            }

            return critRating;
        }

        public double GetBaseCriticalStrikeRating(GameState state)
        {
            var playStyleValue = GetPlaystyle(state, "OverrideStatCriticalStrike").Value;
            if (playStyleValue != 0)
            {
                return playStyleValue;
            }

            var critRating = 0d;

            // Get crit from gear
            foreach (var item in _profileService.GetEquippedItems(state.Profile))
            {
                foreach (var mod in item.Mods)
                {
                    if (mod.Type == ItemModType.ITEM_MOD_CRIT_RATING ||
                        mod.Type == ItemModType.ITEM_MOD_CRIT_SPELL_RATING)
                    {
                        critRating += mod.StatRating;
                    }
                }
            }

            return critRating;
        }

        public double GetCriticalStrikeMultiplier(GameState state)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.Spec).FirstOrDefault();
            double criticalEffectMultiplier = specData.CritMultiplier - 1; // 2 by default higher based on items but lowered to 1 for calc below

            // Apply race bonus for crit effect multi
            switch (state.Profile.Race)
            {
                case Race.Dwarf:
                case Race.Tauren:
                    criticalEffectMultiplier += 0.02;
                    break;
                default:
                    break;
            }

            // Min of calc'd multipler and 2 (cant have more than 100% crit i.e. more than 200% dmg/heal from crit) + crit effect
            return Math.Min(1 + specData.CritBase + (GetDrRating(GetCriticalStrikeRating(state), specData.CritCost) / specData.CritCost / 100), 2) * criticalEffectMultiplier;
        }

        public double GetHasteRating(GameState state)
        {
            var hasteRating = GetBaseHasteRating(state);

            // Add any forced additional stats if specified (stat weights)
            var bonusHaste = GetPlaystyle(state, "GrantAdditionalStatHaste");
            if (bonusHaste != null)
                hasteRating += bonusHaste.Value;

            // Get average haste from effects
            foreach (var spell in state.RegisteredSpells.Where(s => s.SpellService != null))
            {
                hasteRating += spell.SpellService.GetAverageHaste(state, spell.SpellData);
            }

            return hasteRating;
        }

        internal double GetBaseHasteRating(GameState state)
        {
            var playStyleValue = GetPlaystyle(state, "OverrideStatHaste").Value;
            if (playStyleValue != 0)
            {
                return playStyleValue;
            }

            var hasteRating = 0d;

            // Get haste from gear
            foreach (var item in _profileService.GetEquippedItems(state.Profile))
            {
                foreach (var mod in item.Mods)
                {
                    if (mod.Type == ItemModType.ITEM_MOD_HASTE_RATING ||
                        mod.Type == ItemModType.ITEM_MOD_HASTE_SPELL_RATING)
                    {
                        hasteRating += mod.StatRating;
                    }
                }
            }

            return hasteRating;
        }

        public double GetHasteMultiplier(GameState state)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.Spec).FirstOrDefault();
            return 1 + specData.HasteBase + (GetDrRating(GetHasteRating(state), specData.HasteCost) / specData.HasteCost / 100);
        }

        public double GetVersatilityRating(GameState state)
        {
            var versatilityRating = GetBaseVersatilityMultiplier(state);

            // Add any forced additional stats if specified (stat weights)
            var bonusVersatilty = GetPlaystyle(state, "GrantAdditionalStatVersatility");
            if (bonusVersatilty != null)
                versatilityRating += bonusVersatilty.Value;

            // Get average vers from effects
            foreach (var spell in state.RegisteredSpells.Where(s => s.SpellService != null))
            {
                versatilityRating += spell.SpellService.GetAverageVersatility(state, spell.SpellData);
            }

            return versatilityRating;
        }

        internal double GetBaseVersatilityMultiplier(GameState state)
        {
            var playStyleValue = GetPlaystyle(state, "OverrideStatVersatility").Value;
            if (playStyleValue != 0)
            {
                return playStyleValue;
            }

            var versatilityRating = 0d;

            // Get vers from gear
            foreach (var item in _profileService.GetEquippedItems(state.Profile))
            {
                foreach (var mod in item.Mods)
                {
                    if (mod.Type == ItemModType.ITEM_MOD_VERSATILITY_RATING)
                    {
                        versatilityRating += mod.StatRating;
                    }
                }
            }

            return versatilityRating;
        }

        public double GetVersatilityMultiplier(GameState state)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.Spec).FirstOrDefault();

            return 1 + specData.VersBase + (GetDrRating(GetVersatilityRating(state), specData.VersCost) / specData.VersCost / 100);
        }

        public double GetMasteryRating(GameState state)
        {
            var masteryRating = GetBaseMasteryRating(state);

            // Add any forced additional stats if specified (stat weights)
            var bonusMastery = GetPlaystyle(state, "GrantAdditionalStatMastery");
            if (bonusMastery != null)
                masteryRating += bonusMastery.Value;

            // Get average mastery from effects
            foreach (var spell in state.RegisteredSpells.Where(s => s.SpellService != null))
            {
                masteryRating += spell.SpellService.GetAverageMastery(state, spell.SpellData);
            }

            return masteryRating;
        }

        internal double GetBaseMasteryRating(GameState state)
        {
            var playStyleValue = GetPlaystyle(state, "OverrideStatMastery").Value;
            if (playStyleValue != 0)
            {
                return playStyleValue;
            }

            var masteryRating = 0d;

            // Get mastery from gear
            foreach (var item in _profileService.GetEquippedItems(state.Profile))
            {
                foreach (var mod in item.Mods)
                {
                    if (mod.Type == ItemModType.ITEM_MOD_MASTERY_RATING)
                    {
                        masteryRating += mod.StatRating;
                    }
                }
            }

            return masteryRating;
        }

        public double GetMasteryMultiplier(GameState state)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.Spec).FirstOrDefault();

            return 1 + specData.MasteryBase + (GetDrRating(GetMasteryRating(state), specData.MasteryCost) / specData.MasteryCost / 100);
        }

        public double GetIntellect(GameState state)
        {
            double intellect = GetBaseIntellect(state);

            // Add any forced additional stats if specified (stat weights)
            var bonusIntellect = GetPlaystyle(state, "GrantAdditionalStatIntellect");
            if (bonusIntellect != null)
                intellect += bonusIntellect.Value;

            // Get average int from effects
            foreach (var spell in state.RegisteredSpells.Where(s => s.SpellService != null))
            {
                intellect += spell.SpellService.GetAverageIntellect(state, spell.SpellData);
            }

            // Cloth armor check to apply the armor bonus multiplier
            var clothBonus = GetPlaystyle(state, "ForceClothBonus");
            var clothCount = 0;

            if (clothBonus != null && clothBonus.Value == 1)
            {
                clothCount = 8;
            }
            else
            {
                foreach (var item in _profileService.GetEquippedItems(state.Profile))
                {
                    if (item.Slot != InventorySlot.Cloak &&
                        item.ItemType == ItemType.ITEM_CLASS_ARMOR &&
                        item.ItemSubType == 1)
                    {
                        clothCount++;
                    }
                }
            }

            if (clothCount == 8)
                intellect *= 1.05d;

            // TODO: Test if this is actually Floor'd. It's probably not touched at all.
            // TODO: It does not, need to fix this + tests.
            return Math.Floor(intellect);
        }

        public double GetBaseIntellect(GameState state)
        {
            var playStyleValue = GetPlaystyle(state, "OverrideStatIntellect").Value;
            if (playStyleValue != 0)
            {
                return playStyleValue;
            }

            double intellect = 0;

            // Get base intellect based on class
            intellect += state.Profile.Class switch
            {
                Class.Priest => 450,
                _ => throw new NotImplementedException("This class is not yet implemented."),
            };

            // Apply race modifiers
            switch (state.Profile.Race)
            {
                case Race.Vulpera:
                    intellect += 1;
                    break;

                case Race.BloodElf:
                case Race.Mechagnome:
                case Race.Nightborne:
                case Race.VoidElf:
                    intellect += 2;
                    break;

                case Race.Gnome:
                case Race.Goblin:
                    intellect += 3;
                    break;

                case Race.Orc:
                case Race.Dwarf:
                case Race.DarkIronDwarf:
                case Race.MagharOrc:
                case Race.HighmountainTauren:
                case Race.KulTiran:
                    intellect -= 1;
                    break;

                case Race.Undead:
                case Race.Tauren:
                    intellect -= 2;
                    break;

                case Race.Troll:
                case Race.Worgen:
                case Race.ZandalariTroll:
                    intellect -= 3;
                    break;

                case Race.NoRace:
                case Race.Human:
                case Race.NightElf:
                case Race.Draenei:
                case Race.LightforgedDraenei:
                case Race.Pandaren:
                case Race.PandarenAlliance:
                case Race.PandarenHorde:
                default:
                    break;
            }

            // Add intellect from all items
            foreach (var item in _profileService.GetEquippedItems(state.Profile))
            {
                foreach (var mod in item.Mods)
                {
                    if (mod.Type == ItemModType.ITEM_MOD_INTELLECT ||
                        mod.Type == ItemModType.ITEM_MOD_AGILITY_INTELLECT ||
                        mod.Type == ItemModType.ITEM_MOD_STRENGTH_INTELLECT ||
                        mod.Type == ItemModType.ITEM_MOD_STRENGTH_AGILITY_INTELLECT)
                    {
                        intellect += mod.StatRating;
                    }
                }
            }

            return intellect;
        }

        public BaseSpellData GetSpellData(GameState state, Spell spellId)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.Spec).FirstOrDefault();

            BaseSpellData spell = specData.Spells.Where(s => s.Id == (uint)spellId).FirstOrDefault();

            spell = JsonConvert.DeserializeObject<BaseSpellData>(JsonConvert.SerializeObject(spell));

            return spell;
        }

        public bool IsConduitActive(GameState state, Conduit conduit)
        {
            var soulbind = state.Profile.Covenant.Soulbinds.Where(s => s.IsActive).FirstOrDefault();
            if (soulbind == null)
                return false;

            return soulbind.ActiveConduits.ContainsKey(conduit);
        }

        public uint GetConduitRank(GameState state, Conduit conduit)
        {
            var rank = 0u;

            if (IsConduitActive(state, conduit))
            {
                var soulbind = state.Profile.Covenant.Soulbinds.Where(s => s.IsActive).FirstOrDefault();
                if (soulbind.ActiveConduits.ContainsKey(conduit))
                    return soulbind.ActiveConduits[conduit];
            }

            return rank;
        }

        #region Registered Spells

        public List<RegisteredSpell> GetRegisteredSpells(GameState state)
        {
            return state.RegisteredSpells;
        }

        /// <summary>
        /// Populates the profiles RegisteredSpells ready to be processed
        /// </summary>
        /// <param name="state"></param>
        /// <param name="additionalSpells">Additional spells that should be added (not included in the profile). 
        /// Their SpellService parameter will be populated by this method.</param>
        public void RegisterSpells(GameState state, List<RegisteredSpell> additionalSpells)
        {
            var registeredSpells = new List<RegisteredSpell>(additionalSpells);

            // TODO: Talents

            // TODO: Consumables

            // ITEMS: For each item loop through its effects and find all the associated SpellIds.
            // If it's one we have registered, save it so it can be used later if needed.
            foreach (var item in _profileService.GetEquippedItems(state.Profile))
            {
                foreach (var effect in item.Effects.Where(e => e.Spell != null))
                {
                    registeredSpells.AddRange(GetTriggerSpellRecursive(effect.Spell, item.ItemLevel));
                }
            }

            // TODO: Covenant abilities

            // SOULBINDS: Loop through each trait and add it to the list
            if (state.Profile.Covenant.Soulbinds.Where(s => s.IsActive).FirstOrDefault() != null)
            {
                foreach (var trait in state.Profile.Covenant.Soulbinds
                    .Where(s => s.IsActive).FirstOrDefault().ActiveTraits)
                {
                    if (Enum.IsDefined(typeof(Spell), (int)trait))
                    {
                        var registeredSpell = new RegisteredSpell()
                        {
                            Spell = (Spell)trait,
                        };

                        registeredSpells.Add(registeredSpell);
                    }
                }
            }

            // TODO: Conduits

            
            foreach (var spell in registeredSpells)
            {
                // Populate all the spellservices
                spell.SpellService = _spellServiceFactory.GetSpellService(spell.Spell);

                // And the spelldata
                // TODO: Consider moving these overrides to another location. Could run into race conditions
                // if the order isn't: Modify Spelldata => RegisterSpells as we're applying spelldata here.
                // Ideally though the modelling run does RegisterSpells last right before the run begins?
                spell.SpellData = GetSpellData(state, spell.Spell);

                // Add scalevalues
                if (spell.SpellData != null)
                {
                    foreach(var kvp in spell.ScaleValues)
                    {
                        spell.SpellData.ScaleValues.Add(kvp.Key, kvp.Value);
                    }

                    // Update the spelldata to have the new scalevalues added
                    OverrideSpellData(state, spell.SpellData);

                    // Add the overrides if relevant
                    if (spell.ItemLevel > 0)
                        spell.SpellData.Overrides.Add(Override.ItemLevel, spell.ItemLevel);
                }

            }

            state.RegisteredSpells = registeredSpells;
        }

        internal List<RegisteredSpell> GetTriggerSpellRecursive(BaseSpellData spell, int itemLevel)
        {
            var spells = new List<RegisteredSpell>();

            if (spell != null)
            {
                var newSpell = new RegisteredSpell()
                {
                    Spell = (Spell)spell.Id,
                    ScaleValues= spell.ScaleValues,
                    ItemLevel = itemLevel
                };

                spells.Add(newSpell);

                foreach (var effect in spell.Effects)
                {
                    spells.AddRange(GetTriggerSpellRecursive(effect.TriggerSpell, itemLevel));
                }
            }

            return spells;
        }

        #endregion

        #region Covenant

        public void SetCovenant(GameState state, CovenantProfile covenant)
        {
            _profileService.SetCovenant(state.Profile, covenant);
        }

        public Covenant GetActiveCovenant(GameState state)
        {
            return state.Profile.Covenant.Covenant;
        }

        #endregion

        #region Talents

        public bool IsTalentActive(GameState state, Talent talent)
        {
            var exists = state.Profile.Talents.Contains(talent);

            return exists;
        }

        public void SetActiveTalent(GameState state, Talent talent)
        {
            _profileService.AddTalent(state.Profile, talent);
        }

        #endregion

        public bool IsLegendaryActive(GameState state, Spell legendary)
        {
            foreach (var item in _profileService.GetEquippedItems(state.Profile))
            {
                foreach (var effect in item.Effects)
                {
                    if (effect.Spell != null && effect.Spell.Id == (int)legendary)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void OverrideSpellData(GameState state, BaseSpellData newData)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.Spec).FirstOrDefault();

            var requestedData = specData.Spells.Where(s => s.Id == newData.Id).FirstOrDefault();

            if (requestedData == null)
                return;

            specData.Spells.Remove(requestedData);
            specData.Spells.Add(newData);
        }

        /// <summary>
        /// Get a playstyle entry based on the supplied unique name
        /// </summary>
        /// <param name="state"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public PlaystyleEntry GetPlaystyle(GameState state, string name)
        {
            var playstyleEntry = state.Profile.PlaystyleEntries?
                .Where(p => p.Name.ToLower() == name.ToLower())
                .FirstOrDefault();

            playstyleEntry = JsonConvert.DeserializeObject<PlaystyleEntry>(JsonConvert.SerializeObject(playstyleEntry));

            return playstyleEntry;
        }

        /// <summary>
        /// Delete an existing playstyle entry if present, and replace it with the supplied one
        /// </summary>
        /// <param name="state"></param>
        /// <param name="newPlaystyle"></param>
        public void OverridePlaystyle(GameState state, PlaystyleEntry newPlaystyle)
        {
            var playstyleEntry = state.Profile.PlaystyleEntries?
                .Where(p => p.Name.ToLower() == newPlaystyle.Name.ToLower())
                .FirstOrDefault();

            if (playstyleEntry == null)
                return;

            state.Profile.PlaystyleEntries.Remove(playstyleEntry);
            state.Profile.PlaystyleEntries.Add(newPlaystyle);
        }

        public void JournalEntry(GameState state, string message)
        {
            state.JournalEntries.Add(message);
        }

        public List<string> GetJournal(GameState state, bool removeDuplicates = false)
        {
            if (removeDuplicates)
            {
                return state.JournalEntries.Distinct().ToList();
            }

            return state.JournalEntries;
        }

        public double GetFightLength(GameState state)
        {
            return state.Profile.FightLengthSeconds;
        }

        #region Gamestate Creation

        public GameState CreateValidatedGameState(PlayerProfile profile, GlobalConstants constants = null)
        {
            var validatedProfile = _profileService.ValidateProfile(profile);

            if (constants == null)
                constants = _constantsService.LoadConstantsFromFile();

            return new GameState(validatedProfile, constants);
        }

        public GameState CloneGameState(GameState state)
        {
            var stateString = JsonConvert.SerializeObject(state);

            var newState = JsonConvert.DeserializeObject<GameState>(stateString);

            return newState;
        }

        #endregion

        #region Holy Priest Specific
        // TODO: Move this out to a holy priest specific file at some point.

        public double GetTotalHolyWordCooldownReduction(GameState state, Spell spell, bool isApotheosisActive = false)
        {
            // Only let Apoth actually benefit if apoth is talented
            if (!IsTalentActive(state, Talent.Apotheosis))
                isApotheosisActive = false;

            var serenityCDRBase = GetSpellData(state, Spell.HolyWordSerenity).GetEffect(709474).BaseValue;
            var sancCDRPoH = GetSpellData(state, Spell.HolyWordSanctify).GetEffect(709475).BaseValue;
            var sancCDRRenew = GetSpellData(state, Spell.HolyWordSanctify).GetEffect(709476).BaseValue;
            var bhCDR = GetSpellData(state, Spell.BindingHeal).GetEffect(325998).BaseValue;
            var salvCDRBase = GetSpellData(state, Spell.HolyWordSalvation).GetEffect(709211).BaseValue;
            var haCDRBase = GetSpellData(state, Spell.HarmoniousApparatus).GetEffect(833714).BaseValue;
            var chastiseCDRBase = GetSpellData(state, Spell.HolyWordChastise).GetEffect(709477).BaseValue;

            var isLotnActive = IsTalentActive(state, Talent.LightOfTheNaaru);

            // Holy Oration
            var isHolyOrationActive = IsConduitActive(state, Conduit.HolyOration);
            var holyOrationModifier = 0d;
            if (isHolyOrationActive)
            {
                var holyOrationRank = GetConduitRank(state, Conduit.HolyOration);
                var holyOrationSpellData = GetSpellData(state, Spell.HolyOration);

                holyOrationModifier = holyOrationSpellData.ConduitRanks[holyOrationRank] / 100;
            }

            var returnCDR = 0d;

            // This is a bit more verbose than it needs to be for the sake of clarity
            // See #58 for some of the testing/math behind these values
            switch (spell)
            {
                case Spell.FlashHeal:
                case Spell.Heal:
                    returnCDR = serenityCDRBase;
                    returnCDR *= isLotnActive ? 1d + 1d / 3d : 1d; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4d : 1d; // Apotheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? serenityCDRBase * holyOrationModifier : 0d;
                    break;
                case Spell.PrayerOfHealing:
                    returnCDR = sancCDRPoH;
                    returnCDR *= isLotnActive ? 1d + 1d / 3d : 1d; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4d : 1d; // Apotheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? sancCDRPoH * holyOrationModifier : 0d;
                    break;

                case Spell.BindingHeal:
                    returnCDR = bhCDR; // Binding heal gets half the CDR benefit
                    returnCDR *= isLotnActive ? 1d + 1d / 3d : 1d; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4d : 1d; // Apotheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? bhCDR * holyOrationModifier : 0d;
                    break;

                case Spell.Renew:
                    returnCDR = sancCDRRenew; // Renew gets a third of the CDR benefit
                    returnCDR *= isLotnActive ? 1d + 1d / 3d : 1d; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4d : 1d; // Apotheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? sancCDRRenew * holyOrationModifier : 0d;
                    break;

                case Spell.CircleOfHealing:
                case Spell.PrayerOfMending:
                    returnCDR = haCDRBase;
                    returnCDR *= isLotnActive ? 1d + 1d / 3d : 1d; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4d : 1d; // Apotheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? haCDRBase * holyOrationModifier : 0d;
                    break;

                case Spell.HolyWordSerenity:
                case Spell.HolyWordSanctify:
                    returnCDR = salvCDRBase;
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? salvCDRBase * holyOrationModifier : 0;
                    break;

                case Spell.Smite:
                    returnCDR = chastiseCDRBase;
                    returnCDR *= isLotnActive ? 1d + 1d / 3d : 1d; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4d : 1d; // Apotheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? haCDRBase * holyOrationModifier : 0d;
                    break;

                case Spell.HolyFire:
                    returnCDR = chastiseCDRBase;
                    returnCDR *= isLotnActive ? 1d + 1d / 3d : 1d; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4d : 1d; // Apotheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? haCDRBase * holyOrationModifier : 0d;
                    break;

                default:
                    break;
            }

            return returnCDR;
        }

        #endregion
    }
}
