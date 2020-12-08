using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.Common.Traits
{
    public interface IGroveInvigorationSpellService : ISpellService { }

    internal class GroveInvigoration : SpellService, ISpellService<IGroveInvigorationSpellService>
    {
        private readonly ISpellService<IFaeGuardiansSpellService> _faeGuardiansSpellService;

        public GroveInvigoration(IGameStateService gameStateService,
            ISpellService<IFaeGuardiansSpellService> faeGuardiansSpellService)
            : base(gameStateService)
        {
            Spell = Spell.GroveInvigoration;
            _faeGuardiansSpellService = faeGuardiansSpellService;
        }

        public override double GetAverageMastery(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Healing or dealing damage has a chance to grant you a stack of Redirected Anima. Redirected Anima 
            // increases your maximum health by $342814s1% and your Mastery by $342814s2 for $342814d, and stacks overlap.
            // [Activating your Night Fae class ability] grants you ${$s3*$<mod>} stacks of Redirected Anima.

            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.GroveInvigorationAnimaBuff);

            // This is a player scaled effect
            var buffPerStack = buffSpellData.GetEffect(844158).Coefficient * ItemCoefficientMultiplier;

            // Average stacks per minute without the Fae Guardians boost
            var averageBaseStacks = GetUptime(gameState, spellData);

            // Add on the fae guardians - the 1.5 mod comes from the Variables component of spell 322721.
            var fgStacksPerCast = spellData.GetEffect(875153).BaseValue * 1.5;
            var fgCastsPerMinute = _faeGuardiansSpellService.GetActualCastsPerMinute(gameState, null);
            var fgAverageUptime = fgCastsPerMinute * fgStacksPerCast * GetDuration(gameState, spellData) / 60;

            return buffPerStack * (averageBaseStacks + fgAverageUptime);
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            var buffSpellData = _gameStateService.GetSpellData(gameState, Spell.GroveInvigorationAnimaBuff);

            var duration = buffSpellData.Duration / 1000;

            return duration;
        }

        /// <summary>
        /// Uptime is a bit misleading for this spell as it only applies to the RPPM stacks, not the ones applied by the class ability
        /// </summary>
        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Value of 1 = 100% uptime
            // Uptime is the rppm stacks of 1 along with the Fae Guardians stacks of 12.

            // TODO: 0.60 multiplier is here as part of the Grove Invig bug. This also screws with the RPPM BLP
            var rppm = spellData.Rppm * 0.60;
            var regularStacksUptime = rppm * GetDuration(gameState, spellData) / 60;

            return regularStacksUptime;
        }
    }
}
