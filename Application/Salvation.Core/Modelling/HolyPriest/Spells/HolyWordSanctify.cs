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
        private readonly ICircleOfHealingSpellService _circleOfHealingSpellService;

        public HolyWordSanctify(IGameStateService gameStateService,
            IModellingJournal journal,
            IPrayerOfHealingSpellService prayerOfHealingSpellService,
            IRenewSpellService renewSpellService,
            IBindingHealSpellService bindingHealSpellService,
            ICircleOfHealingSpellService circleOfHealingSpellService)
            : base(gameStateService, journal)
        {
            SpellId = (int)Spell.HolyWordSanctify;
            _prayerOfHealingSpellService = prayerOfHealingSpellService;
            _renewSpellService = renewSpellService;
            _bindingHealSpellService = bindingHealSpellService;
            _circleOfHealingSpellService = circleOfHealingSpellService;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.HolyWordSanctify);

            var holyPriestAuraHealingBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            decimal averageHeal = spellData.Coeff1
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * (decimal)GetNumberOfHealingTargets(gameState, spellData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.HolyWordSanctify);

            // Max casts per minute is (60 + (FH + Heal + BH * 0.5) * HwCDR) / CD + 1 / (FightLength / 60)
            // HWCDR is 6 base, more with LOTN/other effects
            // 1 from regular CD + reductions from fillers divided by the cooldown to get base CPM
            // Then add the one charge we start with, 1 per fight, into seconds.

            // TODO: Update these to point to their spells when implemented
            var cpmPoH = _prayerOfHealingSpellService.GetActualCastsPerMinute(gameState);
            var cpmRenew = _renewSpellService.GetActualCastsPerMinute(gameState);
            var cpmBindingHeal = _bindingHealSpellService.GetActualCastsPerMinute(gameState);

            var hastedCD = GetHastedCooldown(gameState, spellData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            var hwCDRPoH = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.PrayerOfHealing);
            var hwCDRBindingHeal = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.BindingHeal);
            var hwCDRRenew = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.Renew);

            decimal hwCDR = cpmPoH * hwCDRPoH +
                cpmBindingHeal * hwCDRBindingHeal +
                cpmRenew * hwCDRRenew;

            if(_gameStateService.IsLegendaryActive(gameState, Spell.HarmoniousApparatus))
            {
                var cpmCoH = _circleOfHealingSpellService.GetActualCastsPerMinute(gameState);
                var hwCDRCoH = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.CircleOfHealing);
                hwCDR += cpmCoH * hwCDRCoH;
            }

            decimal maximumPotentialCasts = (60m + hwCDR) / hastedCD
                + 1m / (fightLength / 60m);

            return maximumPotentialCasts;
        }
    }
}
