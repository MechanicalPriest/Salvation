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

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.HolyWordSanctify);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue;

            var healingSp = spellData.GetEffect(24949).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
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

            double hwCDR = cpmPoH * hwCDRPoH +
                cpmBindingHeal * hwCDRBindingHeal +
                cpmRenew * hwCDRRenew;

            if(_gameStateService.IsLegendaryActive(gameState, Spell.HarmoniousApparatus))
            {
                var cpmCoH = _circleOfHealingSpellService.GetActualCastsPerMinute(gameState);
                var hwCDRCoH = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.CircleOfHealing);
                hwCDR += cpmCoH * hwCDRCoH;
            }

            double maximumPotentialCasts = (60d + hwCDR) / hastedCD
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            // CoH stores its max number of targets in 288932.BaseValue
            var numTargets = spellData.GetEffect(288932).BaseValue;

            return numTargets;
        }
    }
}
