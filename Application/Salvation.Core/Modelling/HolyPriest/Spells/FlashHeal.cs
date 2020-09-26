using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class FlashHeal : SpellService, IFlashHealSpellService
    {
        public FlashHeal(IGameStateService gameStateService,
            IModellingJournal journal)
            : base(gameStateService, journal)
        {
            SpellId = (int)SpellIds.FlashHeal;
        }

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, SpellIds.FlashHeal);

            var holyPriestAuraHealingBonus = _gameStateService.GetModifier(gameState, "HolyPriestAuraHealingMultiplier").Value;

            // Flash Heal's average heal is:
            // SP% * Intellect * Vers * Hpriest Aura
            decimal averageHeal = spellData.Coeff1
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _journal.Entry($"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData, moreData);
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, SpellIds.FlashHeal);

            var hastedCastTime = GetHastedCastTime(gameState, spellData, moreData);
            var hastedGcd = GetHastedGcd(gameState, spellData, moreData);

            decimal fillerCastTime = hastedCastTime == 0
                ? hastedGcd
                : hastedCastTime;

            decimal maximumPotentialCasts = 60m / fillerCastTime;

            return maximumPotentialCasts;
        }
    }
}
