using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class DivineStar 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public DivineStar(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.DivineStar);
        }

        private decimal calcAverageRawDirectHeal()
        {
            // Divine Star healing is capped at a max of ~6 targets, leaving this without a fixed ceiling for TC purposes.
            // Due to the star return also healing, double the amount of healing events
            decimal retVal = SpellData.Coeff1 * HolyModel.RawInt * HolyModel.GetVersMultiplier(HolyModel.RawVers) * NumberOfTargets * 2;

            return retVal;
        }
    }
}
