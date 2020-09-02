using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class Renew 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public Renew(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.Renew);
        }

        private decimal calcAverageRawDirectHeal()
        {
            // Renews's average heal is initial + HoT portion:
            // ((SP% * Intellect * Vers) + (SP% * Intellect * Vers * Haste * 5)) * Hpriest Aura
            decimal retVal =
                (
                    SpellData.Coeff1 
                    * HolyModel.RawInt
                    * HolyModel.GetVersMultiplier(HolyModel.RawVers)
                ) + (
                    SpellData.Coeff1
                    * HolyModel.RawInt
                    * HolyModel.GetVersMultiplier(HolyModel.RawVers)
                    * HolyModel.GetHasteMultiplier(HolyModel.RawHaste)
                    * 5
                );

            return retVal;
        }
    }
}
