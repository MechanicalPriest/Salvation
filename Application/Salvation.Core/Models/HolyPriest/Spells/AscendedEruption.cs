﻿using Salvation.Core.Constants;
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
    public class AscendedEruption : SpellService, IAscendedEruptionSpellService
    {
        public AscendedEruption(IGameStateService gameStateService,
            IModellingJournal journal)
            : base (gameStateService, journal)
        {
            SpellId = (int)SpellIds.AscendedEruption;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.AscendedEruption);

            if (moreData == null)
                throw new ArgumentNullException("moreData");

            if (!moreData.ContainsKey("BoonOfTheAscended.BoonStacks"))
                throw new ArgumentOutOfRangeException("moreData", "Does not contain BoonOfTheAscended.BoonStacks");

            var numberOfBoonStacks = moreData["BoonOfTheAscended.BoonStacks"];

            var holyPriestAuraHealingBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;
            var holyPriestAuraDamageBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraDamageMultiplier").Value;

            // --Boon of the Ascended--
            // AE explodes at the end healing 3% more per stack to all friendlies (15y)

            decimal averageHeal = spellData.Coeff2
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus // This may not affect it? No way to test though.
                * holyPriestAuraDamageBonus; // ??? scales with the damage aura for reasons

            journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= gameStateService.GetCriticalStrikeMultiplier(gameState);

            var bonusPerStack = GetBoonBonusDamagePerStack(gameState, spellData, moreData);

            // Apply boon stack healing bonus
            averageHeal *= 1 + ((bonusPerStack / 100) * numberOfBoonStacks);

            // Apply 1/SQRT() scaling
            averageHeal *= 1 / (decimal)Math.Sqrt((double)GetNumberOfHealingTargets(gameState, spellData, moreData));

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData, moreData);
        }

        public override decimal GetAverageDamage(GameState gameState, BaseSpellData spellData = null, Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.AscendedEruption);

            if (moreData == null)
                throw new ArgumentNullException("moreData");

            if (!moreData.ContainsKey("BoonOfTheAscended.BoonStacks"))
                throw new ArgumentOutOfRangeException("moreData", "Does not contain BoonOfTheAscended.BoonStacks");

            var numberOfBoonStacks = moreData["BoonOfTheAscended.BoonStacks"];

            var holyPriestAuraDamageBonus = gameStateService.GetModifier(gameState, "HolyPriestAuraDamageMultiplier").Value;

            // --Boon of the Ascended--
            // AE explodes at the end healing 3% more per stack to all friendlies (15y)

            decimal averageHeal = spellData.Coeff1
                * gameStateService.GetIntellect(gameState)
                * gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamageBonus; // ??? scales with the damage aura for reasons

            journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= gameStateService.GetCriticalStrikeMultiplier(gameState);

            var bonusPerStack = GetBoonBonusDamagePerStack(gameState, spellData, moreData);

            // Apply boon stack damage bonus
            averageHeal *= 1 + ((bonusPerStack / 100) * numberOfBoonStacks);
            journal.Entry($"[{spellData.Name}] Stacks: {numberOfBoonStacks:0.##} Bonus/stack: {bonusPerStack:0.##}");

            // Apply 1/SQRT() scaling
            averageHeal *= 1 / (decimal)Math.Sqrt((double)GetNumberOfHealingTargets(gameState, spellData, moreData));

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData, moreData);
        }

        public override decimal GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null, Dictionary<string, decimal> moreData = null)
        {
            return GetMaximumCastsPerMinute(gameState, spellData, moreData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (moreData == null)
                throw new ArgumentNullException("moreData");

            if (!moreData.ContainsKey("BoonOfTheAscended.CPM"))
                throw new ArgumentOutOfRangeException("moreData", "Does not contain BoonOfTheAscended.CPM");

            var boonCPM = moreData["BoonOfTheAscended.CPM"];

            return boonCPM;
        }

        /// <summary>
        /// Get the bonus damage per stack of boon. It's stored as an int, 3 = 3% bonus damage per stack
        /// </summary>
        public decimal GetBoonBonusDamagePerStack(GameState gameState, BaseSpellData spellData = null,
           Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, SpellIds.AscendedEruption);

            // The bonus is stored as an int. 3 = 3%
            var bonusPerStack = spellData.Coeff3;

            if (gameStateService.IsConduitActive(gameState, Conduit.CourageousAscension))
            {
                var conduitData = gameStateService.GetConduitData(gameState, Conduit.CourageousAscension);

                bonusPerStack += conduitData.Coeff1;
            }

            return bonusPerStack;
        }
    }
}
