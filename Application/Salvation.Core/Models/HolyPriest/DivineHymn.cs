using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class DivineHymn 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public DivineHymn(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.DivineHymn);
        }

        private decimal calcAverageRawDirectHeal()
        {
            // DH's average heal for the first tick is:
            // SP% * Intellect * Vers * Hpriest Aura
            decimal firstTick = SpellData.Coeff1 * HolyModel.RawInt * HolyModel.GetVersMultiplier(HolyModel.RawVers);

            // For the remaining ticks they're all modified by the +healing% aura
            var divineHymnAura = model.GetModifierbyName("DivineHymnBonusHealing");

            // Now the rest of the 4 ticks including the aura:
            decimal retVal = firstTick + (firstTick * 4 * (1 + divineHymnAura.Value));

            // Return the total healing for all targets, double it if we have 5 or less (dungeon group buff)
            return (NumberOfTargets > 6 ? retVal : retVal * 2) * NumberOfTargets;
        }
    }
}
