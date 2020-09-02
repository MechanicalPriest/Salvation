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
            // CoH's average heal is:
            // SP% * Intellect * Vers * Hpriest Aura
            decimal retVal = SpellData.Coeff1 * HolyModel.RawInt * HolyModel.GetVersMultiplier(HolyModel.RawVers) * NumberOfTargets;

            return retVal;
        }
    }
}
