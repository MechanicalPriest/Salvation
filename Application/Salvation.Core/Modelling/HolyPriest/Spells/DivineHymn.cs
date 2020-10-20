using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class DivineHymn : SpellService, IDivineHymnSpellService
    {
        public DivineHymn(IGameStateService gameStateService,
            IModellingJournal journal)
            : base(gameStateService, journal)
        {
            SpellId = (int)Spell.DivineHymn;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.DivineHymn);

            var holyPriestAuraHealingBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;
            var divineHymnAura = _gameStateService.GetModifier(gameState, "DivineHymnBonusHealing").Value;

            // DH's average heal for the first tick is:
            // SP% * Intellect * Vers * Hpriest Aura
            decimal firstTickRaid = spellData.Coeff1
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            // double it if we have 5 or less (dungeon group buff)
            decimal firstTickParty = firstTickRaid * 2;

            _journal.Entry($"[{spellData.Name}] Tooltip: {firstTickRaid:0.##} & {firstTickParty:0.##} (first tick)");
            _journal.Entry($"[{spellData.Name}] Tooltip: {firstTickRaid * 5:0.##} & {firstTickParty * 5:0.##} (all ticks)");

            // Pick whether we're in part or raid
            decimal firstTick = GetNumberOfHealingTargets(gameState, spellData) <= 5 ? firstTickParty : firstTickRaid;

            firstTick *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            // Now the rest of the 4 ticks including the aura:
            decimal averageHeal = firstTick + (firstTick * 4 * (1 + divineHymnAura));

            return averageHeal * (decimal)GetNumberOfHealingTargets(gameState, spellData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.DivineHymn);

            var hastedCooldown = GetHastedCooldown(gameState, spellData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            // DH is simply 60 / CD + 1 / (FightLength / 60)
            // Number of casts per minute plus one cast at the start of the encounter
            decimal maximumPotentialCasts = 60m / hastedCooldown
                + 1m / (fightLength / 60m);

            return maximumPotentialCasts;
        }
    }
}
