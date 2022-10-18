using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class CircleOfHealing : SpellService, ISpellService<ICircleOfHealingSpellService>
    {
        public CircleOfHealing(IGameStateService gameStateService)
            : base(gameStateService)
        {
            Spell = Spell.CircleOfHealing;
        }

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var holyPriestAuraHealingBonus = _gameStateService.GetSpellData(gameState, Spell.HolyPriest)
                .GetEffect(179715).BaseValue / 100 + 1;

            var healingSp = spellData.GetEffect(302436).SpCoefficient;

            double averageHeal = healingSp
                * _gameStateService.GetIntellect(gameState)
                * _gameStateService.GetVersatilityMultiplier(gameState)
                * holyPriestAuraHealingBonus;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Tooltip: {averageHeal:0.##}");

            averageHeal *= _gameStateService.GetCriticalStrikeMultiplier(gameState)
                * _gameStateService.GetGlobalHealingMultiplier(gameState);

            averageHeal *= GetHealingChorusModifier(gameState);

            return averageHeal * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var hastedCastTime = GetHastedCastTime(gameState, spellData);
            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = _gameStateService.GetFightLength(gameState);

            double maximumPotentialCasts = 60d / (hastedCastTime + hastedCd)
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // CoH stores its number of targets in 302437.BaseValue
            var numTargets = spellData.GetEffect(302437).BaseValue;

            return numTargets + GetOrisonTargetModifier(gameState);
        }

        public override double GetHastedCooldown(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // The Orison cooldown reduction is applied before haste.
            spellData.Overrides[Override.BaseCooldownModifier] = GetOrisonCooldownModifier(gameState);

            return base.GetHastedCooldown(gameState, spellData);
        }

        public override double GetNumberOfHealingTargets(GameState gameState, BaseSpellData spellData = null)
        {
            return base.GetNumberOfHealingTargets(gameState, spellData) + GetOrisonTargetModifier(gameState);
        }

        internal double GetOrisonCooldownModifier(GameState gameState)
        {
            var modifier = 0d;

            var talent = _gameStateService.GetTalent(gameState, Spell.Orison);

            if (talent != null && talent.Rank > 0)
            {
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.Orison);

                modifier += talentSpellData.GetEffect(1028118).BaseValue;
            }

            return modifier;
        }

        internal double GetOrisonTargetModifier(GameState gameState)
        {
            var modifier = 0d;

            var talent = _gameStateService.GetTalent(gameState, Spell.Orison);

            if (talent != null && talent.Rank > 0)
            {
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.Orison);

                modifier += talentSpellData.GetEffect(1028117).BaseValue;
            }

            return modifier;
        }

        internal double GetHealingChorusModifier(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var modifier = 1d;

            var talent = _gameStateService.GetTalent(gameState, Spell.HealingChorus);

            if (talent != null && talent.Rank > 0)
            {
                var talentSpellData = _gameStateService.GetSpellData(gameState, Spell.HealingChorus);

                var healingPerStack = talentSpellData.GetEffect(1028019).TriggerSpell.GetEffect(1028027).BaseValue / 100;

                var stacksPerMinute = _gameStateService.GetRenewTicksPerMinute(gameState);

                var stacksWastedPerMinute = _gameStateService.GetPlaystyle(gameState, "HealingChorusStacksWastedPerMinute");

                if (stacksWastedPerMinute == null)
                    throw new ArgumentOutOfRangeException("HealingChorusStacksWastedPerMinute", $"HealingChorusStacksWastedPerMinute needs to be set.");

                _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] Raw chorus stacks/min: {stacksPerMinute:N3} Wasted chorus stacks/min: {stacksWastedPerMinute}");

                stacksPerMinute -= stacksWastedPerMinute.Value;

                modifier += healingPerStack * stacksPerMinute / GetActualCastsPerMinute(gameState);
            }

            return modifier;
        }
    }
}
