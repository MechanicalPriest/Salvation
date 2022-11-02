using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class DivineStar : SpellService, ISpellService<IDivineStarSpellService>
    {
        public DivineStar(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.DivineStar;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            var divstarHealData = _gameStateService.GetSpellData(gameState, Spell.DivineStarHeal);
            var healingSp = divstarHealData.GetEffect(122873).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##} (per pass)");

            // Divine Star healing goes down after 6 targets
            var targetReductionNum = 6;
            var totalHealingDone = 0d;
            var numHealingTargets = GetNumberOfHealingTargets(gameState, spellData);

            for (var i = 1; i <= numHealingTargets; i++)
            {
                var healAmount = averageHeal * (1 / Math.Sqrt(Math.Max(0, i - targetReductionNum) + 1));
                totalHealingDone += healAmount;
                _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Targets: {i:##} Healing Total: {totalHealingDone:0.##} ({healAmount:0.##})", 25);
            }

            totalHealingDone *= 2 // Add the second pass-back through each target
                * _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Targets: {numHealingTargets:##} Healing Total: {totalHealingDone:0.##} (overall)");

            return totalHealingDone;
        }

        // TODO: Validate the damage doesn't scale
        public override double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraDamageBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest).GetEffect(191077).BaseValue / 100 + 1;

            var dStarDamageData = _gameStateService.GetSpellData(gameState, Spell.DivineStarDamage);

            var damageSp = dStarDamageData.GetEffect(153526).SpCoefficient;

            double averageDamage = damageSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraDamageBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip (Damage): {averageDamage:0.##}");

            averageDamage *= 2 // Add the second pass-back through each target
                * _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageDamage * GetNumberOfDamageTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            double maximumPotentialCasts = 60d / (hastedCastTime + hastedCd)
                + 1d / (fightLength / 60d);

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

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            // Divine Star Spellid doesnt have the "right" type, heal component does
            var healData = _gameStateService.GetSpellData(gameState, Spell.DivineStarHeal);

            return base.TriggersMastery(gameState, healData);
        }
    }
}
