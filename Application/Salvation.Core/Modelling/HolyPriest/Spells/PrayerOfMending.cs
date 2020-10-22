using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class PrayerOfMending : SpellService, IPrayerOfMendingSpellService
    {
        public PrayerOfMending(IGameStateService gameStateService,
            IModellingJournal journal)
            : base(gameStateService, journal)
        {
            SpellId = (int)Spell.PrayerOfMending;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.PrayerOfMending);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue;

            var pomHealData = _gameStateService.GetSpellData(gameState, Spell.PrayerOfMendingHeal);

            var healingSp = pomHealData.GetEffect(22918).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

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
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.PrayerOfMending);

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

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }
    }
}
