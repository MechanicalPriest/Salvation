using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IEchoOfEonarSpellSevice : ISpellService { }
    class EchoOfEonar : SpellService, ISpellService<IEchoOfEonarSpellSevice>
    {
        public EchoOfEonar(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.EchoOfEonar;
        }

        public override double GetAverageHealingBonus(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Healing bonus is self bonus + ally bonuses. Toggle these with a playstyle option
            var eonarHealingBuff = _gameStateService.GetSpellData(gameState, Spell.EchoOfEonarHealingBuffSelf);

            var healingBonus = eonarHealingBuff.GetEffect(875123).BaseValue;

            var countAllyBuffs = _gameStateService.GetPlaystyle(gameState, "EchoOfEonarCountAllyBuffs");

            if (countAllyBuffs == null)
                throw new ArgumentOutOfRangeException("EchoOfEonarCountAllyBuffs", $"EchoOfEonarCountAllyBuffs needs to be set.");

            // TODO: This 3 comes from MaxTargets on 338489 and should be data driven at some stage.
            // If we're counting ally buffs t get an approximate value, add 3 at 50% of the strength (3 allies)
            if (countAllyBuffs.Value == 1)
                healingBonus += healingBonus * spellData.GetEffect(875130).BaseValue * 3.0d / 100;

            return healingBonus * GetUptime(gameState, spellData) / 100;
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.EchoOfEonarHealingBuffSelf);

            var duration = buffSpellData.Duration / 1000;

            return duration;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var duration = GetDuration(gameState, spellData);

            return RppmBadluckProtection * (1 - (Math.Exp(-1 * spellData.Rppm * duration / 60) / 1));
        }
    }
}
