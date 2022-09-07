using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IDarkmoonDeckReposeSpellService : ISpellService { }
    class DarkmoonDeckRepose : SpellService, ISpellService<IDarkmoonDeckReposeSpellService>
    {
        public DarkmoonDeckRepose(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.DarkmoonDeckRepose;
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

            var lowHeal = _gameStateService.GetSpellData(gameState, Spell.DarkmoonDeckReposeAce);
            var highHeal = _gameStateService.GetSpellData(gameState, Spell.DarkmoonDeckReposeEight);

            // Get scale budget
            // TODO: These spells aren't getting scale values for ilvl 200.
            var scaledHealValueLow = lowHeal.GetEffect(792442).GetScaledCoefficientValue(itemLevel);
            var scaledHealValueHigh = highHeal.GetEffect(792449).GetScaledCoefficientValue(itemLevel);

            if (scaledHealValueLow == 0)
                throw new ArgumentOutOfRangeException("itemLevel", $"lowHeal.ScaleValues does not contain itemLevel: {itemLevel}");

            if (scaledHealValueHigh == 0)
                throw new ArgumentOutOfRangeException("itemLevel", $"highHeal.ScaleValues does not contain itemLevel: {itemLevel}");

            var healAmount = (scaledHealValueLow + scaledHealValueHigh) / 2;

            healAmount *= _gameStateService.GetVersatilityMultiplier(gameState);

            healAmount *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            // The pool is split between all targets so we spread it between them all. Can't heal one target for more than 30% though
            healAmount = Math.Min(healAmount * 0.30, healAmount / GetNumberOfHealingTargets(gameState, spellData));

            return healAmount * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            return 60 / hastedCd
                + 1d / (fightLength / 60d); // plus one at the start of the fight
        }

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            // TODO: Add the direct heal effect from 333732?
            return true;
        }
    }
}
