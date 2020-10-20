﻿using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;
using System;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class SpellService : ISpellService
    {
        protected readonly IGameStateService _gameStateService;
        protected readonly IModellingJournal _journal;

        public virtual int SpellId { get; protected set; }

        public SpellService(IGameStateService gameStateService,
            IModellingJournal journal)
        {
            _gameStateService = gameStateService;
            _journal = journal;
            SpellId = 0;
        }

        public virtual AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (Spell)SpellId);

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
                NumberOfHealingTargets = (decimal)GetNumberOfHealingTargets(gameState, spellData),
                Overhealing = GetAverageOverhealing(gameState, spellData),
                RawHealing = GetAverageRawHealing(gameState, spellData)
            };

            if (spellData.IsMasteryTriggered)
            {
                var echoResult = GetHolyPriestMasteryResult(gameState, spellData);
                if (echoResult != null)
                    result.AdditionalCasts.Add(echoResult);
            }

            return result;
        }

        public virtual decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            return 0m;
        }

        public virtual decimal GetAverageHealing(GameState gameState, BaseSpellData spellData = null)
        {
            // Average healing done is raw healing * overheal
            var castProfile = _gameStateService.GetCastProfile(gameState, SpellId);

            var totalDirectHeal = GetAverageRawHealing(gameState, spellData)
                * (1 - castProfile.OverhealPercent);

            return totalDirectHeal;
        }

        public decimal GetAverageOverhealing(GameState gameState, BaseSpellData spellData = null)
        {
            // Average healing done is raw healing * overheal
            var castProfile = _gameStateService.GetCastProfile(gameState, SpellId);

            var totalOverheal = GetAverageRawHealing(gameState, spellData)
                * castProfile.OverhealPercent;

            return totalOverheal;
        }

        public virtual decimal GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (Spell)SpellId);

            var castProfile = _gameStateService.GetCastProfile(gameState, SpellId);

            decimal castsPerMinute = castProfile.Efficiency * GetMaximumCastsPerMinute(gameState, spellData);

            return castsPerMinute;
        }

        public virtual decimal GetAverageDamage(GameState gameState, BaseSpellData spellData = null)
        {
            return 0m;
        }

        public virtual decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            throw new NotImplementedException();
        }

        public virtual decimal GetHastedCastTime(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (Spell)SpellId);

            return spellData.IsCastTimeHasted ? spellData.BaseCastTime / _gameStateService.GetHasteMultiplier(gameState)
                : spellData.BaseCastTime;
        }

        public virtual decimal GetHastedGcd(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (Spell)SpellId);

            return spellData.Gcd / _gameStateService.GetHasteMultiplier(gameState);
        }

        public virtual decimal GetHastedCooldown(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (Spell)SpellId);

            return spellData.IsCooldownHasted
                ? spellData.BaseCooldown / _gameStateService.GetHasteMultiplier(gameState)
                : spellData.BaseCooldown;
        }

        public virtual decimal GetActualManaCost(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (Spell)SpellId);

            var baseMana = _gameStateService.GetBaseManaAmount(gameState);

            return baseMana * (decimal)spellData.ManaCost;
        }

        public virtual decimal GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (Spell)SpellId);

            return spellData.Duration;
        }

        public virtual double GetNumberOfHealingTargets(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData.Overrides.ContainsKey(Override.NumberOfHealingTargets))
                return spellData.Overrides[Override.NumberOfHealingTargets];
            return 0;
        }

        public virtual decimal GetNumberOfDamageTargets(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (Spell)SpellId);

            return spellData.NumberOfDamageTargets;
        }

        // This should probably be moved to another class/helper
        #region Holy Priest Specific

        /// <summary>
        /// This does NOT check to see if mastery applies to this spell
        /// </summary>
        public virtual AveragedSpellCastResult GetHolyPriestMasteryResult(GameState gameState, BaseSpellData spellData)
        {
            AveragedSpellCastResult result = new AveragedSpellCastResult();

            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (Spell)SpellId);

            var averageMasteryHeal = GetAverageRawHealing(gameState, spellData)
                * (_gameStateService.GetMasteryMultiplier(gameState) - 1);

            var castProfile = _gameStateService.GetCastProfile(gameState, (int)Spell.EchoOfLight);

            result.SpellId = (int)Spell.EchoOfLight;
            result.SpellName = "Echo of Light";
            result.RawHealing = averageMasteryHeal;
            result.Healing = averageMasteryHeal * (1 - castProfile.OverhealPercent);

            return result;
        }

        #endregion
    }
}
