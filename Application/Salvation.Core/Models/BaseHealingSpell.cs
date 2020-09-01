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

        public virtual decimal AverageRawDirectHeal { get; }
    }
}
