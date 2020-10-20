using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class Mindgames : SpellService, IMindgamesSpellService
    {
        public Mindgames(IGameStateService gameStateService,
            IModellingJournal journal)
            : base(gameStateService, journal)
        {
            SpellId = (int)Spell.Mindgames;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.Mindgames);

            var holyPriestAuraHealingBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            // Mind Game's average heal is:
            // $damage=${($SPS*$s2/100)*(1+$@versadmg)}
            // (SP% * Coeff1 / 100) * Vers
            decimal averageHeal = (spellData.Coeff1 * _gameStateService.GetIntellect(gameState) / 100)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            // Mindgames absorbs the incoming hit 323701, and heals for the amount absorbed 323706. 
            // The order of events though is Heal then Absorb/Damage.

            return averageHeal * 2 * (decimal)GetNumberOfHealingTargets(gameState, spellData);
        }

        public override decimal GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.Mindgames);

            var holyPriestAuraDamageBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraDamageMultiplier").Value;

            // coeff3 * int * hpriest dmg mod * vers
            decimal averageDamage = spellData.Coeff3
                * _gameStateService.GetIntellect(gameState)
                * holyPriestAuraDamageBonus
                * _gameStateService.GetVersatilityMultiplier(gameState);

            _journal.Entry($"[{spellData.Name}] Tooltip (Dmg): {averageDamage:0.##}");

            // Get the Shattered Perceptions conduit bonus damage
            // TODO: Shift this out to another method maybe, for testing?
            if (_gameStateService.IsConduitActive(gameState, Conduit.ShatteredPerceptions))
            {
                var rank = _gameStateService.GetConduitRank(gameState, Conduit.ShatteredPerceptions);
                var conduitData = _gameStateService.GetConduitData(gameState, Conduit.ShatteredPerceptions);

                averageDamage *= (1 + (conduitData.Ranks[rank] / 100));
            }

            return averageDamage * GetNumberOfDamageTargets(gameState, spellData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.Mindgames);

            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            decimal maximumPotentialCasts = 60m / hastedCd
                + 1m / (fightLength / 60m);

            return maximumPotentialCasts;
        }

        public override decimal GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.Mindgames);

            var baseDuration = base.GetDuration(gameState, spellData);

            // Apply the duration component of the Shattered Perceptions conduit.
            // TODO: Shift this out to another method maybe, for testing?
            if (_gameStateService.IsConduitActive(gameState, Conduit.ShatteredPerceptions))
            {
                var conduitData = _gameStateService.GetConduitData(gameState, Conduit.ShatteredPerceptions);

                // The added duration is the same regardless of rank
                baseDuration += conduitData.Coeff1;
            }
            return baseDuration;
        }
    }
}
