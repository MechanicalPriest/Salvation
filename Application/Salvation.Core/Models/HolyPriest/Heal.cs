using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class Heal 
        : BaseHealingSpell
    {
        protected HolyPriestModel holyModel;

        public override decimal AverageHeal { get => calcAverageHeal(); }

        public Heal(HolyPriestModel holyPriestModel)
            : base (holyPriestModel)
        {
            model = holyPriestModel;

            SpellData = model.GetSpellById((int)HolyPriestModel.SpellIds.Heal);
        }

        private decimal calcAverageHeal()
        {
            // Heal's average heal is:
            // SP% * Intellect * Vers * Hpriest Aura
            // TODO update this to use the buffed amount? Include as a parameter?
            decimal retVal = SpellData.Coeff1 * model.RawInt * model.GetVersMultiplier(model.RawVers);

            return retVal;
        }
    }
}
