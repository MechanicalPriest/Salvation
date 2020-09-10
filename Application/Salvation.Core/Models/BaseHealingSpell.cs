using Salvation.Core.Constants;
using Salvation.Core.Models.Common;
using Salvation.Core.Models.HolyPriest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models
{
    public class BaseHealingSpell
        : BaseSpell
    {

        /// <summary>
        /// The direct healing component
        /// </summary>
        protected virtual decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }


        /// <summary>
        /// The direct healing component any potential additional factors (like Hpriestm astery)
        /// </summary>
        protected virtual decimal AverageTotalHeal { get => calcAverageTotalHeal(); }


        public BaseHealingSpell(BaseModel model, decimal numberOfTargetsHit)
            : base(model, numberOfTargetsHit)
        {

        }

        public override AveragedSpellCastResult CastAverageSpell()
        {
            AveragedSpellCastResult result = base.CastAverageSpell();

            result.Healing = AverageTotalHeal;
            result.RawHealing = AverageRawDirectHeal;

            return result;
        }
        protected virtual decimal calcAverageRawDirectHeal()
        {
            return 0;
        }
        protected virtual decimal calcAverageTotalHeal()
        {
            return 0;
        }
    }
}
