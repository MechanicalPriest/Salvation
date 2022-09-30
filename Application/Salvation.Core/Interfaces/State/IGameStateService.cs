using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System.Collections.Generic;

namespace Salvation.Core.Interfaces.State
{
    /// <summary>
    /// Goal of this is to provide a way to calculate the required fields using the player profile,
    /// constants and other state specific variables
    /// </summary>
    public interface IGameStateService
    {
        // Global constants
        BaseSpellData GetSpellData(GameState state, Spell spellId);
        CastProfile GetSpellCastProfile(GameState state, int spellId);
        void SetSpellCastProfile(GameState state, CastProfile castProfile);
        /// <summary>
        /// Playstyle entry is a value that modifies a component of playstyle, from overheal to proc rate, number of targets etc.
        /// Stored as a (string, value) KVP dictionary.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        PlaystyleEntry GetPlaystyle(GameState state, string name);
        void OverrideSpellData(GameState state, BaseSpellData newData);
        void OverridePlaystyle(GameState state, PlaystyleEntry newPlaystyle);
        /// <summary>
        /// Gets the fight length in seconds
        /// </summary>
        double GetFightLength(GameState state);

        // Player Stats
        double GetCriticalStrikeRating(GameState state);
        double GetHasteRating(GameState state);
        double GetMasteryRating(GameState state);
        double GetVersatilityRating(GameState state);
        double GetIntellect(GameState state);
        double GetLeechRating(GameState state);
        double GetVersatilityMultiplier(GameState state, Spell spell = Spell.None);
        double GetCriticalStrikeMultiplier(GameState state, Spell spell = Spell.None);
        double GetMasteryMultiplier(GameState state, Spell spell = Spell.None);
        double GetHasteMultiplier(GameState state, Spell spell = Spell.None);
        double GetLeechMultiplier(GameState state, Spell spell = Spell.None);
        double GetGlobalHealingMultiplier(GameState state);
        double GetBaseManaAmount(GameState state);
        double GetGCDFloor(GameState gameState);

        // Player Profile Configuration

        void SetProfileName(GameState state, string profileName);
        bool IsLegendaryActive(GameState state, Spell legendary);
        Talent SetTalentRank(GameState state, Spell spell, int rank);
        Talent GetTalent(GameState state, Spell spell);
        List<RegisteredSpell> GetRegisteredSpells(GameState state);
        void RegisterSpells(GameState state, List<RegisteredSpell> additionalSpells);

        // Utility
        GameState CloneGameState(GameState state);
        GameState CreateValidatedGameState(PlayerProfile profile, GlobalConstants constants = null);
        public List<string> GetJournal(GameState state, bool removeDuplicates = false);
        public void JournalEntry(GameState state, string message, int historyToCheck = 5);

        // Holy Priest specific
        double GetTotalHolyWordCooldownReduction(GameState state, Spell spell, bool IsApotheosisActive = false);
        double GetStamina(GameState state);
        double GetHitpoints(GameState state);
    }
}
