using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface IResonantAccoladesSpellService : ISpellService { }
    public class ResonantAccolades : SpellService, ISpellService<IResonantAccoladesSpellService>
    {
        public ResonantAccolades(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.ResonantAccolades;
        }

        public override double GetAverageHealingBonus(GameState gameState, BaseSpellData spellData)
        {
            // TODO: Check if there is an ICD on the heal
            spellData = ValidateSpellData(gameState, spellData);

            var healingTriggers = _gameStateService.GetPlaystyle(gameState, "ResonantAccoladesHealingOver70Percent");

            if (healingTriggers == null)
                throw new ArgumentOutOfRangeException("ResonantAccoladesHealingOver70Percent", $"ResonantAccoladesHealingOver70Percent needs to be set.");

            var overhealing = _gameStateService.GetSpellCastProfile(gameState, SpellId);

            var healingAmount = spellData.GetEffect(839991).BaseValue / 100;

            // Percentage of healing over 70% * healing amount, minus overheal
            return healingTriggers.Value * healingAmount * (1 - overhealing.OverhealPercent);
        }
    }
}
