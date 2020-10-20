using System;
using System.Collections.Generic;
using System.Linq;

namespace Salvation.Core.Constants
{
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
        /// <summary>
        /// Default number of targets this spell can hit, usually its maximum targets
        /// </summary>
        public decimal NumberOfHealingTargets { get; set; }
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

        public BaseSpellData()
        {
            Effects = new List<BaseSpellDataEffect>();
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