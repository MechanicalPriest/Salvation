using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IOverflowingAnimaCageSpellService : ISpellService { }
    public class OverflowingAnimaCage : SpellService, ISpellService<IOverflowingAnimaCageSpellService>
    {
        public OverflowingAnimaCage(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.OverflowingAnimaCage;
        }

        public override double GetAverageCriticalStrike(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            if(!spellData.Overrides.ContainsKey(Override.ItemLevel))
                throw new ArgumentOutOfRangeException("ItemLevel", "Does not contain ItemLevel");

            var itemLevel = (int)spellData.Overrides[Override.ItemLevel];

            var buffSpell = _gameStateService.GetSpellData(gameState, Spell.OverflowingAnimaCageBuff);

            // Get scale budget
            if(!buffSpell.ScaleValues.ContainsKey(itemLevel))
                throw new ArgumentOutOfRangeException("itemLevel", $"buffSpell.ScaleValues does not contain itemLevel: {itemLevel}");

            var scaleBudget = buffSpell.ScaleValues[itemLevel];

            var critAmount = scaleBudget * buffSpell.GetEffect(845125).Coefficient;

            // If we are counting ally buffs, multiply our crit amount
            var countAllyBuffs = _gameStateService.GetPlaystyle(gameState, "OverflowingAnimaCageCountAllyBuffs");

            if (countAllyBuffs == null)
                throw new ArgumentOutOfRangeException("OverflowingAnimaCageCountAllyBuffs", $"OverflowingAnimaCageCountAllyBuffs needs to be set.");

            var avgNumAllies = _gameStateService.GetPlaystyle(gameState, "OverflowingAnimaCageAverageNumberAllies");

            if (avgNumAllies == null)
                throw new ArgumentOutOfRangeException("OverflowingAnimaCageAverageNumberAllies", $"OverflowingAnimaCageAverageNumberAllies needs to be set.");

            if (countAllyBuffs.Value == 1)
                critAmount *= avgNumAllies.Value;

            return critAmount * GetUptime(gameState, spellData);
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            // Uptime is uptime is duration per minute. One proc every minute almost exactly 
            return (GetDuration(gameState, spellData) * GetActualCastsPerMinute(gameState, spellData)) / 60;
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            return 60 / hastedCd
                + 1d / (fightLength / 60d); // plus one at the start of the fight
        }
    }
}
