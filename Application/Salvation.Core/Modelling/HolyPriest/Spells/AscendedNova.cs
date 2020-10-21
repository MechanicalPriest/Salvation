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
            SpellId = (int)Spell.AscendedNova;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.AscendedNova);

            var holyPriestAuraHealingBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            double averageHeal = spellData.Coeff2
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            // Apply the 1/sqrt() scaling based on no. targets
            averageHeal *= 1 / Math.Sqrt(GetNumberOfHealingTargets(gameState, spellData));

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.AscendedNova);

            var holyPriestAuraDamageBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraDamageMultiplier").Value;

            double averageDamage = spellData.Coeff1
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamageBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip (Dmg): {averageDamage:0.##}");

            averageDamage *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            averageDamage *= 1 / Math.Sqrt(GetNumberOfDamageTargets(gameState, spellData));

            return averageDamage * GetNumberOfDamageTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.AscendedNova);

            if (!spellData.Overrides.ContainsKey(Override.CastsPerMinute))
                throw new ArgumentOutOfRangeException("Overrides", "Does not contain CastsPerMinute");

            var boonCPM = spellData.Overrides[Override.CastsPerMinute];

            if (!spellData.Overrides.ContainsKey(Override.AllowedDuration))
                throw new ArgumentOutOfRangeException("moreData", "Does not contain AllowedDuration");

            var allowedDuration = spellData.Overrides[Override.AllowedDuration];

            var hastedGcd = GetHastedGcd(gameState, spellData);

            // Max casts is whatever time we have available multiplied by efficiency
            double maximumPotentialCasts = allowedDuration / hastedGcd;

            // This is the maximum potential casts per Boon CD
            maximumPotentialCasts = maximumPotentialCasts * boonCPM;

            return maximumPotentialCasts;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            // AN stores its max heal targets in the trigger spells effect
            var numTargets = spellData.GetEffect(815031)
                    .TriggerSpell.GetEffect(844031).BaseValue;

            return numTargets;
        }

        public override double GetMaximumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return double.MaxValue;
        }
    }
}
