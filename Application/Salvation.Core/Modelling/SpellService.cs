﻿using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;
using System;

namespace Salvation.Core.Modelling
{
    public class SpellService : ISpellService
    {
        // TODO: Move these variables somewhere that makes some more sense.
        /// <summary>
        /// Multiply this against coefficient to get scaled item spell values (flask buff)
        /// Comes from sc_scale_data.inc's __spell_scaling array (item section) for level 60.
        /// </summary>
        protected readonly double ItemCoefficientMultiplier = 95;
        /// <summary>
        /// Multiply this against coefficient to get scaled item spell values (potion buff)
        /// Comes from sc_scale_data.inc's __spell_scaling array (consumable section) for level 60.
        /// </summary>
        protected readonly double ConsumableCoefficientMultiplier = 25000;
        /// <summary>
        /// Badluck protection modifier for RPPM effects that generate buffs that could overlap
        /// </summary>
        protected readonly double RppmBadluckProtection = 1.13;
        /// <summary>
        /// This isn't ideal but it's better than trying to enforce player logic to be 60 only from inputs. 
        /// This will enable it to be easily refactored out later.
        /// </summary>
        protected readonly int PlayerLevel = 60;

        protected readonly IGameStateService _gameStateService;

        public virtual int SpellId { get { return (int)Spell; } }
        public virtual Spell Spell { get; protected set; }

        public SpellService(IGameStateService gameStateService)
        {
            _gameStateService = gameStateService;
            Spell = Spell.None;
        }

        public virtual AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            AveragedSpellCastResult result = new AveragedSpellCastResult
            {
                SpellName = spellData.Name,
                SpellId = SpellId,

                CastsPerMinute = GetActualCastsPerMinute(gameState, spellData),
                CastTime = GetHastedCastTime(gameState, spellData),
                Cooldown = GetHastedCooldown(gameState, spellData),
                Damage = GetAverageDamage(gameState, spellData),
                Duration = GetDuration(gameState, spellData),
                Gcd = GetHastedGcd(gameState, spellData),
                Healing = GetAverageHealing(gameState, spellData),
                ManaCost = GetActualManaCost(gameState, spellData),
                MaximumCastsPerMinute = GetMaximumCastsPerMinute(gameState, spellData),
                NumberOfDamageTargets = GetNumberOfDamageTargets(gameState, spellData),
                NumberOfHealingTargets = GetNumberOfHealingTargets(gameState, spellData),
                Overhealing = GetAverageOverhealing(gameState, spellData),
                RawHealing = GetAverageRawHealing(gameState, spellData),
                Mp5 = GetAverageMp5(gameState, spellData),
            };

            // Add mastery if it triggers mastery
            if (TriggersMastery(gameState, spellData))
            {
                var echoResult = GetHolyPriestMasteryResult(gameState, spellData);
                if (echoResult != null)
                    result.AdditionalCasts.Add(echoResult);
            }

            // Add leech if there is any
            var leechResult = GetLeechResult(gameState, spellData);

            if(leechResult != null)
                result.AdditionalCasts.Add(leechResult);

            return result;
        }

