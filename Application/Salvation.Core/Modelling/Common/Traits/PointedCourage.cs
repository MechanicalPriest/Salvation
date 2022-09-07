using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface IPointedCourageSpellService : ISpellService { }

    internal class PointedCourage : SpellService, ISpellService<IPointedCourageSpellService>
    {
        public PointedCourage(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.PointedCourage;
        }

        public override double GetAverageCriticalStrikePercent(GameState gameState, BaseSpellData spellData)
        {
            // 1% crit for nearby allies up to 8%
            var critBuffSpell = _gameStateService.GetSpellData(gameState, Spell.PointedCourageBuff);

            // Crit amount: 824199 effect 1
            var critAmount = critBuffSpell.GetEffect(824199).BaseValue;

            // Average number of allies nearby
            var averageStacks = _gameStateService.GetPlaystyle(gameState, "PointedCourageAverageNearbyAllies");

            if (averageStacks == null)
                throw new ArgumentOutOfRangeException("PointedCourageAverageNearbyAllies", $"PointedCourageAverageNearbyAllies needs to be set.");

            // Amount * Stacks * Uptime / Convert_To_Seconds / Convert_To_Percent
            return critAmount * Math.Min(averageStacks.Value, critBuffSpell.MaxStacks) / 100;
        }
    }
}
