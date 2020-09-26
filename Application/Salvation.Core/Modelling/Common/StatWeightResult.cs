using System.Collections.Generic;

namespace Salvation.Core.Modelling.Common
{
    public class StatWeightResult
    {
        public string Name { get; set; }
        public List<StatWeightResultEntry> Results { get; set; }

        public StatWeightResult()
        {
            Results = new List<StatWeightResultEntry>();
        }
    }

    public class StatWeightResultEntry
    {
        public string Stat { get; set; }
        public decimal Weight { get; set; }
        public decimal Value { get; set; }
    }
}
