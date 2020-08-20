using Salvation.Core.Constants;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    public class HolyPriestModel
        : BaseModel
    {
        internal enum SpellIds
        {
            Heal = 2060
        }


        public HolyPriestModel(GlobalConstants constants, BaseProfile profile)
            : base(constants, profile, Spec.HolyPriest)
        {
            Spells.Add(new Heal(this));
        }
    }
}
