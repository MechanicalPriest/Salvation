﻿using Salvation.Core.Constants;
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
    public class HolyWordSerenity : SpellService, IHolyWordSerenitySpellService
    {
        private readonly IFlashHealSpellService flashHealSpellService;
        private readonly IHealSpellService healSpellService;
        private readonly IBindingHealSpellService bindingHealSpellService;

        public HolyWordSerenity(IGameStateService gameStateService,
            IModellingJournal journal, 
            IFlashHealSpellService flashHealSpellService,
            IHealSpellService healSpellService,
            IBindingHealSpellService bindingHealSpellService)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.HolyWordSerenity;
            this.flashHealSpellService = flashHealSpellService;
            this.healSpellService = healSpellService;
            this.bindingHealSpellService = bindingHealSpellService;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.HolyWordSerenity);

            var holyPriestAuraHealingBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;
            
            decimal averageHeal = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData, moreData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.HolyWordSerenity);

            // Max casts per minute is (60 + (FH + Heal + BH * 0.5) * HwCDR) / CD + 1 / (FightLength / 60)
            // HWCDR is 6 base, more with LOTN/other effects
            // 1 from regular CD + reductions from fillers divided by the cooldown to get base CPM
            // Then add the one charge we start with, 1 per fight, into seconds.

            // TODO: Update these to point to their spells when implemented
            var fhCPM = flashHealSpellService.GetActualCastsPerMinute(gameState);
            var healCPM = healSpellService.GetActualCastsPerMinute(gameState);
            var bhCPM = bindingHealSpellService.GetActualCastsPerMinute(gameState);

            var hastedCD = GetHastedCooldown(gameState, spellData, moreData);
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