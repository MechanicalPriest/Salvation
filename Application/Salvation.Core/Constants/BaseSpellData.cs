namespace Salvation.Core.Constants
{
    public class BaseSpellData
    {
        /// <summary>
        /// ID of the spell
        /// </summary>
        public int Id { get; set; }
        public string Name { get; set; }
        // Mana cost as a percentage
        public decimal ManaCost { get; set; }
        // Casting range
        public decimal Range { get; set; }
        /// <summary>
        /// Default number of targets this spell can hit, usually its maximum targets
        /// </summary>
        public decimal DefaultNumberOfTargets { get; set; }
        // Base cast time in seconds before haste
        public decimal BaseCastTime { get; set; }
        public bool IsCastTimeHasted{ get; set; }
        public decimal BaseCooldown { get; set; }
        public bool IsCooldownHasted { get; set; }
        public decimal Duration { get; set; }
        // Base GCD in seconds before haste
        public decimal Gcd { get; set; }

        // Spellpower coefficient for component #1
        public decimal Coeff1 { get; set; }
        // If mastery is triggered for the direct heal portion
        public bool IsMasteryTriggered { get; set; }
    }
}