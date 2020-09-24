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
using System.Runtime.InteropServices;
using System.Text;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class HolyWordSalvation : SpellService, IHolyWordSalvationSpellService
    {
        private readonly IHolyWordSerenitySpellService serenitySpellService;
        private readonly IHolyWordSanctifySpellService holyWordSanctifySpellService;
        private readonly IRenewSpellService renewSpellService;
        private readonly IPrayerOfMendingSpellService prayerOfMendingSpellService;

        public HolyWordSalvation(IGameStateService gameStateService,
            IModellingJournal journal,
            IHolyWordSerenitySpellService serenitySpellService,
            IHolyWordSanctifySpellService holyWordSanctifySpellService,
            IRenewSpellService renewSpellService,
            IPrayerOfMendingSpellService prayerOfMendingSpellService)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.HolyWordSalvation;
            this.serenitySpellService = serenitySpellService;
            this.holyWordSanctifySpellService = holyWordSanctifySpellService;
            this.renewSpellService = renewSpellService;
            this.prayerOfMendingSpellService = prayerOfMendingSpellService;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null, Dictionary<string, decimal> moreData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.HolyWordSalvation);

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
                spellData = gameStateService.GetSpellData(gameState, SpellIds.HolyWordSalvation);

            // Salv is (60 + (SerenityCPM + SancCPM) * SalvCDR) / (CastTime + Cooldown) + 1 / (FightLength / 60)
            // Essentially the CDR per minute is 60 + the CDR from holy words.

            // TODO: Add sanc here properly once implemented
            var serenityCPM = serenitySpellService.GetActualCastsPerMinute(gameState);
            var sancCPM = holyWordSanctifySpellService.GetActualCastsPerMinute(gameState);

            var salvCDRBase = gameStateService.GetModifier(gameState, "SalvationHolyWordCDR").Value;

            var hastedCD = GetHastedCooldown(gameState, spellData, moreData);
            var hastedCT = GetHastedCastTime(gameState, spellData, moreData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            decimal salvCDRPerMin = 60m + (serenityCPM + sancCPM) * salvCDRBase;
            decimal maximumPotentialCasts = salvCDRPerMin / (hastedCT + hastedCD)
                + 1m / (fightLength / 60m);

            return maximumPotentialCasts;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.HolyWordSalvation);

            AveragedSpellCastResult result = base.GetCastResults(gameState, spellData, moreData);

            // We need to add a 0-cost renew:
            var renewSpellData = gameStateService.GetSpellData(gameState, SpellIds.Renew);

            renewSpellData.ManaCost = 0;
            renewSpellData.Gcd = 0;
            renewSpellData.BaseCastTime = 0;
            renewSpellData.NumberOfHealingTargets = GetNumberOfHealingTargets(gameState, spellData, moreData);

            // grab the result of the spell cast
            var renewResult = renewSpellService.GetCastResults(gameState, renewSpellData);

            result.AdditionalCasts.Add(renewResult);

            var pomSpellData = gameStateService.GetSpellData(gameState, SpellIds.PrayerOfMending);

            pomSpellData.ManaCost = 0;
            pomSpellData.Gcd = 0;
            pomSpellData.BaseCastTime = 0;
            pomSpellData.Coeff2 = 2; // Number of initial stacks
            pomSpellData.NumberOfHealingTargets = GetNumberOfHealingTargets(gameState, spellData, moreData);

            // grab the result of the spell cast
            var pomResult = prayerOfMendingSpellService.GetCastResults(gameState, pomSpellData);
            result.AdditionalCasts.Add(pomResult);

            return result;
        }
    }
}