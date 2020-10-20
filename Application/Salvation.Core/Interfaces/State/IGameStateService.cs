using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Profile;
using Salvation.Core.State;

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
        BaseModifier GetModifier(GameState state, string modifierName);
        CastProfile GetCastProfile(GameState state, int spellId);
        ConduitData GetConduitData(GameState state, Conduit conduitId);
        Covenant GetActiveCovenant(GameState state);
        void OverrideSpellData(GameState state, BaseSpellData newData);
        void OverrideModifier(GameState state, BaseModifier newModifier);

        // Player Stats
        double GetIntellect(GameState state);
        double GetVersatilityMultiplier(GameState state);
        double GetCriticalStrikeMultiplier(GameState state);
        double GetMasteryMultiplier(GameState state);
        double GetHasteMultiplier(GameState state);
        double GetBaseManaAmount(GameState state);

        // Player Configuration
        bool IsConduitActive(GameState state, Conduit conduit);
        int GetConduitRank(GameState state, Conduit conduit);
        bool IsLegendaryActive(GameState state, Spell legendary);

        // Utility
        GameState CloneGameState(GameState state);

        // Holy Priest specific
        double GetTotalHolyWordCooldownReduction(GameState state, Spell spell, bool IsApotheosisActive = false);
    }
}
