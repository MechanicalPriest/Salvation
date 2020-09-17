using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Salvation.Core.Models
{
    internal class StatWeightResult
    {
        public string Name { get; set; }
        public List<StatWeightResultEntry> Results { get; set; }    
    }

    internal class StatWeightResultEntry
    {
        public string Stat { get; set; }
        public decimal Weight { get; set; }
        public decimal Value { get; set; }
    }
}
