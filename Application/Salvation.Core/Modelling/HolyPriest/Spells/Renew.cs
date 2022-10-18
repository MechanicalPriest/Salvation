using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class Renew : SpellService, ISpellService<IRenewSpellService>
    {
        private readonly ISpellService<IEmpoweredRenewSpellService> _empoweredRenewSpellService;

        public Renew(IGameStateService gameStateService,
            ISpellService<IEmpoweredRenewSpellService> empoweredRenewSpellService)
            : base(gameStateService)
        {
            Spell = Spell.Renew;
            _empoweredRenewSpellService = empoweredRenewSpellService;
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
                * holyPriestAuraHealingBonus
                * GetRapidRecoveryHealingMultiplier(gameState);

            var journalAverageHealFirstTick = averageHealFirstTick;
            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Actual: {averageHealFirstTick:0.##} (first)");

            // Add the rest of the multipliers
            averageHealFirstTick *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            double duration = (spellData.Duration + GetRapidRecoveryDurationModifier(gameState)) / 1000;
            double tickrate = spellData.GetEffect(95).Amplitude / 1000;
            // HoT is affected by haste
            double averageHealTicks = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingPeriodicBonus
                * GetRapidRecoveryHealingMultiplier(gameState)
                * (duration / tickrate);

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

        public override double GetRenewUptime(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var groupSize = _gameStateService.GetPlaystyle(gameState, "GroupSize");

            if (groupSize == null)
                throw new ArgumentOutOfRangeException("GroupSize", $"GroupSize needs to be set.");

            var cpm = GetActualCastsPerMinute(gameState, spellData);
            var duration = GetDuration(gameState, spellData);
            var uptime = cpm * duration / groupSize.Value / 60;

            return uptime;
        }

        public override double GetRenewTicksPerMinute(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Number of base ticks
            double duration = (spellData.Duration + GetRapidRecoveryDurationModifier(gameState)) / 1000;
            double tickrate = spellData.GetEffect(95).Amplitude / 1000;

            var baseTicks = (duration / tickrate);

            // Add haste - and round up because we don't care if the tick is a partial or not.
            // The 1 is for the initial tick, which happens on cast. This initial tick isn't part of the bonus tick calculations though.
            var totalTicks = 1 + Math.Ceiling(baseTicks * _gameStateService.GetHasteMultiplier(gameState));

            return totalTicks * GetActualCastsPerMinute(gameState, spellData);
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            AveragedSpellCastResult result = base.GetCastResults(gameState, spellData);

            // Calculate Emp Renew if talented.
            if (_gameStateService.GetTalent(gameState, Spell.EmpoweredRenew).Rank > 0)
            {
                var empoweredRenewSpellData = _gameStateService.GetSpellData(gameState, Spell.EmpoweredRenew);

                empoweredRenewSpellData.Overrides[Override.CastsPerMinute] = GetActualCastsPerMinute(gameState, spellData);
                empoweredRenewSpellData.Overrides[Override.ResultMultiplier] = GetAverageRawHealing(gameState, spellData);

                // grab the result of the spell cast
                var empoweredRenewResult = _empoweredRenewSpellService.GetCastResults(gameState, empoweredRenewSpellData);

                result.AdditionalCasts.Add(empoweredRenewResult);
            }

            return result;
        }

        internal double GetRapidRecoveryHealingMultiplier(GameState gameState)
        {
            var multi = 1d;

            var talent = _gameStateService.GetTalent(gameState, Spell.RapidRecovery);

            if (talent != null && talent.Rank > 0)
            {
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.RapidRecovery);

                multi += talentSpellData.GetEffect(1028836).BaseValue / 100;
            }

            return multi;
        }

        internal double GetRapidRecoveryDurationModifier(GameState gameState)
        {
            var modifier = 0d;

            var talent = _gameStateService.GetTalent(gameState, Spell.RapidRecovery);

            if (talent != null && talent.Rank > 0)
            {
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.RapidRecovery);

                modifier += talentSpellData.GetEffect(1028837).BaseValue;
            }

            return modifier;
        }
    }
}
