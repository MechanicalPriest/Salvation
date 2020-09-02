using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class HolyWordSerenity 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public HolyWordSerenity(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.HolyWordSerenity);
        }

        private decimal calcAverageRawDirectHeal()
        {
            decimal retVal = SpellData.Coeff1 * HolyModel.RawInt * HolyModel.GetVersMultiplier(HolyModel.RawVers);

            return retVal;
        }
    }
}
