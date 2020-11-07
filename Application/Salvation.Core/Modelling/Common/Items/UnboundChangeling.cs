using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using SimcProfileParser.Model.Generated;
using System;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IUnboundChangeling : ISpellService { }
    public class UnboundChangeling : SpellService, ISpellService<IUnboundChangeling>
    {
        public UnboundChangeling(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.UnboundChangeling;
        }

        public override double GetAverageHaste(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            if (!spellData.Overrides.ContainsKey(Override.ScaleBudget))
                throw new ArgumentOutOfRangeException("Overrides", "Does not contain ScaleBudget");

            var scaleBudget = spellData.Overrides[Override.ScaleBudget];
            var hasteBuffSpell = _gameStateService.GetSpellData(gameState, Spell.UnboundChangelingHasteProc);

            // X haste for Y seconds. RPPM not haste modified.
            // Currently there is a bug where this trinket is providing less than the tooltip
            // The tooltip provides 2.2 * scale_value (from effect #1 824555) but buff provides
            // only 1.1 * scale_value (from effect #2 873512)
            var hasteAmount = scaleBudget * hasteBuffSpell.GetEffect(873512).Coefficient;

            return hasteAmount * GetUptime(gameState, spellData);
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var duration = spellData.GetEffect(824536).TriggerSpell.Duration / 1000;

            return duration;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var duration = GetDuration(gameState, spellData);

            // Poisson function is (e^-λ) / 1
            return RppmBadluckProtection * (1 - (Math.Exp(-1 * spellData.Rppm * duration / 60) / 1));
        }
    }
}
