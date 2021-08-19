using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Salvation.Core.Constants
{
    public enum Override
    {
        NumberOfHealingTargets,
        NumberOfDamageTargets,
        CastsPerMinute,
        Duration,
        /// <summary>
        /// Used to store the allowed duration a spell can use to calculate its CPM from 
        /// (for things like buff windows)
        /// </summary>
        AllowedDuration,
        /// <summary>
        /// Stores a multiplier that affects the result, typically from an external 
        /// source like storing Boon of the Ascended stacks to buff AscendedEruption
        /// </summary>
        ResultMultiplier,
        /// <summary>
        /// Calculated field, generated from the associated item or player level scaling
        /// to get the scale budget for the SP coefficient
        /// </summary>
        ScaleBudget,
        /// <summary>
        /// The item level of item spells
        /// </summary>
        ItemLevel
    }

    public class BaseSpellData
    {
        /// <summary>
        /// ID of the spell
        /// </summary>
        public uint Id { get; set; }
        public string Name { get; set; }
        // Mana cost as a percentage
        public double ManaCost { get; set; }
        // Casting range
        public double MaxRange { get; set; }
        // Base cast time in seconds before haste
        public double BaseCastTime { get; set; }
        public double BaseCooldown { get; set; }
        public bool IsCooldownHasted { get; set; }
        public double Duration { get; set; }
        // Base GCD in seconds before haste
        public double Gcd { get; set; }
        public uint ChargeCooldown { get; set; }
        public uint Charges { get; set; }
        public double MaxStacks { get; set; }
        public double ProcChance { get; set; }
        public double InternalCooldown { get; set; }
        public double Rppm { get; set; }
        public bool RppmIsHasted { get; set; }

        public IList<BaseSpellDataEffect> Effects { get; set; }
        public IDictionary<uint, double> ConduitRanks { get; set; }
        /// <summary>
        /// Stores value overrides for this particular spell
        /// </summary>
        [JsonIgnore]
        public Dictionary<Override, double> Overrides { get; set; }

        public BaseSpellData()
        {
            Effects = new List<BaseSpellDataEffect>();
            Overrides = new Dictionary<Override, double>();
        }

        /// <summary>
        /// Helper method to either get the requested effect ID or throw an exception
        /// </summary>
        /// <param name="effectId">ID of the effect to get</param>
        /// <returns>The effect as specified by the supplied effectId</returns>
        public BaseSpellDataEffect GetEffect(uint effectId)
        {
            var effect = Effects?.Where(e => e.Id == effectId).FirstOrDefault();

            if (effect == null)
                throw new ArgumentNullException($"Effect list does not contain effect: {effectId} for spell: {Name} ({Id})");

            return effect;
        }
    }
}