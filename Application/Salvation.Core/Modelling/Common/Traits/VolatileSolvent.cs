using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface IVolatileSolventSpellService : ISpellService { }

    internal class VolatileSolvent : SpellService, ISpellService<IVolatileSolventSpellService>
    {
        public VolatileSolvent(IGameStateService gameStateService)
            : base(gameStateService)
        {
            // TODO: Pull in fleshcraft once implemented to populate the CPM
            Spell = Spell.VolatileSolvent;
        }

        public override double GetAverageMastery(GameState gameState, BaseSpellData spellData)
        {
            // TODO: Humanoid buff: X mastery for 2 mins
            var masteryBuffSpell = _gameStateService.GetSpellData(gameState, Spell.VolatileSolventHumanoid);

            // Crit amount: 824199 effect 1
            var masteryAmount = masteryBuffSpell.GetEffect(812461).Coefficient * ItemCoefficientMultiplier;

            //// Average number of allies nearby
            //var averageStacks = _gameStateService.GetPlaystyle(gameState, "PointedCourageAverageNearbyAllies");

            //if (averageStacks == null)
            //    throw new ArgumentOutOfRangeException("PointedCourageAverageNearbyAllies", $"PointedCourageAverageNearbyAllies needs to be set.");

            //// Amount * Stacks * Uptime / Convert_To_Seconds / Convert_To_Percent
            //return critAmount * Math.Min(averageStacks.Value, critBuffSpell.MaxStacks);
            return 0;
        }

        public override double GetAverageIntellect(GameState gameState, BaseSpellData spellData)
        {
            // TODO: Beast buff: Intellect for 2 mins
            return base.GetAverageIntellect(gameState, spellData);
        }

        public override double GetAverageCriticalStrike(GameState gameState, BaseSpellData spellData)
        {
            // TODO: Dragonkin buff: Crit for 2mins
            return base.GetAverageCriticalStrike(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            // TODO: Pull in fleshcraft once implemented to populate the CPM
            // 120s CD.
            return 120 / 60;
        }

        // TODO: NYI Elemental (increased magic damage done)
        // TODO: NYI Mechanical (magic damage takent reduced)
    }
}
