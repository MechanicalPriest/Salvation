using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class PowerWordShield 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public PowerWordShield(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.PowerWordShield);
        }

        private decimal calcAverageRawDirectHeal()
        {
            // PW:S's average absorb is:
            // SP% * 2 * Vers * Hpriest Aura
            decimal retVal = SpellData.Coeff1 
                * 2
                * HolyModel.RawInt
                * HolyModel.GetVersMultiplier(HolyModel.RawVers);

            return retVal;
        }
    }
}
