using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface ISoullettingRubySpellService : ISpellService { }
    public class SoullettingRuby : SpellService, ISpellService<ISoullettingRubySpellService>
    {
        public SoullettingRuby(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.SoullettingRuby;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Use: Draw out a piece of the target's soul, decreasing their movement speed by 30% until the soul 
            // reaches you. The soul instantly heals you for 2995, and grants you up to 1050 Critical Strike for 16 sec. 
            // You gain more Critical Strike from lower health targets. (2 Min Cooldown)

            if (!spellData.Overrides.ContainsKey(Override.ItemLevel))
                throw new ArgumentOutOfRangeException("ItemLevel", "Does not contain ItemLevel");

            var itemLevel = (int)spellData.Overrides[Override.ItemLevel];

            var healSpell = _gameStateService.GetSpellData(gameState, Spell.SoullettingRubyHeal);

            // Get scale budget
            var scaledHealValue = healSpell.GetEffect(871957).GetScaledCoefficientValue(itemLevel);

            if (scaledHealValue == 0)
                throw new ArgumentOutOfRangeException("itemLevel", $"critBuffSpell.ScaleValues does not contain itemLevel: {itemLevel}");

            var healAmount = scaledHealValue;

            healAmount *= _gameStateService.GetVersatilityMultiplier(gameState);

            healAmount *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return healAmount * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetAverageCriticalStrike(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            if (!spellData.Overrides.ContainsKey(Override.ItemLevel))
                throw new ArgumentOutOfRangeException("ItemLevel", "Does not contain ItemLevel");

            var itemLevel = (int)spellData.Overrides[Override.ItemLevel];

            var critBuffSpell = _gameStateService.GetSpellData(gameState, Spell.SoullettingRubyTrigger);

            // Get scale budget
            var scaledLowCritValue = critBuffSpell.GetEffect(871958).GetScaledCoefficientValue(itemLevel);
            var scaledHighCritValue = critBuffSpell.GetEffect(871962).GetScaledCoefficientValue(itemLevel);


            if (scaledLowCritValue == 0 || scaledHighCritValue == 0)
                throw new ArgumentOutOfRangeException("itemLevel", $"critBuffSpell.ScaleValues does not contain itemLevel: {itemLevel}");

            var critAmountLow = scaledLowCritValue;
            var critAmountHigh = scaledHighCritValue;

            var avgEnemyHp = _gameStateService.GetPlaystyle(gameState, "SoullettingRubyAverageEnemyHP");

            if (avgEnemyHp == null)
                throw new ArgumentOutOfRangeException("SoullettingRubyAverageEnemyHP", $"SoullettingRubyAverageEnemyHP needs to be set.");

            // critAmountLow on 100% enemy HP. critAmountLow + critAmountHigh on 0% HP, linear inbetween.
            var averageCrit = critAmountLow + critAmountHigh * (1 - avgEnemyHp.Value);

            return averageCrit * GetUptime(gameState, spellData);
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            var buffSpell = _gameStateService.GetSpellData(gameState, Spell.SoullettingRubyBuff);

            return buffSpell.Duration / 1000;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Uptime is actual casts by uptime
            return GetDuration(gameState, spellData) / 60 * GetActualCastsPerMinute(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // This procs every minute almost exactly with very little variation
            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            return 60 / hastedCd
                + 1d / (fightLength / 60d); // plus one at the start of the fight
        }

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // The spell effect type is nested down inside a couple of effect trigger spells
            return base.TriggersMastery(gameState, spellData.GetEffect(871963).TriggerSpell.GetEffect(871953).TriggerSpell);
        }
    }
}
