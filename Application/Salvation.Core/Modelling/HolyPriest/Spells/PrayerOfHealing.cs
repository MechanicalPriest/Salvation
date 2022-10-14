﻿using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class PrayerOfHealing : SpellService, ISpellService<IPrayerOfHealingSpellService>
    {
        public PrayerOfHealing(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.PrayerOfHealing;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;
            var healingSp = spellData.GetEffect(105332).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            // At least one target healed gets bonus healing from Prayerful Litany
            var averageFirstHeal = averageHeal
                * GetPrayerfulLitanyMultiplier(gameState);

            return averageFirstHeal + averageHeal * Math.Max(GetNumberOfHealingTargets(gameState, spellData) - 1, 0);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);

            double fillerCastTime = hastedCastTime == 0
                ? hastedGcd
                : hastedCastTime;

            double maximumPotentialCasts = 60d / fillerCastTime;

            return maximumPotentialCasts;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // PoH stores its number of targets in 288930.BaseValue
            var numTargets = spellData.GetEffect(288930).BaseValue;

            return numTargets;
        }

        public override double GetHastedCastTime(GameState gameState, BaseSpellData spellData = null)
        {
            var hastedCT = base.GetHastedCastTime(gameState, spellData);

            hastedCT *= this.GetUnwaveringWillMultiplier(gameState);

            return hastedCT;
        }

        internal double GetPrayerfulLitanyMultiplier(GameState gameState)
        {
            var multi = 1d;

            var talent = _gameStateService.GetTalent(gameState, Spell.PrayerfulLitany);

            if (talent != null && talent.Rank > 0)
            {
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.PrayerfulLitany);

                multi += talentSpellData.GetEffect(1028559).BaseValue / 100;
            }

            return multi;
        }
    }
}
