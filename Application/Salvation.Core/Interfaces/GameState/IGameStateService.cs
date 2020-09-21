using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Interfaces.Profile
{
    /// <summary>
    /// Goal of this is to provide a way to calculate the required fields using the player profile,
    /// constants and other state specific variables
    /// </summary>
    public interface IGameStateService
    {
        // Global constants
        BaseSpellData GetSpellData(GameState state, SpellIds spellId);
        BaseModifier GetModifier(GameState state, string modifierName);


        // Player Stats
        public decimal GetIntellect(GameState state);
        public decimal GetVersatilityMultiplier(GameState state);
        public decimal GetCriticalStrikeMultiplier(GameState state);
    }
}
