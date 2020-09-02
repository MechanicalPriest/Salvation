using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class Halo 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public Halo(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.Halo);
        }

        private decimal calcAverageRawDirectHeal()
        {
            // Halo healing is capped at a max of ~6 targets, leaving this without a fixed ceiling for TC purposes.
            decimal retVal = SpellData.Coeff1 * HolyModel.RawInt * HolyModel.GetVersMultiplier(HolyModel.RawVers) * NumberOfTargets;

            return retVal;
        }
    }
}
