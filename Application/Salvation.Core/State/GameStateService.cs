using Newtonsoft.Json;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Salvation.Core.State
{
    public class GameStateService : IGameStateService
    {
        public double GetBaseManaAmount(GameState state)
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

        public double GetDrRating(double rating, double cost)
        {
            double result = 0; 
            for (int i = 0;  i < rating; i++) { 
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

        public double GetCriticalStrikeMultiplier(GameState state)
        {
            // TODO: Add other sources of crit increase here
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();
            return 1 + specData.CritBase + (GetDrRating(state.Profile.CritRating, specData.CritCost) / specData.CritCost / 100);
        }

        public double GetHasteMultiplier(GameState state)
        {
            // TODO: Add other sources of haste increase here
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();
            return 1 + specData.HasteBase + (GetDrRating(state.Profile.HasteRating, specData.HasteCost) / specData.HasteCost / 100);
        }

        public double GetVersatilityMultiplier(GameState state)
        {
            // TODO: Add other sources of vers increase here
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            return 1 + specData.VersBase + (GetDrRating(state.Profile.VersatilityRating, specData.VersCost) / specData.VersCost / 100);
        }

        public double GetMasteryMultiplier(GameState state)
        {
            // TODO: Add other sources of mastery increase here
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            return 1 + specData.MasteryBase + (GetDrRating(state.Profile.MasteryRating, specData.MasteryCost) / specData.MasteryCost / 100);
        }

        public double GetIntellect(GameState state)
        {
            // TODO: Add other sources of int increase here
            return state.Profile.Intellect;
        }

        public BaseSpellData GetSpellData(GameState state, Spell spellId)
        {
            var specData = state.Constants.Specs.Where(s => s.SpecId == (int)state.Profile.SpecId).FirstOrDefault();

            BaseSpellData spell = specData.Spells.Where(s => s.Id == (uint)spellId).FirstOrDefault();

            spell = JsonConvert.DeserializeObject<BaseSpellData>(JsonConvert.SerializeObject(spell));

            return spell;
        }

        public bool IsConduitActive(GameState state, Conduit conduit)
        {
            var exists = state.Profile.Conduits.Keys.Contains(conduit);

            return exists;
        }

        public uint GetConduitRank(GameState state, Conduit conduit)
        {
            var rank = 0u;
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

        public GameState CloneGameState(GameState state)
        {
            var stateString = JsonConvert.SerializeObject(state);

            var newState = JsonConvert.DeserializeObject<GameState>(stateString);

            return newState;
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
                    returnCDR *= isApotheosisActive ? 4d : 1d; // Aptheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? serenityCDRBase * holyOrationModifier : 0d;
                    break;
                case Spell.PrayerOfHealing:
                    returnCDR = sancCDRPoH;
                    returnCDR *= isLotnActive ? 1d + 1d / 3d : 1d; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4d : 1d; // Aptheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? sancCDRPoH * holyOrationModifier : 0d;
                    break;

                case Spell.BindingHeal:
                    returnCDR = bhCDR; // Binding heal gets half the CDR benefit
                    returnCDR *= isLotnActive ? 1d + 1d / 3d : 1d; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4d : 1d; // Aptheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? bhCDR * holyOrationModifier : 0d;
                    break;

                case Spell.Renew:
                    returnCDR = sancCDRRenew; // Renew gets a third of the CDR benefit
                    returnCDR *= isLotnActive ? 1d + 1d / 3d : 1d; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4d : 1d; // Aptheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? sancCDRRenew * holyOrationModifier : 0d;
                    break;

                case Spell.CircleOfHealing:
                case Spell.PrayerOfMending:
                    returnCDR = haCDRBase;
                    returnCDR *= isLotnActive ? 1d + 1d / 3d : 1d; // LotN adds 33% more CDR.
                    returnCDR *= isApotheosisActive ? 4d : 1d; // Aptheosis adds 200% more CDR
                    // Apply this last as it's additive overall and not multiplicative with others
                    returnCDR += isHolyOrationActive ? haCDRBase * holyOrationModifier : 0d;
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
