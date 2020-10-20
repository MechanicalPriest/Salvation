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
        ResultMultiplier
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
        public decimal NumberOfDamageTargets { get; set; }
        // Base cast time in seconds before haste
        public decimal BaseCastTime { get; set; }
        public bool IsCastTimeHasted { get; set; }
        public decimal BaseCooldown { get; set; }
        public bool IsCooldownHasted { get; set; }
        public decimal Duration { get; set; }
        // Base GCD in seconds before haste
        public decimal Gcd { get; set; }

        /// <summary>
        /// Spellpower coefficient for component #1
        /// </summary>
        public decimal Coeff1 { get; set; }
        /// <summary>
        /// Spellpower coefficient for component #2
        /// </summary>
        public decimal Coeff2 { get; set; }
        /// <summary>
        /// Spellpower coefficient for component #3
        /// </summary>
        public decimal Coeff3 { get; set; }
        // If mastery is triggered for the direct heal portion
        // TODO: Move this out to a HolyPriestSpellData inherited class
        public bool IsMasteryTriggered { get; set; }

        public IList<BaseSpellDataEffect> Effects { get; set; }
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
                throw new ArgumentNullException($"Effect list does not contain effect: {effectId}");

            return effect;
        }
    }
}