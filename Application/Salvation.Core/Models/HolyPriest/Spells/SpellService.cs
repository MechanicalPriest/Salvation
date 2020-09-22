using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Models.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest.Spells
{
    public class SpellService : ISpellService
    {
        protected readonly IGameStateService gameStateService;

        public virtual int SpellId { get; protected set; }

        public SpellService(IGameStateService gameStateService)
        {
            this.gameStateService = gameStateService;
            SpellId = 0;
        }

        public virtual AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            AveragedSpellCastResult result = new AveragedSpellCastResult();

            result.SpellName = spellData.Name;
            result.SpellId = SpellId;

            result.CastsPerMinute = GetActualCastsPerMinute(gameState, spellData);
            result.CastTime = GetHastedCastTime(gameState, spellData);
            result.Cooldown = GetHastedCooldown(gameState, spellData);
            result.Damage = GetAverageDamage(gameState, spellData);
            result.Duration = GetDuration(gameState, spellData);
            result.Gcd = GetHastedGcd(gameState, spellData);
            result.Healing = GetAverageHealing(gameState, spellData);
            result.ManaCost = GetActualManaCost(gameState, spellData);
            result.MaximumCastsPerMinute = GetMaximumCastsPerMinute(gameState, spellData);
            result.NumberOfDamageTargets = GetNumberOfDamageTargets(gameState, spellData);
            result.NumberOfHealingTargets = GetNumberOfHealingTargets(gameState, spellData);
            result.RawHealing = GetAverageRawHealing(gameState, spellData);

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
            var castProfile = gameStateService.GetCastProfile(gameState, SpellId);

            var totalDirectHeal = GetAverageRawHealing(gameState, spellData) 
                * (1 - castProfile.OverhealPercent);

            return totalDirectHeal;
        }

        public virtual decimal GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            var castProfile = gameStateService.GetCastProfile(gameState, SpellId);

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
                spellData = gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            return spellData.IsCastTimeHasted ? spellData.BaseCastTime / gameStateService.GetHasteMultiplier(gameState)
                : spellData.BaseCastTime;
        }

        public virtual decimal GetHastedGcd(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            return spellData.Gcd / gameStateService.GetHasteMultiplier(gameState);
        }

        public virtual decimal GetHastedCooldown(GameState gameState, BaseSpellData spellData = null)
        {
            if(spellData == null)
                spellData = gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            return spellData.IsCooldownHasted
                ? spellData.BaseCooldown / gameStateService.GetHasteMultiplier(gameState)
                : spellData.BaseCooldown;
        }

        public virtual decimal GetActualManaCost(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            var baseMana = gameStateService.GetBaseManaAmount(gameState);

            return baseMana * spellData.ManaCost;
        }

        public virtual decimal GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            return spellData.Duration;
        }

        public virtual decimal GetNumberOfHealingTargets(GameState gameState, BaseSpellData spellData = null)
        {
            return spellData.NumberOfHealingTargets;
        }

        public virtual decimal GetNumberOfDamageTargets(GameState gameState, BaseSpellData spellData = null)
        {
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
                spellData = gameStateService.GetSpellData(gameState, (SpellIds)SpellId);

            var averageMasteryHeal = GetAverageRawHealing(gameState, spellData)
                * (gameStateService.GetMasteryMultiplier(gameState) - 1);

            var castProfile = gameStateService.GetCastProfile(gameState, (int)SpellIds.EchoOfLight);

            result.SpellId = (int)SpellIds.EchoOfLight;
            result.SpellName = "Echo of Light";
            result.RawHealing = averageMasteryHeal;
            result.Healing = averageMasteryHeal * (1 - castProfile.OverhealPercent);
            
            return result;
        }

        #endregion
    }
}
