using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public interface ITrailOfLightSpellService : ISpellService { }
    class TrailOfLight : SpellService, ISpellService<ITrailOfLightSpellService>
    {
        public TrailOfLight(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.TrailOfLight;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var healingMultiplier = spellData.GetEffect(294596).BaseValue / 100;

            var rank = _gameStateService.GetTalent(gameState, Spell.TrailOfLight).Rank;

            healingMultiplier *= rank;

            var triggeringHealAmount = spellData.Overrides[Override.ResultMultiplier];

            return healingMultiplier * triggeringHealAmount * GetNumberOfHealingTargets(gameState, spellData);
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
            var trailHealSpellData = _gameStateService.GetSpellData(gameState, Spell.TrailOfLightHeal);

            return base.TriggersMastery(gameState, trailHealSpellData);
        }
    }
}
