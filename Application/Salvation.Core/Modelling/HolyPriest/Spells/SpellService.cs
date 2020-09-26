using Salvation.Core.Constants;
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

        public virtual AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            AveragedSpellCastResult result = new AveragedSpellCastResult
            {
                SpellName = spellData.Name,
                SpellId = SpellId,

                CastsPerMinute = GetActualCastsPerMinute(gameState, spellData, moreData),
                CastTime = GetHastedCastTime(gameState, spellData, moreData),
                Cooldown = GetHastedCooldown(gameState, spellData, moreData),
                Damage = GetAverageDamage(gameState, spellData, moreData),
                Duration = GetDuration(gameState, spellData, moreData),
                Gcd = GetHastedGcd(gameState, spellData, moreData),
                Healing = GetAverageHealing(gameState, spellData, moreData),
                ManaCost = GetActualManaCost(gameState, spellData, moreData),
                MaximumCastsPerMinute = GetMaximumCastsPerMinute(gameState, spellData, moreData),
                NumberOfDamageTargets = GetNumberOfDamageTargets(gameState, spellData, moreData),
                NumberOfHealingTargets = GetNumberOfHealingTargets(gameState, spellData, moreData),
                Overhealing = GetAverageOverhealing(gameState, spellData, moreData),
                RawHealing = GetAverageRawHealing(gameState, spellData, moreData)
            };

            if (spellData.IsMasteryTriggered)
            {
                var echoResult = GetHolyPriestMasteryResult(gameState, spellData, moreData);
                if (echoResult != null)
                    result.AdditionalCasts.Add(echoResult);
            }

            return result;
        }

        public virtual decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            return 0m;
        }

        public virtual decimal GetAverageHealing(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            // Average healing done is raw healing * overheal
            var castProfile = _gameStateService.GetCastProfile(gameState, SpellId);

            var totalDirectHeal = GetAverageRawHealing(gameState, spellData, moreData)
                * (1 - castProfile.OverhealPercent);

            return totalDirectHeal;
        }

        public decimal GetAverageOverhealing(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            // Average healing done is raw healing * overheal
            var castProfile = _gameStateService.GetCastProfile(gameState, SpellId);

            var totalOverheal = GetAverageRawHealing(gameState, spellData, moreData)
                * castProfile.OverhealPercent;

            return totalOverheal;
        }

        public virtual decimal GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            var castProfile = _gameStateService.GetCastProfile(gameState, SpellId);

            decimal castsPerMinute = castProfile.Efficiency * GetMaximumCastsPerMinute(gameState, spellData, moreData);

            return castsPerMinute;
        }

        public virtual decimal GetAverageDamage(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            return 0m;
        }

        public virtual decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            throw new NotImplementedException();
        }

        public virtual decimal GetHastedCastTime(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            return spellData.IsCastTimeHasted ? spellData.BaseCastTime / _gameStateService.GetHasteMultiplier(gameState)
                : spellData.BaseCastTime;
        }

        public virtual decimal GetHastedGcd(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            return spellData.Gcd / _gameStateService.GetHasteMultiplier(gameState);
        }

        public virtual decimal GetHastedCooldown(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            return spellData.IsCooldownHasted
                ? spellData.BaseCooldown / _gameStateService.GetHasteMultiplier(gameState)
                : spellData.BaseCooldown;
        }

        public virtual decimal GetActualManaCost(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            var baseMana = _gameStateService.GetBaseManaAmount(gameState);

            return baseMana * spellData.ManaCost;
        }

        public virtual decimal GetDuration(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            return spellData.Duration;
        }

        public virtual decimal GetNumberOfHealingTargets(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            return spellData.NumberOfHealingTargets;
        }

        public virtual decimal GetNumberOfDamageTargets(GameState gameState, BaseSpellData spellData = null,
            Dictionary<string, decimal> moreData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            return spellData.NumberOfDamageTargets;
        }

        // This should probably be moved to another class/helper
        #region Holy Priest Specific

        /// <summary>
        /// This does NOT check to see if mastery applies to this spell
        /// </summary>
        public virtual AveragedSpellCastResult GetHolyPriestMasteryResult(GameState gameState, BaseSpellData spellData,
            Dictionary<string, decimal> moreData = null)
        {
            AveragedSpellCastResult result = new AveragedSpellCastResult();

            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            var averageMasteryHeal = GetAverageRawHealing(gameState, spellData, moreData)
                * (_gameStateService.GetMasteryMultiplier(gameState) - 1);

            var castProfile = _gameStateService.GetCastProfile(gameState, (int)SpellIds.EchoOfLight);

            result.SpellId = (int)SpellIds.EchoOfLight;
            result.SpellName = "Echo of Light";
            result.RawHealing = averageMasteryHeal;
            result.Healing = averageMasteryHeal * (1 - castProfile.OverhealPercent);

            return result;
        }

        #endregion
    }
}
