using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class HolyWordSanctify : SpellService, IHolyWordSanctifySpellService
    {
        private readonly IPrayerOfHealingSpellService _prayerOfHealingSpellService;
        private readonly IRenewSpellService _renewSpellService;
        private readonly IBindingHealSpellService _bindingHealSpellService;

        public HolyWordSanctify(IGameStateService gameStateService,
            IModellingJournal journal,
            IPrayerOfHealingSpellService prayerOfHealingSpellService,
            IRenewSpellService renewSpellService,
            IBindingHealSpellService bindingHealSpellService)
            : base(gameStateService, journal)
        {
            SpellId = (int)SpellIds.HolyWordSanctify;
            _prayerOfHealingSpellService = prayerOfHealingSpellService;
            _renewSpellService = renewSpellService;
            _bindingHealSpellService = bindingHealSpellService;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, SpellIds.HolyWordSanctify);

            var holyPriestAuraHealingBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            decimal averageHeal = spellData.Coeff1
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData, moreData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, SpellIds.HolyWordSanctify);

            // Max casts per minute is (60 + (FH + Heal + BH * 0.5) * HwCDR) / CD + 1 / (FightLength / 60)
            // HWCDR is 6 base, more with LOTN/other effects
            // 1 from regular CD + reductions from fillers divided by the cooldown to get base CPM
            // Then add the one charge we start with, 1 per fight, into seconds.

            // TODO: Update these to point to their spells when implemented
            var pohCPM = _prayerOfHealingSpellService.GetActualCastsPerMinute(gameState);
            var renewCPM = _renewSpellService.GetActualCastsPerMinute(gameState);
            var bhCPM = _bindingHealSpellService.GetActualCastsPerMinute(gameState);

            var hastedCD = GetHastedCooldown(gameState, spellData, moreData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            // TODO: Add other HW CDR increasing effects.
            var hwCDRBase = _gameStateService.GetModifier(gameState, "HolyWordsBaseCDR").Value;

            decimal hwCDR = (pohCPM + bhCPM * 0.5m + renewCPM * 1m / 3m) * hwCDRBase;

            decimal maximumPotentialCasts = (60m + hwCDR) / hastedCD
                + 1m / (fightLength / 60m);

            return maximumPotentialCasts;
        }
    }
}
