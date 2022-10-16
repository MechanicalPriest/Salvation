using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;


namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class HolyWordChastise : SpellService, ISpellService<IHolyWordChastiseSpellService>
    {
        private readonly ISpellService<ISmiteSpellService> _smiteSpellService;
        private readonly ISpellService<IHolyFireSpellService> _holyFireSpellService;

        public HolyWordChastise(IGameStateService gameStateService,
            ISpellService<ISmiteSpellService> smiteSpellService,
            ISpellService<IHolyFireSpellService> holyFireSpellService)
            : base(gameStateService)
        {
            Spell = Spell.HolyWordChastise;
            _smiteSpellService = smiteSpellService;
            _holyFireSpellService = holyFireSpellService;
        }

        public override double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraDamageBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(191077).BaseValue / 100 + 1;

            var damageSp = spellData.GetEffect(91044).SpCoefficient;

            double averageDmg = damageSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamageBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageDmg:0.##}");

            averageDmg *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageDmg * GetNumberOfDamageTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Max casts per minute is (60 + Smite * HwCDR) / CD + 1 / (FightLength / 60)
            // HWCDR is 6 base, more with LOTN/other effects
            // 1 from regular CD + reductions from fillers divided by the cooldown to get base CPM
            // Then add the one charge we start with, 1 per fight, into seconds.

            var cpmSmite = _smiteSpellService.GetActualCastsPerMinute(gameState);

            var hastedCD = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            var hwCDRSmite = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.Smite);

            double hwCDR = cpmSmite * hwCDRSmite;

            if (_gameStateService.GetTalent(gameState, Spell.HarmoniousApparatus).Rank > 0)
            {
                var cpmHF = _holyFireSpellService.GetActualCastsPerMinute(gameState);
                var hwCDRHF = _gameStateService.GetTotalHolyWordCooldownReduction(gameState, Spell.HolyFire);
                hwCDR += cpmHF * hwCDRHF;
            }

            double maximumPotentialCasts = (60d + hwCDR) / hastedCD
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }

        public override double GetMinimumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }
    }
}
