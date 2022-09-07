using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface ISoothingShadeSpellService : ISpellService { }

    internal class SoothingShade : SpellService, ISpellService<ISoothingShadeSpellService>
    {
        public SoothingShade(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.SoothingShade;
        }

        public override double GetAverageMastery(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Your spells and abilities have a chance to call Tubbins and Gubbins to your side for $336808d, 
            // parasol in hand. Standing in the shaded area grants you $336885s1 Mastery.

            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.SoothingShadeBuff);

            var masteryAmount = buffSpellData.GetEffect(834570).Coefficient * ItemCoefficientMultiplier;

            return masteryAmount * GetUptime(gameState, spellData);
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.SoothingShadeEffect);

            var duration = buffSpellData.Duration / 1000;

            return duration;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            return (GetActualCastsPerMinute(gameState, spellData) * GetDuration(gameState, spellData)) / 60;
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // TODO: Validate RPPM with ICD against proc rate/uptime values properly.
            var rppm = spellData.Rppm;

            var secondsBetweenProcs = rppm * 60 + 20;

            return 60 / secondsBetweenProcs;
        }
    }
}
