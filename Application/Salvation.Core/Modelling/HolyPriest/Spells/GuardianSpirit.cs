using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public interface IGuardianSpiritSpellService : ISpellService { }

    public class GuardianSpirit : SpellService, ISpellService<IGuardianSpiritSpellService>
    {
        public GuardianSpirit(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.GuardianSpirit;
        }

        public override double GetAverageHealingBonus(GameState gameState, BaseSpellData spellData)
        {
            // Use GS to apply a small healing bonus to all healing done.
            spellData = ValidateSpellData(gameState, spellData);

            // Get the healing bonus
            var healingBonus = (spellData.GetEffect(40042).BaseValue / 100);

            healingBonus += GetLastingSpiritAdditionalHealing(gameState, spellData);

            return healingBonus * GetUptime(gameState, spellData);
        }

        public override double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var duration = GetDuration(gameState, spellData);

            var actualCpm = GetActualCastsPerMinute(gameState, spellData);
            return duration * actualCpm / 60;
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Halo is simply 60 / (CastTime + CD) + 1 / (FightLength / 60)
            // Number of casts per minute plus one cast at the start of the encounter
            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            double maximumPotentialCasts = 60d / (hastedCastTime + hastedCd)
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var duration = base.GetDuration(gameState, spellData);

            if (_gameStateService.IsConduitActive(gameState, Conduit.LastingSpirit))
            {
                var conduitData = _gameStateService.GetSpellData(gameState, Spell.LastingSpirit);

                duration += (conduitData.GetEffect(836029).BaseValue / 1000);
            }

            return duration;
        }

        internal double GetLastingSpiritAdditionalHealing(GameState gameState, BaseSpellData spellData)
        {
            var additionalHealing = 0d;

            if (_gameStateService.IsConduitActive(gameState, Conduit.LastingSpirit))
            {
                var conduitData = _gameStateService.GetSpellData(gameState, Spell.LastingSpirit);
                var rank = _gameStateService.GetConduitRank(gameState, Conduit.FocusedMending);

                additionalHealing += (conduitData.ConduitRanks[rank] / 100);
            }

            return additionalHealing;
        }
    }
}
