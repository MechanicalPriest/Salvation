using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Models.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest.Spells
{
    public class Renew : SpellService, IRenewSpellService
    {
        public Renew(IGameStateService gameStateService)
            : base (gameStateService)
        {
            SpellId = (int)SpellIds.Renew;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.Renew);

            var holyPriestAuraHealingBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            // Renews's average heal is initial + HoT portion:
            decimal averageHealFirstTick = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            averageHealFirstTick *= gameStateService.GetCriticalStrikeMultiplier(gameState)
                * gameStateService.GetHasteMultiplier(gameState);


            // HoT is affected by haste
            decimal averageHealTicks = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus
                * 5;

            averageHealTicks *= gameStateService.GetCriticalStrikeMultiplier(gameState)
                * gameStateService.GetHasteMultiplier(gameState);

            return (averageHealFirstTick + averageHealTicks) * spellData.NumberOfHealingTargets;
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);

            // A fix to the spell being modified to have no cast time and no gcd and no CD
            // This can happen if it's a component in another spell
            if (hastedCastTime == 0 && hastedGcd == 0 && hastedCd == 0)
                return 0;

            decimal fillerCastTime = hastedCastTime == 0
                ? hastedGcd
                : hastedCastTime;

            decimal maximumPotentialCasts = 60m / fillerCastTime;

            return maximumPotentialCasts;
        }
    }
}
