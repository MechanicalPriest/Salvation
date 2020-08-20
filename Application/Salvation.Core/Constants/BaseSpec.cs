using System.Collections.Generic;

namespace Salvation.Core.Constants
{
    public class BaseSpec
    {
        public string Class { get; set; }
        public string Spec { get; set; }
        public int SpecId { get; set; }
        public int ManaBase { get; set; }


        public decimal CritBase { get; set; }
        public decimal HasteBase { get; set; }
        public decimal VersBase { get; set; }
        public decimal MasteryBase { get; set; }
        public decimal IntBase { get; set; }
        public decimal StamBase { get; set; }

        public decimal CritCost { get; set; }
        public decimal HasteCost { get; set; }
        public decimal VersCost { get; set; }
        public decimal MasteryCost { get; set; }
        public decimal LeechCost { get; set; }


        public List<BaseSpellData> Spells { get; set; }
        public List<BaseModifier> Modifiers { get; set; }

        public BaseSpec()
        {
            Spells = new List<BaseSpellData>();
            Modifiers = new List<BaseModifier>();
        }
    }
}