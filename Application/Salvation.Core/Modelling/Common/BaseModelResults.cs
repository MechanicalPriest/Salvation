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

        // Stats
        public double AverageIntellect { get; set; } = double.MinValue;
        public double AverageHasteRating { get; set; } = double.MinValue;
        public double AverageCritRating { get; set; } = double.MinValue;
        public double AverageVersatilityRating { get; set; } = double.MinValue;
        public double AverageMasteryRating { get; set; } = double.MinValue;
        public double AverageLeechRating { get; set; } = double.MinValue;

        public double AverageHastePercent { get; set; } = double.MinValue;
        public double AverageCritPercent { get; set; } = double.MinValue;
        public double AverageVersatilityPercent { get; set; } = double.MinValue;
        public double AverageMasteryPercent { get; set; } = double.MinValue;
        public double AverageLeechPercent { get; set; } = double.MinValue;

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
