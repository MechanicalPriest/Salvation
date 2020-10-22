using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class AscendedEruption : SpellService, IAscendedEruptionSpellService
    {
        public AscendedEruption(IGameStateService gameStateService)
            : base(gameStateService)
        {
            SpellId = (int)Spell.AscendedEruption;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.AscendedEruption);

            // Default to the 1 stack we automatically get if not provided.
            var numberOfBoonStacks = 1d;

            if (spellData.Overrides.ContainsKey(Override.ResultMultiplier))
                numberOfBoonStacks = spellData.Overrides[Override.ResultMultiplier];

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;
            var holyPriestAuraDamageBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(191077).BaseValue / 100 + 1;

            // --Boon of the Ascended--
            // AE explodes at the end healing 3% more per stack to all friendlies (15y)
            var healingSp = spellData.GetEffect(815532).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus // This may not affect it? No way to test though.
                * holyPriestAuraDamageBonus; // ??? scales with the damage aura for reasons

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            var bonusPerStack = GetBoonBonusPerStack(gameState);

            // Apply boon stack healing bonus
            averageHeal *= 1 + ((bonusPerStack / 100d) * numberOfBoonStacks);

            // Apply 1/SQRT() scaling
            // Healing scales down with the number of enemy + friendly targets, see Issue #24
            var numTargetReduction = GetNumberOfHealingTargets(gameState, spellData) + GetNumberOfDamageTargets(gameState, spellData);
            averageHeal *= 1d / Math.Sqrt(numTargetReduction);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.AscendedEruption);

            // Default to the 1 stack we automatically get if not provided.
            var numberOfBoonStacks = 1d;

            if (spellData.Overrides.ContainsKey(Override.ResultMultiplier))
                numberOfBoonStacks = spellData.Overrides[Override.ResultMultiplier];

            var holyPriestAuraDamageBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(191077).BaseValue / 100 + 1;
            // --Boon of the Ascended--
            // AE explodes at the end healing 3% more per stack to all friendlies (15y)
            var damageSp = spellData.GetEffect(815531).SpCoefficient;

            double averageHeal = damageSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamageBonus; // ??? scales with the damage aura for reasons

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            var bonusPerStack = GetBoonBonusPerStack(gameState);

            // Apply boon stack damage bonus
            averageHeal *= 1d + ((bonusPerStack / 100d) * numberOfBoonStacks);
            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Stacks: {numberOfBoonStacks:0.##} Bonus/stack: {bonusPerStack:0.##}");

            // Apply 1/SQRT() 
            // Damage scales down with the number of enemy + friendly targets, see Issue #52
            var numTargetReduction = GetNumberOfHealingTargets(gameState, spellData) + GetNumberOfDamageTargets(gameState, spellData);
            averageHeal *= 1d / Math.Sqrt(numTargetReduction);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            return GetMaximumCastsPerMinute(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            return 0d;
        }

        /// <summary>
        /// Get the bonus damage per stack of boon. It's stored as an int, 3 = 3% bonus damage per stack
        /// </summary>
        public double GetBoonBonusPerStack(GameState gameState)
        {
            // The bonus is stored as an int. 3 = 3%
            var boonSpellData = _gameStateService.GetSpellData(gameState, Spell.BoonOfTheAscended);
            var bonusPerStack = boonSpellData.GetEffect(815475).BaseValue;

            if (_gameStateService.IsConduitActive(gameState, Conduit.CourageousAscension))
            {
                var conduitData = _gameStateService.GetSpellData(gameState, Spell.CourageousAscension);

                bonusPerStack += conduitData.GetEffect(842371).BaseValue;
            }

            return bonusPerStack;
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
