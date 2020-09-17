using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Salvation.Core.Models
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
