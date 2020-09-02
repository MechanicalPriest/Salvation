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
            decimal averageHeal = SpellData.Coeff1 
                * model.RawInt
                * model.GetVersMultiplier(model.RawVers)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus
                * 2; // PW:S has a x2 multiplier built in to it

            return averageHeal * NumberOfTargets;
        }
    }
}
