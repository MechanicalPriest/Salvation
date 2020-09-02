using Salvation.Core.Constants;
using Salvation.Core.Models.HolyPriest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models
{
    public class BaseHealingSpell
        : BaseSpell
    {

        public BaseHealingSpell(BaseModel model, decimal numberOfTargetsHit)
            : base(model, numberOfTargetsHit)
        {

        }

        /// <summary>
        /// The direct healing component
        /// </summary>
        public virtual decimal AverageRawDirectHeal { get; }
        /// <summary>
        /// The direct healing component any potential additional factors (like Hpriestm astery)
        /// </summary>
        public virtual decimal AverageTotalHeal { get { return AverageRawDirectHeal; } }
    }
}
