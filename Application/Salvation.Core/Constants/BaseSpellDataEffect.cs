using System.Collections.Generic;

namespace Salvation.Core.Constants
{
    public class BaseSpellDataEffect
    {
        public uint Id { get; set; }
        public double BaseValue { get; set; }
        public double SpCoefficient { get; set; }
        public double Coefficient { get; set; }
        public uint TriggerSpellid { get; set; }
        /// <summary>
        /// Typically used as Tick Rate for HoT/DoTs
        /// </summary>
        public double Amplitude { get; set; }
        public BaseSpellData TriggerSpell { get; set; }
        public uint Type { get; set; }


        /// <summary>
        /// Scale multiplier used for scaling item-scaled spells
        /// </summary>
        public Dictionary<int, double> ScaleValues { get; set; }

        public BaseSpellDataEffect()
        {
            ScaleValues = new Dictionary<int, double>();
        }

        public double GetScaledCoefficientValue(int level)
        {
            if (ScaleValues.ContainsKey(level))
                return ScaleValues[level] * Coefficient;
            return 0;
        }
    }
}
