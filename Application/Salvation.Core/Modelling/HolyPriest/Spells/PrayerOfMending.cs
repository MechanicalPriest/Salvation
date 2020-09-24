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
    public class PrayerOfMending : SpellService, IPrayerOfMendingSpellService
    {
        public PrayerOfMending(IGameStateService gameStateService,
            IModellingJournal journal)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.PrayerOfMending;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.PrayerOfMending);

            var holyPriestAuraHealingBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            decimal averageHeal = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= gameStateService.GetCriticalStrikeMultiplier(gameState)
                * spellData.Coeff2; // Coeff2 is number of initial stacks

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData, moreData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.PrayerOfMending);

            var hastedCastTime = GetHastedCastTime(gameState, spellData, moreData);
            var hastedGcd = GetHastedGcd(gameState, spellData, moreData);
            var hastedCd = GetHastedCooldown(gameState, spellData, moreData);

            // A fix to the spell being modified to have no cast time and no gcd and no CD
            // This can happen if it's a component in another spell
            if (hastedCastTime == 0 && hastedGcd == 0 && hastedCd == 0)
                return 0;

            decimal maximumPotentialCasts = 60m / (hastedCastTime + hastedCd);

            return maximumPotentialCasts;
        }
    }
}
