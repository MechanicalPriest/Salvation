using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class Renew : SpellService, ISpellService<IRenewSpellService>
    {
        public Renew(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.Renew;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            var holyPriestAuraHealingPeriodicBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(191076).BaseValue / 100 + 1;

            var healingSp = spellData.GetEffect(95).SpCoefficient;

            // This is broken up a bit for the sake of logging.
            // Renews's average heal is initial + HoT portion:
            double averageHealFirstTick = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            var journalAverageHealFirstTick = averageHealFirstTick;
            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Actual: {averageHealFirstTick:0.##} (first)");

            // Add the rest of the multipliers
            averageHealFirstTick *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            double duration = spellData.Duration / 1000;
            double tickrate = spellData.GetEffect(95).Amplitude / 1000;
            // HoT is affected by haste
            double averageHealTicks = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingPeriodicBonus
                * duration / tickrate;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Actual: {averageHealTicks:0.##} (ticks total)");
            
            // This just adds extra partial ticks.
            averageHealTicks *= _gameStateService.GetHasteMultiplier(gameState);

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Actual: {averageHealTicks / journalAverageHealFirstTick:0.##} (num ticks)");
            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Actual: {averageHealTicks % journalAverageHealFirstTick:0.##} (partial tick)");
            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {journalAverageHealFirstTick + averageHealTicks:0.##} (total)");

            // Add the rest of the multipliers
            averageHealTicks *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            return (averageHealFirstTick + averageHealTicks) * GetNumberOfHealingTargets(gameState, spellData);
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

            double fillerCastTime = hastedCastTime == 0d
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
            return 1;
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Override used by Salvation to apply 2-stack PoMs
            if (spellData.Overrides.ContainsKey(Override.CastsPerMinute))
                return spellData.Overrides[Override.CastsPerMinute];
            return base.GetActualCastsPerMinute(gameState, spellData);
        }
    }
}
