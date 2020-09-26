using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class AscendedNova : SpellService, IAscendedNovaSpellService
    {
        public AscendedNova(IGameStateService gameStateService,
            IModellingJournal journal)
            : base(gameStateService, journal)
        {
            SpellId = (int)SpellIds.AscendedNova;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null, Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, SpellIds.AscendedNova);

            var holyPriestAuraHealingBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            decimal averageHeal = spellData.Coeff2
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            // Apply the 1/sqrt() scaling based on no. targets
            averageHeal *= 1 / (decimal)Math.Sqrt((double)GetNumberOfHealingTargets(gameState, spellData, moreData));

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData, moreData);
        }

        public override decimal GetAverageDamage(GameState gameState, BaseSpellData spellData = null, Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, SpellIds.AscendedNova);

            var holyPriestAuraDamageBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraDamageMultiplier").Value;

            decimal averageDamage = spellData.Coeff1
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamageBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip (Dmg): {averageDamage:0.##}");

            averageDamage *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            averageDamage *= 1 / (decimal)Math.Sqrt((double)GetNumberOfDamageTargets(gameState, spellData, moreData));

            return averageDamage * GetNumberOfDamageTargets(gameState, spellData, moreData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null, Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, SpellIds.AscendedNova);

            if (moreData == null)
                throw new ArgumentNullException("moreData");

            if (!moreData.ContainsKey("BoonOfTheAscended.CPM"))
                throw new ArgumentOutOfRangeException("moreData", "Does not contain BoonOfTheAscended.CPM");

            var boonCPM = moreData["BoonOfTheAscended.CPM"];

            if (!moreData.ContainsKey("BoonOfTheAscended.LeftoverDuration"))
                throw new ArgumentOutOfRangeException("moreData", "Does not contain BoonOfTheAscended.Duration");

            var allowedDuration = moreData["BoonOfTheAscended.LeftoverDuration"];

            var hastedGcd = GetHastedGcd(gameState, spellData, moreData);

            // Max casts is whatever time we have available multiplied by efficiency
            decimal maximumPotentialCasts = allowedDuration / hastedGcd;

            // This is the maximum potential casts per Boon CD
            maximumPotentialCasts = maximumPotentialCasts * boonCPM;

            return maximumPotentialCasts;
        }
    }
}
