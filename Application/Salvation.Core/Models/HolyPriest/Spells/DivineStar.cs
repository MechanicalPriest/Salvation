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
    public class DivineStar : SpellService, IDivineStarSpellService
    {
        public DivineStar(IGameStateService gameStateService,
            IModellingJournal journal)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.DivineStar;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.DivineStar);

            var holyPriestAuraHealingBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;
            
            decimal averageHeal = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##} (per pass)");

            averageHeal *= 2 // Add the second pass-back through each target
                * gameStateService.GetCriticalStrikeMultiplier(gameState);

            // Divine Star caps at roughly 6 targets worth of healing
            return averageHeal * Math.Min(6, GetNumberOfHealingTargets(gameState, spellData, moreData));
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.DivineStar);

            var hastedCastTime = GetHastedCastTime(gameState, spellData, moreData);
            var hastedCd = GetHastedCooldown(gameState, spellData, moreData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            decimal maximumPotentialCasts = 60m / (hastedCastTime + hastedCd)
                + 1m / (fightLength / 60m);

            return maximumPotentialCasts;
        }
    }
}
