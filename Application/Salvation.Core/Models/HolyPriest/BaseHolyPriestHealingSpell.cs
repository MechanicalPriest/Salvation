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

        public BaseHolyPriestHealingSpell(BaseModel model, decimal numberOfTargetsHit)
            : base(model, numberOfTargetsHit)
        {

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
    }
}
