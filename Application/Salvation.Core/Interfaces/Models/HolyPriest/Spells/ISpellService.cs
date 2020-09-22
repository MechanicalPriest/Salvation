using Salvation.Core.Constants;
using Salvation.Core.Models;
using Salvation.Core.Models.Common;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Interfaces.Models.HolyPriest.Spells
{
    /// <summary>
    /// Inherited by each spell implementation. 
    /// Stateless object that performs calculations based on the provided gamestate
    /// </summary>
    public interface ISpellService
    {
        /// <summary>
        /// The spell ID of the spell this service is manipulating
        /// </summary>
        public int SpellId { get; }
        // TODO: Rename the return type into SpellResultModel
        /// <summary>
        /// Calculate the results of casting one spell, and the casting efficiency calculations
        /// </summary>
        AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Essentially GetCastResults but just on the mastery component.
        /// </summary>
        AveragedSpellCastResult GetHolyPriestMasteryResult(GameState gameState, BaseSpellData spellData);

        // Healing values

        /// <summary>
        /// Get the average healing one cast of the spell does, excluding overheal
        /// </summary>
        public decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the average healing one cast of the spell does factoring in overheal
        /// </summary>
        public decimal GetAverageHealing(GameState gameState, BaseSpellData spellData = null);
        public decimal GetAverageOverhealing(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the number of targets the healing component of the spell cast hits
        /// </summary>
        public decimal GetNumberOfHealingTargets(GameState gameState, BaseSpellData spellData = null);
        
        // Damage values

        /// <summary>
        /// Get the average damage one cast of the spell does
        /// </summary>
        public decimal GetAverageDamage(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the number of targets the damage component of the spell cast hits
        /// </summary>
        public decimal GetNumberOfDamageTargets(GameState gameState, BaseSpellData spellData = null);
        
        // Cast values

        /// <summary>
        /// Get the Hasted Cast Time. Should return the regular cast time if it's not affected by haste.
        /// </summary>
        public decimal GetHastedCastTime(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the Hasted GCD
        /// </summary>
        public decimal GetHastedGcd(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the Hasted Cooldown. Should return the regular cooldown if it's not affected by haste.
        /// </summary>
        public decimal GetHastedCooldown(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the actual mana cost, taking into account mana reduction factors and using the base mana pool.
        /// </summary>
        public decimal GetActualManaCost(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the actual number of casts per minute. This is the efficiency-modified maximum casts
        /// </summary>
        public decimal GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the maximum potential casts per minute using the current cast profile
        /// </summary>
        public decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the duration of the cast. Typically for buffs/debuffs and DoTs/HoTs.
        /// </summary>
        public decimal GetDuration(GameState gameState, BaseSpellData spellData = null);
        
    }

    /// <summary>
    /// Could use the following if needed to override a spells model / data. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //public interface ISpellService<T> : ISpellService where T : BaseSpell
    //{
    //    //SpellCastResult GetCastResults(HolyPriestModel model, T spellOverride = null);
    //}
}
