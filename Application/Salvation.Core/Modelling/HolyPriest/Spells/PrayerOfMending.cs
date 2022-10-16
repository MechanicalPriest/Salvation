using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class PrayerOfMending : SpellService, ISpellService<IPrayerOfMendingSpellService>, IPrayerOfMendingExtensions
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
            var numPoMStacks = GetAverageBounces(gameState, spellData);

            var pomFirstTargetHeal = averageHeal * GetFocusedMendingMultiplier(gameState, spellData);
            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {pomFirstTargetHeal:0.##} (first heal)");

            // Apply modifiers
            // Including Divine Service. Full value to first stack, average out the remaining stacks
            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState)
                * (1 + GetDivineServiceStackMultiplier(gameState) * (numPoMStacks - 1) / 2);

            pomFirstTargetHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState)
                * GetFocusedMendingMultiplier(gameState, spellData)
                * (1 + GetDivineServiceStackMultiplier(gameState) * numPoMStacks);

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
            var multi = 1d;

            var talent = _gameStateService.GetTalent(gameState, Spell.FocusedMending);

            if (talent != null && talent.Rank > 0)
            {
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.FocusedMending);

                multi += talentSpellData.GetEffect(996915).BaseValue / 100;
            }

            return multi;
        }

        internal double GetPrayersOfTheVirtuousModifier(GameState gameState)
        {
            var multi = 0d;

            var talent = _gameStateService.GetTalent(gameState, Spell.PrayersOfTheVirtuous);

            if (talent != null && talent.Rank > 0)
            {
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.PrayersOfTheVirtuous);

                multi += talentSpellData.GetEffect(1028179).BaseValue * talent.Rank;
            }

            return multi;
        }

        internal double GetSayYourPrayersBounceMultiplier(GameState gameState)
        {
            var multi = 1d;

            var talent = _gameStateService.GetTalent(gameState, Spell.SayYourPrayers);

            if (talent != null && talent.Rank > 0)
            {
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.SayYourPrayers);

                multi += talentSpellData.GetEffect(1028522).BaseValue / 100;
            }

            return multi;
        }

        internal double GetDivineServiceStackMultiplier(GameState gameState)
        {
            var multi = 0d;

            var talent = _gameStateService.GetTalent(gameState, Spell.DivineService);

            if (talent != null && talent.Rank > 0)
            {
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.DivineService);

                multi += talentSpellData.GetEffect(1028593).BaseValue / 100;
            }

            return multi;
        }

        public double GetAverageBounces(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var numPoMStacks = spellData.GetEffect(22870).BaseValue + GetPrayersOfTheVirtuousModifier(gameState);

            // Override used by Salvation to apply 2-stack PoMs
            if (spellData.Overrides.ContainsKey(Override.ResultMultiplier))
                numPoMStacks = spellData.Overrides[Override.ResultMultiplier];

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Actual: {numPoMStacks:0.##} (stacks)");

            // SYP is down here so it also affects Salv PoM's (Done above witih the ResultMultiplier override.
            numPoMStacks *= GetSayYourPrayersBounceMultiplier(gameState);
            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {numPoMStacks:0.##} (Avg stacks with SYP)");

            var percentageStacksExpired = _gameStateService.GetPlaystyle(gameState, "PoMPercentageStacksExpired");

            if (percentageStacksExpired == null)
                throw new ArgumentOutOfRangeException("PoMPercentageStacksExpired", $"PoMPercentageStacksExpired needs to be set.");

            return numPoMStacks * (1 - percentageStacksExpired.Value);
        }
    }
}
