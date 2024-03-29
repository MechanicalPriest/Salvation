﻿using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class HolyNova : SpellService, ISpellService<IHolyNovaSpellService>
    {
        public HolyNova(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.HolyNova;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            BaseSpellData spellDataRank2 = _gameStateService.GetSpellData(gameState, Spell.HolyNovaRank2);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            var healingSp = spellData.GetEffect(709210).TriggerSpell.GetEffect(739572).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            // Rank 2 stuff
            double threeTargetPercent = _gameStateService.GetPlaystyle(gameState, "HolyNovaPercentOfCastsOnThreeOrMore").Value;

            averageHeal *= 1 + threeTargetPercent * spellDataRank2.GetEffect(844508).BaseValue / 100;

            // Apply the relative square root scaling
            var numTargets = GetNumberOfHealingTargets(gameState, spellData);
            averageHeal *= GetTargetScaling(numTargets);

            return averageHeal * numTargets;
        }

        public override double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            BaseSpellData spellDataRank2 = _gameStateService.GetSpellData(gameState, Spell.HolyNovaRank2);

            var holyPriestAuraDamageBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest).GetEffect(191077).BaseValue / 100 + 1;

            var holyNovaDamageData = _gameStateService.GetSpellData(gameState, Spell.HolyNova);

            // 122128 = Holy Nova effect (following binding heal so far)
            var damageSp = holyNovaDamageData.GetEffect(170810).SpCoefficient;

            double averageDamage = damageSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamageBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip (Damage): {averageDamage:0.##}");

            // Rank 2 stuff
            double threeTargetPercent = _gameStateService.GetPlaystyle(gameState, "HolyNovaPercentOfCastsOnThreeOrMore").Value;

            averageDamage *= 1 + threeTargetPercent * spellDataRank2.GetEffect(844508).BaseValue / 100;

            averageDamage *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageDamage * GetTargetScaling(GetNumberOfDamageTargets(gameState, spellData));
        }

        internal double GetTargetScaling(double numTargets)
        {
            return (1 / Math.Sqrt(Math.Max(5, numTargets))) / (1 / Math.Sqrt(5));
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
            // TODO: Clamp to raid size?
            return double.MaxValue;
        }

        public override double GetMaximumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return double.MaxValue;
        }
    }
}
