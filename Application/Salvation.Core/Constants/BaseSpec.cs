using System.Collections.Generic;

namespace Salvation.Core.Constants
{
    public class BaseSpec
    {
        public string Class { get; set; }
        public string Spec { get; set; }
        public int SpecId { get; set; }
        public double ManaBase { get; set; }


        public double CritBase { get; set; }
        public double HasteBase { get; set; }
        public double VersBase { get; set; }
        public double MasteryBase { get; set; }
        public double IntBase { get; set; }
        public double StamBase { get; set; }

        public double CritCost { get; set; }
        public double HasteCost { get; set; }
        public double VersCost { get; set; }
        public double MasteryCost { get; set; }
        public double LeechCost { get; set; }
        public double SpeedCost { get; set; }
        public double AvoidanceCost { get; set; }
        public double StamCost { get; set; }


        public List<BaseSpellData> Spells { get; set; }
        public List<BaseModifier> Modifiers { get; set; }
        public List<ConduitData> Conduits { get; set; }

        public BaseSpec()
        {
            Spells = new List<BaseSpellData>();
            Modifiers = new List<BaseModifier>();
            Conduits = new List<ConduitData>();
        }
    }
}