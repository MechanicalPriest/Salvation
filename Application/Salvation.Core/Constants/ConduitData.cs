using System.Collections.Generic;

namespace Salvation.Core.Constants
{
    public class ConduitData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ConduitId { get; set; }
        public List<decimal> Ranks { get; set; }
        public decimal Coeff1 { get; set; }

        public ConduitData()
        {
            Ranks = new List<decimal>();
        }
    }
}
