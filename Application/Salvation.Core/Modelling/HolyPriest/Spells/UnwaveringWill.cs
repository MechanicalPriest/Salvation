using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    internal static class UnwaveringWill
    {
        internal static double GetUnwaveringWillMultiplier(GameState gameState, ISpellService spellService)
        {
            var multi = 1.0d;

            var talent = spellService.GameStateService.GetTalent(gameState, Spell.UnwaveringWill);

            if (talent != null && talent.Rank > 0)
            {
                // Figure out what percentage of buffs the calling spell gets
                var unwaveringWillUptime = spellService.GameStateService.GetPlaystyle(gameState, "UnwaveringWillUptime");

                if (unwaveringWillUptime == null)
                    throw new ArgumentOutOfRangeException("UnwaveringWillUptime", $"UnwaveringWillUptime needs to be set.");

                var talentSpellData = spellService.GameStateService.GetSpellData(gameState, Spell.UnwaveringWill);

                // Divine this by actual casts to get the average multiplier per cast
                var castTimeReduction = talentSpellData.GetEffect(998678).BaseValue / 100 * talent.Rank * -1;
               
                multi -= castTimeReduction * unwaveringWillUptime.Value;
            }

            return multi;
        }

        internal static double GetUnwaveringWillMultiplier(this FlashHeal spellService, GameState gameState)
        {
            return GetUnwaveringWillMultiplier(gameState, spellService);
        }

        internal static double GetUnwaveringWillMultiplier(this Heal spellService, GameState gameState)
        {
            return GetUnwaveringWillMultiplier(gameState, spellService);
        }

        internal static double GetUnwaveringWillMultiplier(this PrayerOfHealing spellService, GameState gameState)
        {
            return GetUnwaveringWillMultiplier(gameState, spellService);
        }

        internal static double GetUnwaveringWillMultiplier(this Smite spellService, GameState gameState)
        {
            return GetUnwaveringWillMultiplier(gameState, spellService);
        }
    }
}
