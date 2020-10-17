using Newtonsoft.Json;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.Profile;
using System;
using System.Linq;

namespace Salvation.Core.State
{
    public class GameStateService : IGameStateService
    {
        public decimal GetBaseManaAmount(GameState state)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            return specData.ManaBase;
        }

        public CastProfile GetCastProfile(GameState state, int spellId)
        {
            var castProfile = state.Profile.Casts?
                .Where(c => c.SpellId == spellId)
                .FirstOrDefault();

            castProfile = JsonConvert.DeserializeObject<CastProfile>(JsonConvert.SerializeObject(castProfile));

            return castProfile;
        }

        public decimal GetCriticalStrikeMultiplier(GameState state)
        {
            // TODO: Add other sources of crit increase here
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            return 1 + specData.CritBase + (state.Profile.CritRating / specData.CritCost / 100);
        }

        public decimal GetHasteMultiplier(GameState state)
        {
            // TODO: Add other sources of haste increase here
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            return 1 + specData.HasteBase + (state.Profile.HasteRating / specData.HasteCost / 100);
        }

        public decimal GetVersatilityMultiplier(GameState state)
        {
            // TODO: Add other sources of vers increase here
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            return 1 + specData.VersBase + (state.Profile.VersatilityRating / specData.VersCost / 100);
        }

        public decimal GetMasteryMultiplier(GameState state)
        {
            // TODO: Add other sources of mastery increase here
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            return 1 + specData.MasteryBase + (state.Profile.MasteryRating / specData.MasteryCost / 100);
        }

        public decimal GetIntellect(GameState state)
        {
            // TODO: Add other sources of int increase here
            return state.Profile.Intellect;
        }

        public BaseModifier GetModifier(GameState state, string modifierName)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            var modifier = specData.Modifiers.Where(s => s.Name == modifierName).FirstOrDefault();

            modifier = JsonConvert.DeserializeObject<BaseModifier>(JsonConvert.SerializeObject(modifier));

            return modifier;
        }

        public BaseSpellData GetSpellData(GameState state, Spell spellId)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            BaseSpellData spell = specData.Spells.Where(s => s.Id == (uint)spellId).FirstOrDefault();

            spell = JsonConvert.DeserializeObject<BaseSpellData>(JsonConvert.SerializeObject(spell));

            return spell;
        }

        public ConduitData GetConduitData(GameState state, Conduit conduitId)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            ConduitData conduit = specData.Conduits
                .Where(c => c.Id == (int)conduitId).FirstOrDefault();

            conduit = JsonConvert.DeserializeObject<ConduitData>(
                JsonConvert.SerializeObject(conduit));

            return conduit;
        }

        public bool IsConduitActive(GameState state, Conduit conduit)
        {
            var exists = state.Profile.Conduits.Keys.Contains(conduit);

            return exists;
        }

        public int GetConduitRank(GameState state, Conduit conduit)
        {
            var rank = 0;
            if (state.Profile.Conduits.ContainsKey(conduit))
                rank = state.Profile.Conduits[conduit];

            return rank;
        }

        public Covenant GetActiveCovenant(GameState state)
        {
            return state.Profile.Covenant;
        }

        public bool IsTalentActive(GameState state, Talent talent)
        {
            var exists = state.Profile.Talents.Contains(talent);

            return exists;
        }

        public bool IsLegendaryActive(GameState state, Spell legendary)
        {
            var exists = state.Profile.Legendaries.Contains(legendary);

            return exists;
        }

        public void OverrideSpellData(GameState state, BaseSpellData newData)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            var requestedData = specData.Spells.Where(s => s.Id == newData.Id).FirstOrDefault();

            if (requestedData == null)
                return;

            specData.Spells.Remove(requestedData);
            specData.Spells.Add(newData);
        }

        public void OverrideModifier(GameState state, BaseModifier newModifier)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            var requestedModifier = specData.Modifiers.Where(s => s.Name == newModifier.Name).FirstOrDefault();

            if (requestedModifier == null)
                return;

            specData.Modifiers.Remove(requestedModifier);
            specData.Modifiers.Add(newModifier);
        }

        public GameState CloneGameState(GameState state)
        {
            var stateString = JsonConvert.SerializeObject(state);

            var newState = JsonConvert.DeserializeObject<GameState>(stateString);

            return newState;
        }

        #region Holy Priest Specific
        // TODO: Move this out to a holy priest specific file at some point.

        public decimal GetTotalHolyWordCooldownReduction(GameState state, Spell spell, bool isApotheosisActive = false)
        {
            // Only let Apoth actually benefit if apoth is talented
            if (!IsTalentActive(state, Talent.Apotheosis))
                isApotheosisActive = false;

            var hwCDRBase = GetModifier(state, "HolyWordsBaseCDR").Value;
            var salvCDRBase = GetModifier(state, "SalvationHolyWordCDR").Value;
            var haCDRBase = GetSpellData(state, Spell.HarmoniousApparatus).Coeff1;
            var isLotnActive = IsTalentActive(state, Talent.LightOfTheNaaru);

            // Holy Oration
            var isHolyOrationActive = IsConduitActive(state, Conduit.HolyOration);
            var holyOrationModifier = 0m;
            if (isHolyOrationActive)
            {
                var holyOrationRank = GetConduitRank(state, Conduit.HolyOration);
                var holyOrationSpellData = GetConduitData(state, Conduit.HolyOration);

                holyOrationModifier = holyOrationSpellData.Ranks[holyOrationRank] / 100;
            }

            var returnCDR = 0m;

            // This is a bit more verbose than it needs to be for the sake of clarity
            // See #58 for some of the testing/math behind these values
            switch (spell)
            {
                case Spell.FlashHeal:
                case Spell.Heal:
                case Spell.PrayerOfHealing:
                    returnCDR = hwCDRBase;
                    returnCDR *= isLotnActive ? 1m + 1m / 3m : 1m; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4m : 1m; // Aptheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? hwCDRBase * holyOrationModifier : 0m;
                    break;

                case Spell.BindingHeal:
                    returnCDR = hwCDRBase / 2m; // Binding heal gets half the CDR benefit
                    returnCDR *= isLotnActive ? 1m + 1m / 3m : 1m; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4m : 1m; // Aptheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? (hwCDRBase / 2m) * holyOrationModifier : 0m;
                    break;

                case Spell.Renew:
                    returnCDR = hwCDRBase / 3m; // Renew gets a third of the CDR benefit
                    returnCDR *= isLotnActive ? 1m + 1m / 3m : 1m; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4m : 1m; // Aptheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? (hwCDRBase / 3m) * holyOrationModifier : 0m;
                    break;

                case Spell.CircleOfHealing:
                case Spell.PrayerOfMending:
                    returnCDR = haCDRBase;
                    returnCDR *= isLotnActive ? 1m + 1m / 3m : 1m; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4m : 1m; // Aptheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? haCDRBase * holyOrationModifier : 0m;
                    break;

                case Spell.HolyWordSerenity:
                case Spell.HolyWordSanctify:
                    returnCDR = salvCDRBase;
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? salvCDRBase * holyOrationModifier : 0;
                    break;

                default:
                    break;
            }

            return returnCDR;
        }

        #endregion
    }
}
