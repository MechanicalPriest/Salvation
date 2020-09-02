using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class BaseHolyPriestHealingSpell
        : BaseHealingSpell
    {
        protected HolyPriestModel HolyModel { get { return model as HolyPriestModel; } }

        public virtual decimal AverageRawMasteryHeal { get => calcAverageRawMasteryHeal(); }
        public override decimal AverageTotalHeal { get => calcAverageTotalHeal(); }

        public BaseHolyPriestHealingSpell(BaseModel model, decimal numberOfTargetsHit)
            : base(model, numberOfTargetsHit)
        {
            // Some notes on holy priest spells:
            // - Everything basically interacts with mastery except renew/pw:s
            // - Most spells are modified by crit/vers
            // - At time of writing the hpriest spec aura is nuked and not relevant
        }


        private decimal calcAverageRawMasteryHeal()
        {
            if (model is HolyPriestModel && SpellData.IsMasteryTriggered)
            {
                // TODO: Clean this up a bit, another method maybe?
                decimal retVal = AverageRawDirectHeal * (model.GetMasteryMultiplier(model.RawMastery) - 1);

                return retVal;
            }

            return 0;
        }

        private decimal calcAverageTotalHeal()
        {
            return AverageRawDirectHeal + AverageRawMasteryHeal;
        }
    }
}
