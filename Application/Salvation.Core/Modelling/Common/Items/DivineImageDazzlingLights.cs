using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IDivineImageDazzlingsLightSpellService : ISpellService { }
    class DivineImageDazzlingLights : SpellService, ISpellService<IDivineImageDazzlingsLightSpellService>
    {
        private readonly ISpellService<IPrayerOfHealingSpellService> _prayerOfHealingSpellService;
        private readonly ISpellService<IHolyWordSanctifySpellService> _holyWordSanctifySpellService;
        private readonly ISpellService<IDivineStarSpellService> _divineStarSpellService;
        private readonly ISpellService<IHaloSpellService> _haloSpellService;
        private readonly ISpellService<IDivineHymnSpellService> _divineHymnSpellService;
        private readonly ISpellService<ICircleOfHealingSpellService> _circleOfHealingSpellService;

        public DivineImageDazzlingLights(IGameStateService gameStateService,
            ISpellService<IPrayerOfHealingSpellService> prayerOfHealingSpellService,
            ISpellService<IHolyWordSanctifySpellService> holyWordSanctifySpellService,
            ISpellService<IDivineStarSpellService> divineStarSpellService,
            ISpellService<IHaloSpellService> haloSpellService,
            ISpellService<IDivineHymnSpellService> divineHymnSpellService,
            ISpellService<ICircleOfHealingSpellService> circleOfHealingSpellService)
            : base(gameStateService)
        {
            Spell = Spell.DivineImageDazzlingLight;
            _prayerOfHealingSpellService = prayerOfHealingSpellService;
            _holyWordSanctifySpellService = holyWordSanctifySpellService;
            _divineStarSpellService = divineStarSpellService;
            _haloSpellService = haloSpellService;
            _divineHymnSpellService = divineHymnSpellService;
            _circleOfHealingSpellService = circleOfHealingSpellService;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var healingSp = spellData.GetEffect(288949).SpCoefficient;

            var averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState);

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            if (!spellData.Overrides.ContainsKey(Override.AllowedDuration))
                throw new ArgumentOutOfRangeException("Override.AllowedDuration", "SpellData Override.AllowedDuration must be set.");

            var allowedDuration = spellData.Overrides[Override.AllowedDuration];

            var maxCpm = GetMaximumCastsPerMinute(gameState, spellData);

            // Average number of casts per minute is casts per second increased by the number of seconds the buff is active
            return maxCpm / 60 * allowedDuration;
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            var cpm = _prayerOfHealingSpellService.GetActualCastsPerMinute(gameState, null);
            cpm += _holyWordSanctifySpellService.GetActualCastsPerMinute(gameState, null);
            cpm += _divineHymnSpellService.GetActualCastsPerMinute(gameState, null);
            cpm += _circleOfHealingSpellService.GetActualCastsPerMinute(gameState, null);

            if(_gameStateService.GetTalent(gameState, Spell.DivineStar).Rank > 0)
                cpm += _divineStarSpellService.GetActualCastsPerMinute(gameState, null);

            if (_gameStateService.GetTalent(gameState, Spell.Halo).Rank > 0)
                cpm += _haloSpellService.GetActualCastsPerMinute(gameState, null);

            return cpm;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            return spellData.GetEffect(336109).BaseValue;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }
    }
}
