using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class CircleOfHealing 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public CircleOfHealing(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.CircleOfHealing);
        }

        private decimal calcAverageRawDirectHeal()
        {
            decimal totalHeal = 0m;
            var SpellResultsStruct = new object();

            for (var i = 0; i < NumberOfTargets; i++)
            {
                totalHeal += SpellData.Coeff1
                    * model.RawInt
                    * model.GetVersMultiplier(model.RawVers)
                    * model.GetCritMultiplier(model.RawCrit)
                    * holyPriestAuraHealingBonus;

                // SpellResultsStruct.PopulateResults();
            }

            return totalHeal;
        }

        // Cast (00:00:00, CoH, 234324, DidCrit)
        // Cast (00:00:00, CoH, 234324, DidCrit)
        // Cast (00:00:00, CoH, 234324, DidCrit)
        // Cast (00:00:00, CoH, 234324, DidNotCrit)
        // Cast (00:00:00, CoH, 234324, DidNotCrit)
        //                         v
        // Cast (--------, CoH, 234324, --------)

        protected override decimal calcCastsPerMinute()
        {
            decimal castsPerMinute = CastProfile.Efficiency * MaximumCastsPerMinute;

            return castsPerMinute;
        }

        protected override decimal calcMaximumCastsPerMinute()
        {
            // CoH is 60 / (CastTime + CD) + 1 / (FightLength / 60)
            decimal maximumPotentialCasts = 60m / (HastedCastTime + HastedCooldown)
                + 1m / (model.FightLengthSeconds / 60m);

            return maximumPotentialCasts;
        }
    }
}
