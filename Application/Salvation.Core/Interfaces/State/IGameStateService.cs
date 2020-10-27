using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Profile;
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
        CastProfile GetCastProfile(GameState state, int spellId);
        PlaystyleEntry GetPlaystyle(GameState state, string name);
        Covenant GetActiveCovenant(GameState state);
        void OverrideSpellData(GameState state, BaseSpellData newData);
        void OverridePlaystyle(GameState state, PlaystyleEntry newPlaystyle);

        // Player Stats
        double GetIntellect(GameState state);
        double GetVersatilityMultiplier(GameState state);
        double GetCriticalStrikeMultiplier(GameState state);
        double GetMasteryMultiplier(GameState state);
        double GetHasteMultiplier(GameState state);
        double GetBaseManaAmount(GameState state);

        // Player Configuration
        bool IsConduitActive(GameState state, Conduit conduit);
        uint GetConduitRank(GameState state, Conduit conduit);
        bool IsLegendaryActive(GameState state, Spell legendary);

        // Utility
        GameState CloneGameState(GameState state);
        public List<string> GetJournal(GameState state, bool removeDuplicates = false);
        public void JournalEntry(GameState state, string message);

        // Holy Priest specific
        double GetTotalHolyWordCooldownReduction(GameState state, Spell spell, bool IsApotheosisActive = false);
        double GetCriticalStrikeRating(GameState state);
        double GetHasteRating(GameState state);
        double GetMasteryRating(GameState state);
        double GetVersatilityRating(GameState state);
    }
}
