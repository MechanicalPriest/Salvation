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
        public virtual decimal AverageRawDirectHeal { get; }
        /// <summary>
        /// The direct healing component any potential additional factors (like Hpriestm astery)
        /// </summary>
        public virtual decimal AverageTotalHeal { get { return AverageRawDirectHeal; } }


        public BaseHealingSpell(BaseModel model, decimal numberOfTargetsHit)
            : base(model, numberOfTargetsHit)
        {

        }

        public override AveragedSpellCastResult CastAverageSpell()
        {
            AveragedSpellCastResult result = base.CastAverageSpell();

            result.Healing = AverageTotalHeal;

            return result;
        }
    }
}
