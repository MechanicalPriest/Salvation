using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;
using System.Linq;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class FlashHeal : SpellService, ISpellService<IFlashHealSpellService>
    {
        public FlashHeal(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.FlashHeal;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            // Flash Heal's average heal is:
            // SP% * Intellect * Vers * Hpriest Aura
            var healingSp = spellData.GetEffect(613).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState)
                * GetImprovedFlashHealMultiplier(gameState, spellData);

            // TODO: Cleanup post implementation
            // * GetResonantWordsMulti(gameState, spellData);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedGcd = GetHastedGcd(gameState, spellData);

            double fillerCastTime = hastedCastTime == 0
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

        internal double GetImprovedFlashHealMultiplier(GameState gameState, BaseSpellData spellData)
        {
            var multi = 1d;

            var talent = _gameStateService.GetTalent(gameState, Spell.ImprovedFlashHeal);

            if (talent != null && talent.Rank > 0)
            {
                var improvedFHSpellData = _gameStateService.GetSpellData(gameState, Spell.ImprovedFlashHeal);

                var increasedHealingAmount = improvedFHSpellData.GetEffect(1033063).BaseValue;

                multi += increasedHealingAmount / 100;
            }

            return multi;
        }

        // TODO: Cleanup post implementation
        //internal double GetResonantWordsMulti(GameState gameState, BaseSpellData spellData)
        //{
        //    // TODO: Move this to its own location rather than copy/pasted in Heal & FH
        //    var multi = 1d;

        //    // If resonant words is active, attempt to get a value increasing heal (on average).
        //    if (_gameStateService.IsConduitActive(gameState, Conduit.ResonantWords))
        //    {
        //        // This is very much a hack, but including them directly causes a circular dependency error. 
        //        // This could be resolved in RW becomes its own effect/heal.
        //        var serenity = _gameStateService.GetRegisteredSpells(gameState).Where(s => s.Spell == Spell.HolyWordSerenity).FirstOrDefault();
        //        var sanc = _gameStateService.GetRegisteredSpells(gameState).Where(s => s.Spell == Spell.HolyWordSanctify).FirstOrDefault();
        //        var chastise = _gameStateService.GetRegisteredSpells(gameState).Where(s => s.Spell == Spell.HolyWordChastise).FirstOrDefault();

        //        var hwCasts = serenity.SpellService.GetActualCastsPerMinute(gameState, serenity.SpellData)
        //            + sanc.SpellService.GetActualCastsPerMinute(gameState, sanc.SpellData)
        //            + chastise.SpellService.GetActualCastsPerMinute(gameState, chastise.SpellData);

        //        var numberBuffsUsed = _gameStateService.GetPlaystyle(gameState, "ResonantWordsPercentageBuffsUsed");

        //        if (numberBuffsUsed == null)
        //            throw new ArgumentOutOfRangeException("ResonantWordsPercentageBuffsUsed", $"ResonantWordsPercentageBuffsUsed needs to be set.");

        //        var percentageBuffsForHeal = _gameStateService.GetPlaystyle(gameState, "ResonantWordsPercentageBuffsHeal");

        //        if (percentageBuffsForHeal == null)
        //            throw new ArgumentOutOfRangeException("ResonantWordsPercentageBuffsHeal", $"ResonantWordsPercentageBuffsHeal needs to be set.");

        //        // Finally grab the conduit multi
        //        var conduitData = _gameStateService.GetSpellData(gameState, Spell.ResonantWords);
        //        var conduitRank = _gameStateService.GetConduitRank(gameState, Conduit.ResonantWords);

        //        var conduitValue = conduitData.ConduitRanks[conduitRank] / 100;

        //        var numBuffedSpellsTotal = hwCasts * numberBuffsUsed.Value;

        //        // Max number of Heal spells that can be buffed: hwCasts * Heal_percent
        //        // Actual number of buffed casts: lowest of either max spells cast or actual casts per minute
        //        var numBuffedCasts = Math.Min(numBuffedSpellsTotal * (1 - percentageBuffsForHeal.Value), GetActualCastsPerMinute(gameState, spellData));

        //        // This is the extra healing on all the buffed casts
        //        var extraHealing = numBuffedCasts * conduitValue;

        //        // Now divide this by all casts
        //        var increase = extraHealing / GetActualCastsPerMinute(gameState, spellData);

        //        multi += increase;
        //    }

        //    return multi;
        //}
    }
}
