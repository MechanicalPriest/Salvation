using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Modelling.Common
{
    public class BaseModelResults
    {
        public List<AveragedSpellCastResult> SpellCastResults;

        public BaseProfile Profile { get; set; }

        public decimal TotalActualHPS { get; set; }
        public decimal TotalRawHPS { get; set; }
        public decimal TotalMPS { get; set; }
        public decimal TotalRawHPM { get; set; }
        public decimal TotalActualHPM { get; set; }
        public decimal TimeToOom { get; set; }
        public AveragedSpellCastResult OverallResults { get; set; }

        public BaseModelResults()
        {
            SpellCastResults = new List<AveragedSpellCastResult>();
        }
    }
}
