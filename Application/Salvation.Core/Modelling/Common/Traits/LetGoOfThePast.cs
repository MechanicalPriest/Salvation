using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface ILetGoOfThePastSpellService : ISpellService { }

    internal class LetGoOfThePast : SpellService, ISpellService<ILetGoOfThePastSpellService>
    {
        public LetGoOfThePast(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.LetGoOfThePast;
        }

        public override double GetAverageVersatilityPercent(GameState gameState, BaseSpellData spellData)
        {
            // This was changed:
            // Using a spell or ability reduces your magic damage taken by 1% for 6 sec. Using another spell or ability 
            // increases this amount by 1% when it is not a repeat of the previous spell or ability, stacking to 3%.
            return 0;
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            var versBuffSpell = _gameStateService.GetSpellData(gameState, Spell.LetGoOfThePastBuff);

            return versBuffSpell.Duration / 1000;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            // Average total uptime of the buff
            var averageUptime = _gameStateService.GetPlaystyle(gameState, "LetGoOfThePastAverageUptime");

            if (averageUptime == null)
                throw new ArgumentOutOfRangeException("LetGoOfThePastAverageUptime", $"LetGoOfThePastAverageUptime needs to be set.");

            // Value of 1 = 100% uptime
            return averageUptime.Value * 60;
        }
    }
}
