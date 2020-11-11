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
            spellData = ValidateSpellData(gameState, spellData);

            // 1% vers for each new spell cast during the buff up to 4 stacks
            var versBuffSpell = _gameStateService.GetSpellData(gameState, Spell.LetGoOfThePastBuff);

            // Mastery amount: 328908 effect 1
            var versAmount = versBuffSpell.GetEffect(821709).BaseValue;

            // Average stacks while the buff is up
            var averageStacks = _gameStateService.GetPlaystyle(gameState, "LetGoOfThePastAverageStacks");

            if (averageStacks == null)
                throw new ArgumentOutOfRangeException("LetGoOfThePastAverageStacks", $"LetGoOfThePastAverageStacks needs to be set.");

            // Amount * Stacks * Uptime / Convert_To_Seconds / Convert_To_Percent
            return versAmount * Math.Min(averageStacks.Value, versBuffSpell.MaxStacks)
                * GetUptime(gameState, spellData) / 60 / 100;
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
