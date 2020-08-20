namespace Salvation.Core.Constants
{
    public class BaseSpellData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Mana cost as a percentage
        public decimal ManaCost { get; set; }
        // Casting range
        public decimal Range { get; set; }
        // Base cast time in seconds before haste
        public decimal BaseCastTime { get; set; }
        public bool IsCastTimeHasted{ get; set; }
        // Base GCD in seconds before haste
        public decimal Gcd { get; set; }

        // Spellpower coefficient for component #1
        public decimal Coeff1 { get; set; }
    }
}