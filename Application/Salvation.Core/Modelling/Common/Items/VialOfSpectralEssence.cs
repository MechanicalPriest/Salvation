using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IVialOfSpectralEssenceSpellService : ISpellService { }
    class VialOfSpectralEssence : SpellService, ISpellService<IVialOfSpectralEssenceSpellService>
    {
        public VialOfSpectralEssence(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.VialOfSpectralEssence;
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

            // Get scale budget
            var scaledHealingValue = spellData.GetEffect(871762).GetScaledCoefficientValue(itemLevel);
            if (scaledHealingValue == 0)
                throw new ArgumentOutOfRangeException("itemLevel", $"healSpell.ScaleValues does not contain itemLevel: {itemLevel}");

            var healingPool = scaledHealingValue;

            healingPool *= _gameStateService.GetVersatilityMultiplier(gameState);

            healingPool *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return healingPool * GetNumberOfHealingTargets(gameState, spellData);
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
            // TODO: Add the direct heal effect from 345701?
            return true;
        }
    }
}
