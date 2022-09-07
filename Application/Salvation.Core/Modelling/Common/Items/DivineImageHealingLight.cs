using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IDivineImageHealingLightSpellService : ISpellService { }
    class DivineImageHealingLight : SpellService, ISpellService<IDivineImageHealingLightSpellService>
    {
        private readonly ISpellService<IFlashHealSpellService> _flashHealSpellService;
        private readonly ISpellService<IHealSpellService> _healSpellService;
        private readonly ISpellService<IHolyWordSerenitySpellService> _serenitySpellService;
        private readonly ISpellService<IBindingHealSpellService> _bindingHealSpellService;

        public DivineImageHealingLight(IGameStateService gameStateService,
            ISpellService<IFlashHealSpellService> flashHealSpellService,
            ISpellService<IHealSpellService> healSpellService,
            ISpellService<IHolyWordSerenitySpellService> serenitySpellService,
            ISpellService<IBindingHealSpellService> bindingHealSpellService)
            : base(gameStateService)
        {
            Spell = Spell.DivineImageHealingLight;
            _flashHealSpellService = flashHealSpellService;
            _healSpellService = healSpellService;
            _serenitySpellService = serenitySpellService;
            _bindingHealSpellService = bindingHealSpellService;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var healingSp = spellData.GetEffect(288947).SpCoefficient;

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
            var cpm = _flashHealSpellService.GetActualCastsPerMinute(gameState, null);
            cpm += _healSpellService.GetActualCastsPerMinute(gameState, null);
            cpm += _serenitySpellService.GetActualCastsPerMinute(gameState, null);

            if (_gameStateService.IsTalentActive(gameState, Talent.BindingHeal))
                cpm += _bindingHealSpellService.GetActualCastsPerMinute(gameState, null);

            return cpm;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }
    }
}
