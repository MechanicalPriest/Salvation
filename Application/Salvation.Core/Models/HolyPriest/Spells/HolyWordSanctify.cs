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
    public class HolyWordSanctify : SpellService, IHolyWordSanctifySpellService
    {
        private readonly IPrayerOfHealingSpellService prayerOfHealingSpellService;
        private readonly IRenewSpellService renewSpellService;
        private readonly IBindingHealSpellService bindingHealSpellService;

        public HolyWordSanctify(IGameStateService gameStateService,
            IModellingJournal journal, 
            IPrayerOfHealingSpellService prayerOfHealingSpellService,
            IRenewSpellService renewSpellService,
            IBindingHealSpellService bindingHealSpellService)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.HolyWordSanctify;
            this.prayerOfHealingSpellService = prayerOfHealingSpellService;
            this.renewSpellService = renewSpellService;
            this.bindingHealSpellService = bindingHealSpellService;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.HolyWordSanctify);

            var holyPriestAuraHealingBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;
            
            decimal averageHeal = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * spellData.NumberOfHealingTargets;
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.HolyWordSanctify);

            // Max casts per minute is (60 + (FH + Heal + BH * 0.5) * HwCDR) / CD + 1 / (FightLength / 60)
            // HWCDR is 6 base, more with LOTN/other effects
            // 1 from regular CD + reductions from fillers divided by the cooldown to get base CPM
            // Then add the one charge we start with, 1 per fight, into seconds.

            // TODO: Update these to point to their spells when implemented
            var pohCPM = prayerOfHealingSpellService.GetActualCastsPerMinute(gameState);
            var renewCPM = renewSpellService.GetActualCastsPerMinute(gameState);
            var bhCPM = bindingHealSpellService.GetActualCastsPerMinute(gameState);

            var hastedCD = GetHastedCooldown(gameState, spellData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            // TODO: Add other HW CDR increasing effects.
            var hwCDRBase = gameStateService.GetModifier(gameState, "HolyWordsBaseCDR").Value;

            decimal hwCDR = (pohCPM + bhCPM * 0.5m + renewCPM * 1m / 3m) * hwCDRBase;

            decimal maximumPotentialCasts = (60m + hwCDR) / hastedCD
                + 1m / (fightLength / 60m);

            return maximumPotentialCasts;
        }
    }
}
