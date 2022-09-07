using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.Core.Modelling.Common.Items
{
    public interface IUnboundChangelingSpellService : ISpellService { }
    public class UnboundChangeling : SpellService, ISpellService<IUnboundChangelingSpellService>
    {
        public UnboundChangeling(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.UnboundChangeling;
        }

        public override double GetAverageHaste(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            if(!spellData.Overrides.ContainsKey(Override.ItemLevel))
                throw new ArgumentOutOfRangeException("ItemLevel", "Does not contain ItemLevel");

            var itemLevel = (int)spellData.Overrides[Override.ItemLevel];

            var hasteBuffSpell = _gameStateService.GetSpellData(gameState, Spell.UnboundChangelingBuff);

            // SpellId:  (crit buff) - proc 330730
            // SpellId: 330131 (haste buff) - proc 330733
            // SpellId:  (mastery buff) - proc 330734
            // SpellId: 330764 (tri buff) - proc 330765
            // Scale values: 330747 - e#824555 normal, e#873512 tri buff


            // Get scale budget - 873512 for the triple buff
            var scaledHasteValue = hasteBuffSpell.GetEffect(824555).GetScaledCoefficientValue(itemLevel);
            if (scaledHasteValue == 0)
                throw new ArgumentOutOfRangeException("itemLevel", $"hasteBuffSpell.ScaleValues does not contain itemLevel: {itemLevel}");

            // X haste for Y seconds. RPPM not haste modified.
            // The tooltip provides 2.2 * scale_value (from effect #1 824555) but buff provides
            // only 1.1 * scale_value (from effect #2 873512)
            // 03 Dec. 2020: Effect 824555 is now being used.
            var hasteAmount = scaledHasteValue;

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

            // TODO: validate actual number of procs per minute
            // Poisson function is (e^-λ) / 1
            return RppmBadluckProtection * (1 - (Math.Exp(-1 * spellData.Rppm * duration / 60) / 1));
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            return GetMaximumCastsPerMinute(gameState, spellData);
        }
    }
}
