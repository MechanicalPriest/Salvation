using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class PrayerOfHealing 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public PrayerOfHealing(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellById((int)HolyPriestModel.SpellIds.PrayerOfHealing);
        }

        private decimal calcAverageRawDirectHeal()
        {
            // PoH's average heal is:
            // SP% * Intellect * Vers * Hpriest Aura
            decimal retVal = SpellData.Coeff1 * HolyModel.RawInt * HolyModel.GetVersMultiplier(HolyModel.RawVers) * NumberOfTargets;

            return retVal;
        }
    }
}
