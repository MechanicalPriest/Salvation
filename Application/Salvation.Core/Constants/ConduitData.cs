using System.Collections.Generic;

namespace Salvation.Core.Constants
{
    public class ConduitData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ConduitId { get; set; }
        public List<double> Ranks { get; set; }
        public double Coeff1 { get; set; }

        public ConduitData()
        {
            Ranks = new List<double>();
        }
    }
}
