using Salvation.Core.Constants;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;

namespace Salvation.Core.Interfaces.Modelling
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
        IGameStateService GameStateService { get; }

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
        public double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the average healing one cast of the spell does factoring in overheal
        /// </summary>
        public double GetAverageHealing(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the average overhealing done from one cast of the spell
        /// </summary>
        public double GetAverageOverhealing(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the number of targets the healing component of the spell cast hits
        /// </summary>
        public double GetNumberOfHealingTargets(GameState gameState, BaseSpellData spellData = null);

        // Damage values

        /// <summary>
        /// Get the average damage one cast of the spell does
        /// </summary>
        public double GetAverageDamage(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the number of targets the damage component of the spell cast hits
        /// </summary>
        public double GetNumberOfDamageTargets(GameState gameState, BaseSpellData spellData = null);

        // Cast values

        /// <summary>
        /// Get the Hasted Cast Time. Should return the regular cast time if it's not affected by haste.
        /// </summary>
        public double GetHastedCastTime(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the Hasted GCD
        /// </summary>
        public double GetHastedGcd(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the Hasted Cooldown. Should return the regular cooldown if it's not affected by haste.
        /// </summary>
        public double GetHastedCooldown(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the actual mana cost, taking into account mana reduction factors and using the base mana pool.
        /// </summary>
        public double GetActualManaCost(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the actual number of casts per minute. This is the efficiency-modified maximum casts
        /// </summary>
        public double GetActualCastsPerMinute(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the maximum potential casts per minute using the current cast profile
        /// </summary>
        public double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null);
        /// <summary>
        /// Get the duration of the cast in seconds. Typically for buffs/debuffs and DoTs/HoTs.
        /// </summary>
        public double GetDuration(GameState gameState, BaseSpellData spellData = null);
        public double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData);
        public double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData);
        public double GetMinimumDamageTargets(GameState gameState, BaseSpellData spellData);
        public double GetMaximumDamageTargets(GameState gameState, BaseSpellData spellData);

        // Effect Behaviour
        double GetUptime(GameState gameState, BaseSpellData spellData);

        // Effect Properties
        double GetAverageCriticalStrike(GameState gameState, BaseSpellData spellData);
        double GetAverageCriticalStrikePercent(GameState gameState, BaseSpellData spellData);
        double GetAverageHaste(GameState gameState, BaseSpellData spellData);
        double GetAverageHastePercent(GameState gameState, BaseSpellData spellData);
        double GetAverageIntellect(GameState gameState, BaseSpellData spellData);
        double GetAverageIntellectBonus(GameState gameState, BaseSpellData spellData);
        double GetAverageMastery(GameState gameState, BaseSpellData spellData);
        double GetAverageMasteryPercent(GameState gameState, BaseSpellData spellData);
        double GetAverageVersatility(GameState gameState, BaseSpellData spellData);
        double GetAverageVersatilityPercent(GameState gameState, BaseSpellData spellData);
        double GetAverageLeech(GameState gameState, BaseSpellData spellData);
        double GetAverageLeechPercent(GameState gameState, BaseSpellData spellData);
        double GetAverageMp5(GameState gameState, BaseSpellData spellData);
        double GetAverageHealingMultiplier(GameState gameState, BaseSpellData spellData);

        // Model Behaviour
        // These could be moved to a Holy specific interface?
        /// <summary>
        /// Return the Renew Uptime from renew effects associated with this spell.
        /// This uptime is spread over all RaidSize members. 
        /// 1 = 100% on everyone, 0 - 0% on everyone.
        /// </summary>
        double GetRenewUptime(GameState gameState, BaseSpellData spellData);
    }

    public interface ISpellService<T> : ISpellService where T : ISpellService
    {
        // TODO: Create ServiceProvider mock for testing?
    }
}
