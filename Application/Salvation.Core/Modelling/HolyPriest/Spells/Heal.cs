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
    public class Heal : SpellService, ISpellService<IHealSpellService>
    {
        public Heal(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.Heal;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            var healingSp = spellData.GetEffect(612).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            averageHeal *= GetResonantWordsMulti(gameState, spellData);

            averageHeal *= GetFlashConcentrationHealingModifier(gameState);

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

        internal double GetResonantWordsMulti(GameState gameState, BaseSpellData spellData)
        {
            // TODO: Move this to its own location rather than copy/pasted in Heal & FH
            var multi = 1d;

            // TODO: Clean up post-implementation
            //// If resonant words is active, attempt to get a value increasing heal (on average).
            //if (_gameStateService.IsConduitActive(gameState, Conduit.ResonantWords))
            //{
            //    // This is very much a hack, but including them directly causes a circular dependency error. 
            //    // This could be resolved in RW becomes its own effect/heal.
            //    var serenity = _gameStateService.GetRegisteredSpells(gameState).Where(s => s.Spell == Spell.HolyWordSerenity).FirstOrDefault();
            //    var sanc = _gameStateService.GetRegisteredSpells(gameState).Where(s => s.Spell == Spell.HolyWordSanctify).FirstOrDefault();
            //    var chastise = _gameStateService.GetRegisteredSpells(gameState).Where(s => s.Spell == Spell.HolyWordChastise).FirstOrDefault();

            //    var hwCasts = serenity.SpellService.GetActualCastsPerMinute(gameState, serenity.SpellData)
            //        + sanc.SpellService.GetActualCastsPerMinute(gameState, sanc.SpellData)
            //        + chastise.SpellService.GetActualCastsPerMinute(gameState, chastise.SpellData);

            //    var numberBuffsUsed = _gameStateService.GetPlaystyle(gameState, "ResonantWordsPercentageBuffsUsed");

            //    if(numberBuffsUsed == null)
            //        throw new ArgumentOutOfRangeException("ResonantWordsPercentageBuffsUsed", $"ResonantWordsPercentageBuffsUsed needs to be set.");

            //    var percentageBuffsForHeal = _gameStateService.GetPlaystyle(gameState, "ResonantWordsPercentageBuffsHeal");

            //    if (percentageBuffsForHeal == null)
            //        throw new ArgumentOutOfRangeException("ResonantWordsPercentageBuffsHeal", $"ResonantWordsPercentageBuffsHeal needs to be set.");

            //    // Finally grab the conduit multi
            //    var conduitData = _gameStateService.GetSpellData(gameState, Spell.ResonantWords);
            //    var conduitRank = _gameStateService.GetConduitRank(gameState, Conduit.ResonantWords);

            //    var conduitValue = conduitData.ConduitRanks[conduitRank] / 100;

            //    var numBuffedSpellsTotal = hwCasts * numberBuffsUsed.Value;

            //    // Max number of Heal spells that can be buffed: hwCasts * Heal_percent
            //    // Actual number of buffed casts: lowest of either max spells cast or actual casts per minute
            //    var numBuffedCasts = Math.Min(numBuffedSpellsTotal * percentageBuffsForHeal.Value, GetActualCastsPerMinute(gameState, spellData));

            //    // This is the extra healing on all the buffed casts
            //    var extraHealing = numBuffedCasts * conduitValue;

            //    // Now divide this by all casts
            //    var increase = extraHealing / GetActualCastsPerMinute(gameState, spellData);

            //    multi += increase;
            //}

            return multi;
        }

        public override double GetHastedCastTime(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var castTime = spellData.BaseCastTime / 1000;

            castTime -= GetFlashConcentrationCastTimeReduction(gameState);

            var hastedCastTime = castTime / _gameStateService.GetHasteMultiplier(gameState);

            return hastedCastTime;
        }

        internal double GetFlashConcentrationCastTimeReduction(GameState gameState)
        {
            var castTimeReduction = 0d;

            if(_gameStateService.IsLegendaryActive(gameState, Spell.FlashConcentration))
            {
                var fcSpellData = _gameStateService.GetSpellData(gameState, Spell.FlashConcentration);

                // This value comes through as -150 for -0.15s cast time
                var castTimeReductionPerStack = fcSpellData.GetEffect(833393).TriggerSpell.GetEffect(833395).BaseValue / 1000;

                var averageStacks = _gameStateService.GetPlaystyle(gameState, "FlashConcentrationAverageStacks");

                if (averageStacks == null)
                    throw new ArgumentOutOfRangeException("FlashConcentrationAverageStacks", $"FlashConcentrationAverageStacks needs to be set.");

                castTimeReduction += (castTimeReductionPerStack * averageStacks.Value * -1);
            }

            return castTimeReduction;
        }

        internal double GetFlashConcentrationHealingModifier(GameState gameState)
        {
            var modifier = 1d;

            if (_gameStateService.IsLegendaryActive(gameState, Spell.FlashConcentration))
            {
                var fcSpellData = _gameStateService.GetSpellData(gameState, Spell.FlashConcentration);

                // This comes through as 3 for 3%.
                var increasedHealingPerStack = fcSpellData.GetEffect(833393).TriggerSpell.GetEffect(833396).BaseValue / 100;

                var averageStacks = _gameStateService.GetPlaystyle(gameState, "FlashConcentrationAverageStacks");

                if (averageStacks == null)
                    throw new ArgumentOutOfRangeException("FlashConcentrationAverageStacks", $"FlashConcentrationAverageStacks needs to be set.");

                modifier += increasedHealingPerStack * averageStacks.Value;
            }

            return modifier;
        }
    }
}
