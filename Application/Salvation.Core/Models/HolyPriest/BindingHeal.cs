using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class BindingHeal 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public BindingHeal(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.BindingHeal);
        }

        private decimal calcAverageRawDirectHeal()
        {
            decimal retVal = SpellData.Coeff1 * model.RawInt * model.GetVersMultiplier(model.RawVers) * NumberOfTargets;

            return retVal;
        }
    }
}
