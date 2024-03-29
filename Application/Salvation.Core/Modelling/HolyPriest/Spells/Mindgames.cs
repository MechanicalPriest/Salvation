﻿using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class Mindgames : SpellService, ISpellService<IMindgamesSpellService>
    {
        public Mindgames(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.Mindgames;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var healingSp = spellData.GetEffect(812776).BaseValue;

            // Mind Game's average heal is:
            // $damage=${($SPS*$s2/100)*(1+$@versadmg)}
            // The heal doesn't scale with the healing aura. See Issue #72
            double averageHeal = (healingSp * _gameStateService.GetIntellect(gameState) / 100)
                * _gameStateService.GetVersatilityMultiplier(gameState);

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= GetShatteredPerceptionsModifier(gameState);
            // Mindgames absorbs the incoming hit 323701, and heals for the amount absorbed 323706. 
            // The order of events though is Heal then Absorb/Damage.

            return averageHeal * 2 * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraDamageBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(191077).BaseValue / 100 + 1;

            // coeff * int * hpriest dmg mod * vers
            var reverseDamageSp = spellData.GetEffect(812771).SpCoefficient;
            double averageDamage = reverseDamageSp
                * _gameStateService.GetIntellect(gameState)
                * holyPriestAuraDamageBonus
                * _gameStateService.GetVersatilityMultiplier(gameState);

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip (Dmg): {averageDamage:0.##}");

            // Get the Shattered Perceptions conduit bonus damage
            averageDamage *= GetShatteredPerceptionsModifier(gameState);

            return averageDamage * GetNumberOfDamageTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            double maximumPotentialCasts = 60d / hastedCd
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var baseDuration = base.GetDuration(gameState, spellData);

            // Apply the duration component of the Shattered Perceptions conduit.
            // TODO: Shift this out to another method maybe, for testing?
            if (_gameStateService.IsConduitActive(gameState, Conduit.ShatteredPerceptions))
            {
                var conduitData = _gameStateService.GetSpellData(gameState, Spell.ShatteredPerceptions);

                // The added duration is the same regardless of rank
                baseDuration += conduitData.GetEffect(836828).BaseValue / 1000;
            }
            return baseDuration;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMinimumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            // MindGames Spellid doesnt have the "right" type, heal component does
            var healData = _gameStateService.GetSpellData(gameState, Spell.MindgamesHeal);

            return base.TriggersMastery(gameState, healData);
        }

        internal double GetShatteredPerceptionsModifier(GameState gameState)
        {
            var multi = 1d;

            if (_gameStateService.IsConduitActive(gameState, Conduit.ShatteredPerceptions))
            {
                var rank = _gameStateService.GetConduitRank(gameState, Conduit.ShatteredPerceptions);
                var conduitData = _gameStateService.GetSpellData(gameState, Spell.ShatteredPerceptions);

                multi += (conduitData.ConduitRanks[rank] / 100d);
            }

            return multi;
        }
    }
}
