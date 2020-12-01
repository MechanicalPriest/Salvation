using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface ICabalistsHymnalSpellService : ISpellService { }
    public class CabalistsHymnal : SpellService, ISpellService<ICabalistsHymnalSpellService>
    {
        public CabalistsHymnal(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.CabalistsHymnal;
        }

        public override double GetAverageCriticalStrike(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            if(!spellData.Overrides.ContainsKey(Override.ItemLevel))
                throw new ArgumentOutOfRangeException("ItemLevel", "Does not contain ItemLevel");

            var itemLevel = (int)spellData.Overrides[Override.ItemLevel];

            var critBuffSpell = _gameStateService.GetSpellData(gameState, Spell.CabalistsHymnalBuff);

            // Get scale budget
            if(!critBuffSpell.ScaleValues.ContainsKey(itemLevel))
                throw new ArgumentOutOfRangeException("itemLevel", $"critBuffSpell.ScaleValues does not contain itemLevel: {itemLevel}");

            var scaleBudget = critBuffSpell.ScaleValues[itemLevel];

            var critAmount = scaleBudget * spellData.GetEffect(869450).Coefficient;

            // 3 stacks, average of 2 for its duration
            var averageCritAmount = critAmount * 2;

            return averageCritAmount * GetUptime(gameState, spellData);
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var critBuffSpell = _gameStateService.GetSpellData(gameState, Spell.CabalistsHymnalBuff);

            // Duration of the buff is 10s * 3 stacks
            return (critBuffSpell.Duration / 1000) * critBuffSpell.MaxStacks;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            var critBuffSpell = _gameStateService.GetSpellData(gameState, Spell.CabalistsHymnalBuff);

            // Uptime is uptime is duration per minute. One proc every minute almost exactly 
            return GetDuration(gameState, spellData) / 60;
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            return GetMaximumCastsPerMinute(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            // This procs every minute almost exactly with very little variation
            return 1;
        }
    }
}
