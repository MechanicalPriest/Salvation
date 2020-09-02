using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class PrayerOfMending 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public PrayerOfMending(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.PrayerOfMending);
        }

        private decimal calcAverageRawDirectHeal()
        {
            var pomBounces = model.GetModifierbyName("PrayerOfMendingBounces").Value;
            // PoM bounces 4 times, healing 5 (1 + 4) people total. 
            decimal averageHeal = SpellData.Coeff1 
                * model.RawInt 
                * model.GetVersMultiplier(model.RawVers)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus
                * (1 + pomBounces);

            return averageHeal * NumberOfTargets;
        }
    }
}
