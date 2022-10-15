using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public interface IBindingHealsSpellService : ISpellService { }
    class BindingHeals : SpellService, ISpellService<IBindingHealsSpellService>
    {
        public BindingHeals(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.BindingHeals;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var healingMultiplier = spellData.GetEffect(912575).BaseValue / 100;

            var rank = _gameStateService.GetTalent(gameState, Spell.BindingHeals).Rank;

            healingMultiplier *= rank;

            if (!spellData.Overrides.ContainsKey(Override.ResultMultiplier))
                throw new ArgumentOutOfRangeException("Override.ResultMultiplier", "SpellData Override.ResultMultiplier must be set.");

            var triggeringHealAmount = spellData.Overrides[Override.ResultMultiplier];

            var unwaveringWillUptime = _gameStateService.GetPlaystyle(gameState, "BindingHealsSelfCastPercentage");

            if (unwaveringWillUptime == null)
                throw new ArgumentOutOfRangeException("BindingHealsSelfCastPercentage", $"BindingHealsSelfCastPercentage needs to be set.");

            var healingAmount = healingMultiplier
                * triggeringHealAmount
                * (1 - unwaveringWillUptime.Value);

            return healingAmount * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            if (!spellData.Overrides.ContainsKey(Override.CastsPerMinute))
                throw new ArgumentOutOfRangeException("Override.CastsPerMinute", "SpellData Override.CastsPerMinute must be set.");

            var cpm = spellData.Overrides[Override.CastsPerMinute];

            return cpm;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            var trailHealSpellData = _gameStateService.GetSpellData(gameState, Spell.BindingHealsHeal);

            return base.TriggersMastery(gameState, trailHealSpellData);
        }
    }
}