        public virtual double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            return 0;
        }

        public virtual double GetAverageHealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Average healing done is raw healing * overheal
            var castProfile = _gameStateService.GetSpellCastProfile(gameState, SpellId);

            var totalDirectHeal = GetAverageRawHealing(gameState, spellData);

            if (castProfile != null)
                totalDirectHeal *= (1 - castProfile.OverhealPercent);

            return totalDirectHeal;
        }

        public virtual double GetAverageOverhealing(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Average healing done is raw healing * overheal
            var castProfile = _gameStateService.GetSpellCastProfile(gameState, SpellId);

            var totalOverheal = 0d;

            if (castProfile != null)
                totalOverheal = GetAverageRawHealing(gameState, spellData) * castProfile.OverhealPercent;

            return totalOverheal;
        }

        public virtual double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var castProfile = _gameStateService.GetSpellCastProfile(gameState, SpellId);

            double castsPerMinute = GetMaximumCastsPerMinute(gameState, spellData);

            if (castProfile != null)
                castsPerMinute *= castProfile.Efficiency;

            return castsPerMinute;
        }

        public virtual double GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            return 0;
        }

        public virtual double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            return 0;
        }

        public virtual double GetHastedCastTime(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Get the hasted cast time in seconds
            var hastedCastTime = spellData.BaseCastTime / 1000 / _gameStateService.GetHasteMultiplier(gameState);
            return hastedCastTime;
        }

        public virtual double GetHastedGcd(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Can't be lower than GCDFloor (0.75)
            return Math.Max(spellData.Gcd / _gameStateService.GetHasteMultiplier(gameState), _gameStateService.GetGCDFloor(gameState));
        }

        public virtual double GetHastedCooldown(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var baseCooldown = spellData.BaseCooldown / 1000d;

            return spellData.IsCooldownHasted
                ? baseCooldown / _gameStateService.GetHasteMultiplier(gameState)
                : baseCooldown;
        }

        public virtual double GetActualManaCost(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var baseMana = _gameStateService.GetBaseManaAmount(gameState);

            return baseMana * (spellData.ManaCost / 100);
        }

        public virtual double GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // Spells are stored with duration in milliseconds. We want seconds.
            return spellData.Duration / 1000;
        }

        /// <summary>
        /// Gets the number of healing targets the spell will hit by checking spelldata overrides
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="spellData"></param>
        /// <returns></returns>
        public virtual double GetNumberOfHealingTargets(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var profileData = _gameStateService.GetSpellCastProfile(gameState, SpellId);

            var numTargets = profileData == null ? 0 : profileData.AverageHealingTargets;

            if (spellData.Overrides.ContainsKey(Override.NumberOfHealingTargets))
                numTargets = spellData.Overrides[Override.NumberOfHealingTargets];

            return numTargets;
        }

        public virtual double GetNumberOfDamageTargets(GameState gameState, BaseSpellData spellData = null)
        {
            spellData = ValidateSpellData(gameState, spellData);

            var profileData = _gameStateService.GetSpellCastProfile(gameState, SpellId);

            var numTargets = profileData == null ? 0 : profileData.AverageDamageTargets;

            if (spellData.Overrides.ContainsKey(Override.NumberOfDamageTargets))
                numTargets = spellData.Overrides[Override.NumberOfDamageTargets];

            return numTargets;
        }

        public virtual double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetMinimumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetMaximumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        internal virtual BaseSpellData ValidateSpellData(GameState gameState, BaseSpellData spellData)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell);

            if (spellData == null)
                throw new ArgumentOutOfRangeException(nameof(SpellId),
                    $"Spelldata for SpellId ({SpellId}) not found: {Spell}");

            return spellData;
        }

        #region Spell Effect 


        /// <summary>
        /// Uptime as a percentage. 1.0 = 100%
        /// </summary>
        public virtual double GetUptime(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageIntellect(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageIntellectBonus(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageCriticalStrike(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageCriticalStrikePercent(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageHaste(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageHastePercent(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageMastery(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageMasteryPercent(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageVersatility(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageVersatilityPercent(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageLeech(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageLeechPercent(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageMp5(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        public virtual double GetAverageHealingBonus(GameState gameState, BaseSpellData spellData)
        {
            return 0;
        }

        #endregion

        #region Leech
        /// <summary>
        /// Calculates a spell cast result for leech for this spell
        /// </summary>
        public virtual AveragedSpellCastResult GetLeechResult(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            // If the spell doesn't trigger leech, or the total damage/healing is 0 then return null

            // Remove self healing as it doesn't generate leech
            double selfHealingPercent = 0;

            // Use the selfheal percent if it's set. Otherwise 0% self healing
            var selfHealingPlaystyle = _gameStateService.GetPlaystyle(gameState, "LeechSelfHealPercent");

            if (selfHealingPlaystyle != null)
                selfHealingPercent = selfHealingPlaystyle.Value;

            var totalNonSelfHealing = GetAverageRawHealing(gameState, spellData) * (1 - selfHealingPercent);

            // Add damage + healing together as both generate leech
            var totalDamageHealing = totalNonSelfHealing
                + GetAverageDamage(gameState, spellData);

            // Don't return a result if there is nothing generating leech
            if (!TriggersLeech(gameState, spellData)
                || totalDamageHealing == 0
                || _gameStateService.GetLeechMultiplier(gameState) == 1)
                return null;

            AveragedSpellCastResult result = new AveragedSpellCastResult();

            var averageLeechHeal = totalDamageHealing
                * (_gameStateService.GetLeechMultiplier(gameState) - 1);

            var castProfile = _gameStateService.GetSpellCastProfile(gameState, (int)Spell.LeechHeal);

            double overhealPercent = 0;
            if (castProfile != null)
                overhealPercent = castProfile.OverhealPercent;

            result.SpellId = (int)Spell.LeechHeal;
            result.SpellName = "Leech";
            result.RawHealing = averageLeechHeal;
            result.Healing = averageLeechHeal * (1 - overhealPercent);
            result.Overhealing = averageLeechHeal * overhealPercent;
            result.CastsPerMinute = GetActualCastsPerMinute(gameState, spellData);
            result.NumberOfHealingTargets = 1;

            return result;
        }

        /// <summary>
        /// Defaults to True. Used to override if something doesn't trigger leech
        /// </summary>
        public virtual bool TriggersLeech(GameState gameState, BaseSpellData spellData)
        {
            return true;
        }

        #endregion

        // This should probably be moved to another class/helper
        #region Holy Priest Specific

        /// <summary>
        /// This does NOT check to see if mastery applies to this spell
        /// </summary>
        public virtual AveragedSpellCastResult GetHolyPriestMasteryResult(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            AveragedSpellCastResult result = new AveragedSpellCastResult();

            var averageMasteryHeal = GetAverageRawHealing(gameState, spellData)
                * (_gameStateService.GetMasteryMultiplier(gameState) - 1);

            var castProfile = _gameStateService.GetSpellCastProfile(gameState, (int)Spell.EchoOfLight);

            result.SpellId = (int)Spell.EchoOfLight;
            result.SpellName = "Echo of Light";
            result.RawHealing = averageMasteryHeal;
            result.Healing = averageMasteryHeal * (1 - castProfile.OverhealPercent);
            result.Overhealing = averageMasteryHeal * castProfile.OverhealPercent;
            result.CastsPerMinute = GetActualCastsPerMinute(gameState, spellData);
            result.NumberOfHealingTargets = GetNumberOfHealingTargets(gameState, spellData);

            return result;
        }

        public virtual bool TriggersMastery(GameState gameState, BaseSpellData spellData)
        {
            spellData = ValidateSpellData(gameState, spellData);

            foreach (var effect in spellData.Effects)
            {
                if (effect.Type == 10)
                {
                    return true;
                }

                if (effect.TriggerSpell != null)
                {
                    foreach (var triggerSpellEffect in effect.TriggerSpell.Effects)
                    {
                        if (triggerSpellEffect.Type == 10)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        #endregion
    }
}
