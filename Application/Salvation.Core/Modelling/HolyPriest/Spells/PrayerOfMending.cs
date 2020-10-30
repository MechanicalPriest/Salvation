using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class PrayerOfMending : SpellService, IPrayerOfMendingSpellService
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

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            // Number of initial PoM stacks
            var numPoMStacks = spellData.GetEffect(22870).BaseValue;

            // Override used by Salvation to apply 2-stack PoMs
            if (spellData.Overrides.ContainsKey(Override.ResultMultiplier))
                numPoMStacks = spellData.Overrides[Override.ResultMultiplier];

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * numPoMStacks;

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

        public override double GetHastedCastTime(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Apply PoM rank 2 to reduce the cast time by 100%.
            var pomRank2 = _gameStateService.GetSpellData(gameState, Spell.PrayerOfMendingRank2);

            var castTimeMulti = pomRank2.GetEffect(806684).BaseValue;

            spellData.BaseCastTime *= (1d + castTimeMulti / 100d);

            return base.GetHastedCastTime(gameState, spellData);
        }

        public override bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            // Prayer Of Healing Spellid doesnt have the "right" type, heal component does
            var healData = _gameStateService.GetSpellData(gameState, Spell.PrayerOfMendingHeal);

            return base.TriggersMastery(gameState, healData);
        }
    }
}
