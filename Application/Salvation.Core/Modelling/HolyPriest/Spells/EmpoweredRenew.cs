using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public interface IEmpoweredRenewSpellService : ISpellService { }
    class EmpoweredRenew : SpellService, ISpellService<IEmpoweredRenewSpellService>
    {
        public EmpoweredRenew(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.EmpoweredRenew;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var healingMultiplier = spellData.GetEffect(1028796).BaseValue / 100;

            var rank = _gameStateService.GetTalent(gameState, Spell.EmpoweredRenew).Rank;

            healingMultiplier *= rank;

            if (!spellData.Overrides.ContainsKey(Override.ResultMultiplier))
                throw new ArgumentOutOfRangeException("Override.ResultMultiplier", "SpellData Override.ResultMultiplier must be set.");

            var triggeringHealAmount = spellData.Overrides[Override.ResultMultiplier];

            // Healing amount is whatever renew heals for + vers.
            // While Emp Renew can crit, we inherit the crit multiplier from the renew heal amount so it isn't added here. 
            // Alternatively we could send the base healing amount through and apply crit afterwards.
            // Healing multipliers are not added to Empowered Renew, nor is the Holy Priest aura.
            var healingAmount = healingMultiplier
                * triggeringHealAmount
                * _gameStateService.GetVersatilityMultiplier(gameState);

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
            var trailHealSpellData = _gameStateService.GetSpellData(gameState, Spell.EmpoweredRenewHeal);

            return base.TriggersMastery(gameState, trailHealSpellData);
        }
    }
}
