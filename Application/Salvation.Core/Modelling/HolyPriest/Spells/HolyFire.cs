﻿using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class HolyFire : SpellService, ISpellService<IHolyFireSpellService>
    {
        public HolyFire(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.HolyFire;
        }

        public override double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraDamagesBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(191077).BaseValue / 100 + 1;

            var holyPriestAuraDamagePeriodicBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(191078).BaseValue / 100 + 1;

            var damageSpInstant = spellData.GetEffect(6990).SpCoefficient;
            var damageSpPeriodic = spellData.GetEffect(6991).SpCoefficient;

            // Holy Fire's average dmg is initial + DoT portion:
            double averageDamageFirstTick = damageSpInstant
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamagesBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageDamageFirstTick:0.##} (first)");

            averageDamageFirstTick *= _gameStateService.GetCriticalStrikeMultiplier(gameState) * _gameStateService.GetHasteMultiplier(gameState);

            double tickrate = spellData.GetEffect(6991).Amplitude / 1000;

            // DoT is affected by haste
            double averageDmgTicks = damageSpPeriodic
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * _gameStateService.GetHasteMultiplier(gameState)
                * holyPriestAuraDamagePeriodicBonus
                * GetDuration(gameState, spellData) / tickrate;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageDmgTicks:0.##} (ticks)");

            averageDmgTicks *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return (averageDamageFirstTick + averageDmgTicks) * GetNumberOfDamageTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);

            // A fix to the spell being modified to have no cast time and no gcd and no CD
            // This can happen if it's a component in another spell
            if (hastedCastTime == 0 && hastedGcd == 0 && hastedCd == 0)
                return 0;

            double maximumPotentialCasts = 60d / (hastedCastTime + hastedCd);

            return maximumPotentialCasts;
        }

        public override double GetHastedCastTime(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Get the hasted cast time in seconds
            var hastedCastTime = spellData.BaseCastTime / 1000 / _gameStateService.GetHasteMultiplier(gameState);
            return hastedCastTime;
        }

        public override double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Spells are stored with duration in milliseconds. We want seconds.
            return spellData.Duration / 1000;
        }

        public override double GetMinimumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }
    }
}
