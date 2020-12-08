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

            // Crit amount: 824199 effect 1
            var healPercent = healData.GetEffect(873342).BaseValue / 100;

            var healAmount = healPercent
                * _gameStateService.GetHitpoints(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState);

            // Healing amount * target (you) * duration of buff (ticks every 1s, 4s = 4 tick) * 2 for the bonus at the end
            return healAmount * GetNumberOfHealingTargets(gameState) * (healData.Duration / 1000) * 2;
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
