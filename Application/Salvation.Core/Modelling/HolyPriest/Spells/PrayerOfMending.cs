using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class PrayerOfMending : SpellService, ISpellService<IPrayerOfMendingSpellService>
    {
        public PrayerOfMending(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.PrayerOfMending;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            var pomHealData = _gameStateService.GetSpellData(gameState, Spell.PrayerOfMendingHeal);

            var healingSp = pomHealData.GetEffect(22918).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##} (per stack)");

            // Number of initial PoM stacks
            var numPoMStacks = spellData.GetEffect(22870).BaseValue;

            // Override used by Salvation to apply 2-stack PoMs
            if (spellData.Overrides.ContainsKey(Override.ResultMultiplier))
                numPoMStacks = spellData.Overrides[Override.ResultMultiplier];

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Actual: {numPoMStacks:0.##} (stacks)");

            var pomFirstTargetHeal = averageHeal * GetFocusedMendingMultiplier(gameState, spellData);
            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {pomFirstTargetHeal:0.##} (first heal)");

            // Apply modifiers
            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            pomFirstTargetHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            // Apply healing to each PoM stack
            averageHeal = (averageHeal * (numPoMStacks - 1)) + pomFirstTargetHeal; 

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
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

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            // Prayer Of Healing Spellid doesnt have the "right" type, heal component does
            var healData = _gameStateService.GetSpellData(gameState, Spell.PrayerOfMendingHeal);

            return base.TriggersMastery(gameState, healData);
        }

        public override double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Override used by Salvation to apply 2-stack PoMs
            if (spellData.Overrides.ContainsKey(Override.CastsPerMinute))
                return spellData.Overrides[Override.CastsPerMinute];
            return base.GetActualCastsPerMinute(gameState, spellData);
        }

        internal double GetFocusedMendingMultiplier(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            //if (_gameStateService.IsConduitActive(gameState, Conduit.FocusedMending))
            //{
            //    var conduitData = _gameStateService.GetSpellData(gameState, Spell.FocusedMending);
            //    var rank = _gameStateService.GetConduitRank(gameState, Conduit.FocusedMending);

            //    var multiplier = 1 + (conduitData.ConduitRanks[rank] / 100);

            //    _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Applying FocusedMending ({(int)Conduit.FocusedMending}) conduit " +
            //        $"multiplier: {multiplier:0.##}");

            //    return multiplier;
            //}

            return 1;
        }
    }
}
