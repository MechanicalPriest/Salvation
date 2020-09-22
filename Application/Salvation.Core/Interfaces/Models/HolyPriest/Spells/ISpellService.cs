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
        public int SpellId { get; }
        // TODO: Rename the return type into SpellResultModel
        AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null);

        // Healing values
        public decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null);
        public decimal GetAverageHealing(GameState gameState, BaseSpellData spellData = null);
        public decimal GetNumberOfHealingTargets(GameState gameState, BaseSpellData spellData = null);
        
        // Damage values
        public decimal GetAverageDamage(GameState gameState, BaseSpellData spellData = null);
        public decimal GetNumberOfDamageTargets(GameState gameState, BaseSpellData spellData = null);
        
        // Cast values
        public decimal GetHastedCastTime(GameState gameState, BaseSpellData spellData = null);
        public decimal GetHastedGcd(GameState gameState, BaseSpellData spellData = null);
        public decimal GetHastedCooldown(GameState gameState, BaseSpellData spellData = null);
        public decimal GetActualManaCost(GameState gameState, BaseSpellData spellData = null);
        public decimal GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null);
        public decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null);
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
