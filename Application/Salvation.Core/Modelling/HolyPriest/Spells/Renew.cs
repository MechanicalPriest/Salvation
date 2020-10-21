using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class Renew : SpellService, IRenewSpellService
    {
        public Renew(IGameStateService gameStateService,
            IModellingJournal journal)
            : base(gameStateService, journal)
        {
            SpellId = (int)Spell.Renew;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.Renew);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue;

            var holyPriestAuraHealingPeriodicBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(191076).BaseValue;

            var healingSp = spellData.GetEffect(95).SpCoefficient;

            // Renews's average heal is initial + HoT portion:
            double averageHealFirstTick = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHealFirstTick:0.##} (first)");

            averageHealFirstTick *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetHasteMultiplier(gameState);


            // HoT is affected by haste
            double averageHealTicks = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * _gameStateService.GetHasteMultiplier(gameState)
                * holyPriestAuraHealingPeriodicBonus
                * 5;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHealTicks:0.##} (ticks)");

            averageHealTicks *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return (averageHealFirstTick + averageHealTicks) * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.Renew);

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

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }
    }
}
