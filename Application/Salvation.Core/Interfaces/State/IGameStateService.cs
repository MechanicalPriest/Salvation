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
        PlaystyleEntry GetPlaystyle(GameState state, string name);
        void SetCovenant(GameState state, CovenantProfile covenant);
        Covenant GetActiveCovenant(GameState state);
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
        double GetVersatilityMultiplier(GameState state);
        double GetCriticalStrikeMultiplier(GameState state);
        double GetMasteryMultiplier(GameState state);
        double GetHasteMultiplier(GameState state);
        double GetBaseManaAmount(GameState state);
        double GetGCDFloor(GameState gameState);

        // Player Profile Configuration

        void SetProfileName(GameState state, string profileName);
        bool IsConduitActive(GameState state, Conduit conduit);
        uint GetConduitRank(GameState state, Conduit conduit);
        bool IsLegendaryActive(GameState state, Spell legendary);
        void SetActiveTalent(GameState state, Talent talent);
        bool IsTalentActive(GameState state, Talent talent); 
        void RegisterSpells(GameState state);

        // Utility
        GameState CloneGameState(GameState state);
        GameState CreateValidatedGameState(PlayerProfile profile, GlobalConstants constants = null);
        public List<string> GetJournal(GameState state, bool removeDuplicates = false);
        public void JournalEntry(GameState state, string message);

        // Holy Priest specific
        double GetTotalHolyWordCooldownReduction(GameState state, Spell spell, bool IsApotheosisActive = false);
    }
}
