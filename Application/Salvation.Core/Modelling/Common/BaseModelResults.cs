using Salvation.Core.Profile.Model;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.Common
{
    public class BaseModelResults
    {
        public List<AveragedSpellCastResult> SpellCastResults { get; set; }
        public List<AveragedSpellCastResult> RolledUpResultsSummary { get; set; }

        public PlayerProfile Profile { get; set; }

        public double TotalActualHPS { get; set; }
        public double TotalRawHPS { get; set; }
        public double TotalMPS { get; set; }
        public double TotalRawHPM { get; set; }
        public double TotalActualHPM { get; set; }
        public double TimeToOom { get; set; }
        public AveragedSpellCastResult OverallResults { get; set; }

        public BaseModelResults()
        {
            SpellCastResults = new List<AveragedSpellCastResult>();
            RolledUpResultsSummary = new List<AveragedSpellCastResult>();
        }

        public override string ToString()
        {
            return $"[{Profile?.Name}] RawHPS: {TotalRawHPS} ActualHPS: {TotalActualHPS}";
        }
    }
}
