using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class HolyNova 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public HolyNova(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellById((int)HolyPriestModel.SpellIds.HolyNova);

            // TODO: Implement the double cast frequency using the HolyNovaDoubleCastFrequency modifier
            // TODO: Implement damage component
            // Holy Nova has a secondary spellID to store the healing component: 281265
        }

        private decimal calcAverageRawDirectHeal()
        {
            // HN's average heal is:
            // SP% * Intellect * Vers * Hpriest Aura
            decimal retVal = SpellData.Coeff1 * HolyModel.RawInt * HolyModel.GetVersMultiplier(HolyModel.RawVers) * NumberOfTargets;

            return retVal;
        }
    }
}
