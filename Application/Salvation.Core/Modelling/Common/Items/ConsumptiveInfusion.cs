using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IConsumptiveInfusionSpellService : ISpellService { }
    public class ConsumptiveInfusion : SpellService, ISpellService<IConsumptiveInfusionSpellService>
    {
        public ConsumptiveInfusion(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.ConsumptiveInfusion;
        }

        public override double GetAverageLeech(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Use: Draw out a piece of the target's soul, decreasing their movement speed by 30% until the soul 
            // reaches you. The soul instantly heals you for 2995, and grants you up to 1050 Critical Strike for 16 sec. 
            // You gain more Critical Strike from lower health targets. (2 Min Cooldown)

            if (!spellData.Overrides.ContainsKey(Override.ItemLevel))
                throw new ArgumentOutOfRangeException("ItemLevel", "Does not contain ItemLevel");

            var itemLevel = (int)spellData.Overrides[Override.ItemLevel];

            var buffSpell = _gameStateService.GetSpellData(gameState, Spell.ConsumptiveInfusionBuff);

            // Get scale budget
            var scaledLeechValue = buffSpell.GetEffect(868394).GetScaledCoefficientValue(itemLevel);

            if (scaledLeechValue == 0)
                throw new ArgumentOutOfRangeException("itemLevel", $"buffSpell.ScaleValues does not contain itemLevel: {itemLevel}");

            var leechAmount = scaledLeechValue;

            return leechAmount * GetUptime(gameState, spellData);
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // TODO: Should probably add at least one GCD per cooldown here, as it takes a GCD to trigger the damage buff
            var uptime = GetActualCastsPerMinute(gameState, spellData) * GetDuration(gameState, spellData);

            return uptime / 60;
        }

        public override double GetHastedCooldown(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var debuffSpell = spellData.GetEffect(868384).TriggerSpell.GetEffect(868400).TriggerSpell;

            return debuffSpell.Duration / 1000;
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var debuffSpell = spellData.GetEffect(868384).TriggerSpell.GetEffect(868400).TriggerSpell;
            var fightLength = _gameStateService.GetFightLength(gameState);

            return 60 / (debuffSpell.Duration / 1000)
                + 1d / (fightLength / 60d); // plus one at the start of the fight
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var debuffSpell = spellData.GetEffect(868384).TriggerSpell.GetEffect(868391).TriggerSpell;

            return debuffSpell.Duration / 1000;
        }
    }
}
