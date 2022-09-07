using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.HolyPriest.Spells;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface IUltimateFormSpellService : ISpellService { }

    internal class UltimateForm : SpellService, ISpellService<IUltimateFormSpellService>
    {
        private readonly ISpellService<IFleshcraftSpellService> _fleshcraftSpellService;

        public UltimateForm(IGameStateService gameStateService,
            ISpellService<IFleshcraftSpellService> fleshcraftSpellService)
            : base(gameStateService)
        {
            Spell = Spell.UltimateForm;
            _fleshcraftSpellService = fleshcraftSpellService;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            // 2% health for 8 seconds (4 during channel + 4 if you complete it)
            var healData = _gameStateService.GetSpellData(gameState, Spell.UltimateFormHeal);

            // Healing amount (% of health pool)
            var healPercent = healData.GetEffect(873342).BaseValue / 100;

            var healAmount = healPercent
                * _gameStateService.GetHitpoints(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState);

            // Healing amount * target (you) * duration of buff (ticks every 1s, 4s = 4 tick) * 2 for the bonus at the end
            // TODO: Update the 8 to come from a configuration variable, if you don't channel full you only get 1-3 ticks. If you
            // channel full you get the 4th tick & the bonus 4 ticks.
            return healAmount * GetNumberOfHealingTargets(gameState) * 8;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            return _fleshcraftSpellService.GetActualCastsPerMinute(gameState, null);
        }
    }
}
