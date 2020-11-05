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
    }
}
