namespace Salvation.Core.Constants
{
    public class BaseSpellDataEffect
    {
        public uint Id { get; set; }
        public double BaseValue { get; set; }
        public double SpCoefficient { get; set; }
        public uint TriggerSpellid { get; set; }
        public BaseSpellData TriggerSpell { get; set; }
        public uint Type { get; set; }
    }
}
