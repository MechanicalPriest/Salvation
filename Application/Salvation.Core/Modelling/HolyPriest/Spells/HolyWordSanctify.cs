using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class HolyWordSanctify : SpellService, ISpellService<IHolyWordSanctifySpellService>
    {
        private readonly ISpellService<IPrayerOfHealingSpellService> _prayerOfHealingSpellService;
        private readonly ISpellService<IRenewSpellService> _renewSpellService;
        private readonly ISpellService<ICircleOfHealingSpellService> _circleOfHealingSpellService;

        public HolyWordSanctify(IGameStateService gameStateService,
            ISpellService<IPrayerOfHealingSpellService> prayerOfHealingSpellService,
            ISpellService<IRenewSpellService> renewSpellService,
            ISpellService<ICircleOfHealingSpellService> circleOfHealingSpellService)
            : base(gameStateService)
        {
            Spell = Spell.HolyWordSanctify;
            _prayerOfHealingSpellService = prayerOfHealingSpellService;
            _renewSpellService = renewSpellService;
            _circleOfHealingSpellService = circleOfHealingSpellService;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            var healingSp = spellData.GetEffect(24949).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            averageHeal *= this.GetPontifexMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Max casts per minute is (60 + (FH + Heal + BH * 0.5) * HwCDR) / CD + 1 / (FightLength / 60)
            // HWCDR is 6 base, more with LOTN/other effects
            // 1 from regular CD + reductions from fillers divided by the cooldown to get base CPM
            // Then add the one charge we start with, 1 per fight, into seconds.

            var cpmPoH = _prayerOfHealingSpellService.GetActualCastsPerMinute(gameState);
            var cpmRenew = _renewSpellService.GetActualCastsPerMinute(gameState);

            var hastedCD = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            var hwCDRPoH = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.PrayerOfHealing);
            var hwCDRRenew = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.Renew);

            double hwCDR = cpmPoH * hwCDRPoH +
                cpmRenew * hwCDRRenew;

            if (_gameStateService.GetTalent(gameState, Spell.HarmoniousApparatus).Rank > 0)
            {
                var cpmCoH = _circleOfHealingSpellService.GetActualCastsPerMinute(gameState);
                var hwCDRCoH = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.CircleOfHealing);
                hwCDR += cpmCoH * hwCDRCoH;
            }

            double charges = spellData.Charges + GetMiracleWorkerCharges(gameState, spellData);

            double maximumPotentialCasts = (60d + hwCDR) / hastedCD
                + charges / (fightLength / 60d);

            return maximumPotentialCasts;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Sanc stores its max number of targets in 288932.BaseValue
            var numTargets = spellData.GetEffect(288932).BaseValue;

            return numTargets;
        }

        public override double GetHastedCooldown(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Cooldown for Sanctify is stored in the chargecooldown instead as it has charges
            var cooldown = spellData.ChargeCooldown / 1000;

            return spellData.IsCooldownHasted
                ? cooldown / _gameStateService.GetHasteMultiplier(gameState)
                : cooldown;
        }

        internal double GetMiracleWorkerCharges(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var miracleWorkerCharges = 0d;

            if (_gameStateService.GetTalent(gameState, Spell.MiracleWorker).Rank > 0)
            {
                var miracleWorkerSpellData = _gameStateService.GetSpellData(gameState, Spell.MiracleWorker);
                miracleWorkerCharges += miracleWorkerSpellData.GetEffect(356036).BaseValue;
            }

            return miracleWorkerCharges;
        }
    }
}
