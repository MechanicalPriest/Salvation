using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class Renew : SpellService, IRenewSpellService
    {
        public Renew(IGameStateService gameStateService,
            IModellingJournal journal)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.Renew;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.Renew);

            var holyPriestAuraHealingBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            // Renews's average heal is initial + HoT portion:
            decimal averageHealFirstTick = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            journal.Entry($"[{spellData.Name}] Tooltip: {averageHealFirstTick:0.##} (first)");

            averageHealFirstTick *= gameStateService.GetCriticalStrikeMultiplier(gameState)
                * gameStateService.GetHasteMultiplier(gameState);


            // HoT is affected by haste
            decimal averageHealTicks = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * gameStateService.GetHasteMultiplier(gameState)
                * holyPriestAuraHealingBonus
                * 5;

            journal.Entry($"[{spellData.Name}] Tooltip: {averageHealTicks:0.##} (ticks)");

            averageHealTicks *= gameStateService.GetCriticalStrikeMultiplier(gameState);

            return (averageHealFirstTick + averageHealTicks) * GetNumberOfHealingTargets(gameState, spellData, moreData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.Renew);

            var hastedCastTime = GetHastedCastTime(gameState, spellData, moreData);
            var hastedGcd = GetHastedGcd(gameState, spellData, moreData);
            var hastedCd = GetHastedCooldown(gameState, spellData, moreData);

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
