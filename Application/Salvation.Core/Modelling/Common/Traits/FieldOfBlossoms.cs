using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface IFieldOfBlossomsSpellService : ISpellService { }

    internal class FieldOfBlossoms : SpellService, ISpellService<IFieldOfBlossomsSpellService>
    {
        private readonly ISpellService<IFaeGuardiansSpellService> _faeGuardiansSpellService;

        public FieldOfBlossoms(IGameStateService gameStateService,
            ISpellService<IFaeGuardiansSpellService> faeGuardiansSpellService)
            : base(gameStateService)
        {
            Spell = Spell.FieldOfBlossoms;
            _faeGuardiansSpellService = faeGuardiansSpellService;
        }

        public override double GetAverageHastePercent(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // [Activating your Night Fae class ability] puts flowers at your feet for ${$342761d*$<mod>}.1 
            // sec that increase your Haste by $342774s1% while you stand with them.
            // 342761 = FieldOfBlossomsEffect
            // 342774 = FieldOfBlossomsBuff

            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.FieldOfBlossomsBuff);

            var hasteAmount = buffSpellData.GetEffect(844100).BaseValue / 100;

            return hasteAmount * GetUptime(gameState, spellData);
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.FieldOfBlossomsEffect);

            // The 1.5 comes from the mod value in 319191 for priest class
            var duration = buffSpellData.Duration / 1000 * 1.5;

            return duration;
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            return (GetActualCastsPerMinute(gameState, spellData) * GetDuration(gameState, spellData)) / 60;
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            return _faeGuardiansSpellService.GetActualCastsPerMinute(gameState, null);
        }
    }
}
