﻿using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class PrayerOfHealing : SpellService, IPrayerOfHealingSpellService
    {
        public PrayerOfHealing(IGameStateService gameStateService,
            IModellingJournal journal)
            : base(gameStateService, journal)
        {
            SpellId = (int)Spell.PrayerOfHealing;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.PrayerOfHealing);

            var holyPriestAuraHealingBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            double averageHeal = spellData.Coeff1
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.PrayerOfHealing);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);

            double fillerCastTime = hastedCastTime == 0
                ? hastedGcd
                : hastedCastTime;

            double maximumPotentialCasts = 60d / fillerCastTime;

            return maximumPotentialCasts;
        }

        public override double GetNumberOfHealingTargets(GameState gameState, BaseSpellData spellData = null)
        {
            var numTargets = base.GetNumberOfHealingTargets(gameState, spellData);

            if (numTargets == 0)
            {
                // PoH stores its number of targets in 288930.BaseValue
                numTargets = spellData.GetEffect(288930).BaseValue;
            }                

            return numTargets;
        }
    }
}
