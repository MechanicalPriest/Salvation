using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
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
    public class HolyWordSerenity : SpellService, IHolyWordSerenitySpellService
    {
        private readonly IFlashHealSpellService flashHealSpellService;

        public HolyWordSerenity(IGameStateService gameStateService,
            IModellingJournal journal, 
            IFlashHealSpellService flashHealSpellService)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.HolyWordSerenity;
            this.flashHealSpellService = flashHealSpellService;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.HolyWordSerenity);

            var holyPriestAuraHealingBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;
            
            decimal averageHeal = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            journal.Entry($"[{spellData.Name}] Testable: {averageHeal:0.##}");

            averageHeal *= gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * spellData.NumberOfHealingTargets;
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.HolyWordSerenity);

            // Max casts per minute is (60 + (FH + Heal + BH * 0.5) * HwCDR) / CD + 1 / (FightLength / 60)
            // HWCDR is 6 base, more with LOTN/other effects
            // 1 from regular CD + reductions from fillers divided by the cooldown to get base CPM
            // Then add the one charge we start with, 1 per fight, into seconds.

            // TODO: Update these to point to their spells when implemented
            var fhCPM = flashHealSpellService.GetActualCastsPerMinute(gameState);
            var healCPM = flashHealSpellService.GetActualCastsPerMinute(gameState);
            var bhCPM = flashHealSpellService.GetActualCastsPerMinute(gameState);

            var hastedCD = GetHastedCooldown(gameState, spellData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            // TODO: Add other HW CDR increasing effects.
            var hwCDRBase = gameStateService.GetModifier(gameState, "HolyWordsBaseCDR").Value;

            decimal hwCDR = (fhCPM + healCPM + bhCPM * 0.5m) * hwCDRBase;

            decimal maximumPotentialCasts = (60m + hwCDR) / hastedCD
                + 1m / (fightLength / 60m);

            return maximumPotentialCasts;
        }
    }
}
