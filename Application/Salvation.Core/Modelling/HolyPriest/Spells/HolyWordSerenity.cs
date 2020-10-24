using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class HolyWordSerenity : SpellService, IHolyWordSerenitySpellService
    {
        private readonly IFlashHealSpellService _flashHealSpellService;
        private readonly IHealSpellService _healSpellService;
        private readonly IBindingHealSpellService _bindingHealSpellService;
        private readonly IPrayerOfMendingSpellService _prayerOfMendingSpellService;

        public HolyWordSerenity(IGameStateService gameStateService,
            IFlashHealSpellService flashHealSpellService,
            IHealSpellService healSpellService,
            IBindingHealSpellService bindingHealSpellService,
            IPrayerOfMendingSpellService prayerOfMendingSpellService)
            : base(gameStateService)
        {
            SpellId = (int)Spell.HolyWordSerenity;
            _flashHealSpellService = flashHealSpellService;
            _healSpellService = healSpellService;
            _bindingHealSpellService = bindingHealSpellService;
            _prayerOfMendingSpellService = prayerOfMendingSpellService;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.HolyWordSerenity);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            var healingSp = spellData.GetEffect(611).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.HolyWordSerenity);

            // Max casts per minute is (60 + (FH + Heal + BH * 0.5) * HwCDR) / CD + 1 / (FightLength / 60)
            // HWCDR is 6 base, more with LOTN/other effects
            // 1 from regular CD + reductions from fillers divided by the cooldown to get base CPM
            // Then add the one charge we start with, 1 per fight, into seconds.

            var cpmFlashHeal = _flashHealSpellService.GetActualCastsPerMinute(gameState);
            var cpmHeal = _healSpellService.GetActualCastsPerMinute(gameState);
            var cpmBindingHeal = _bindingHealSpellService.GetActualCastsPerMinute(gameState);

            var hastedCD = GetHastedCooldown(gameState, spellData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            var hwCDRFlashHeal = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.FlashHeal);
            var hwCDRHeal = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.Heal);
            var hwCDRBindingHeal = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.BindingHeal);

            double hwCDR = cpmFlashHeal * hwCDRFlashHeal +
                cpmHeal * hwCDRHeal +
                cpmBindingHeal * hwCDRBindingHeal;

            if (_gameStateService.IsLegendaryActive(gameState, Spell.HarmoniousApparatus))
            {
                var cpmPoM = _prayerOfMendingSpellService.GetActualCastsPerMinute(gameState);
                var hwCDRPoM = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.PrayerOfMending);
                hwCDR += cpmPoM * hwCDRPoM;
            }

            double maximumPotentialCasts = (60d + hwCDR) / hastedCD
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetHastedCooldown(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (Spell)SpellId);

            // Cooldown for Serenity is stored in the chargecooldown instead as it has charges
            var cooldown = spellData.ChargeCooldown / 1000;

            return spellData.IsCooldownHasted
                ? cooldown / _gameStateService.GetHasteMultiplier(gameState)
                : cooldown;
        }
    }
}
