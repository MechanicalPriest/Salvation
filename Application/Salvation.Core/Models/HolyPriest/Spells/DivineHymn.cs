using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Models.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest.Spells
{
    public class DivineHymn : SpellService, IDivineHymnSpellService
    {
        public DivineHymn(IGameStateService gameStateService,
            IModellingJournal journal)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.DivineHymn;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.DivineHymn);

            var holyPriestAuraHealingBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;
            var divineHymnAura = gameStateService.GetModifier(gameState, "DivineHymnBonusHealing").Value;

            // DH's average heal for the first tick is:
            // SP% * Intellect * Vers * Hpriest Aura
            decimal firstTickRaid = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            // double it if we have 5 or less (dungeon group buff)
            decimal firstTickParty = firstTickRaid * 2;

            journal.Entry($"[{spellData.Name}] Tooltip: {firstTickRaid:0.##} & {firstTickParty:0.##} (first tick)");
            journal.Entry($"[{spellData.Name}] Tooltip: {firstTickRaid * 5:0.##} & {firstTickParty * 5:0.##} (all ticks)");

            // Pick whether we're in part or raid
            decimal firstTick = spellData.NumberOfHealingTargets <= 5 ? firstTickParty : firstTickRaid;

            firstTick *= gameStateService.GetCriticalStrikeMultiplier(gameState);

            // Now the rest of the 4 ticks including the aura:
            decimal averageHeal = firstTick + (firstTick * 4 * (1 + divineHymnAura));

            return averageHeal * spellData.NumberOfHealingTargets;
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.DivineHymn);

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
