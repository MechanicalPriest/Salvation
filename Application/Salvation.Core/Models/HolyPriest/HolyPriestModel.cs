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
            Heal = 2060,
            FlashHeal = 2061,
            PrayerOfHealing = 596,
            HolyNova = 132157,
            CircleOfHealing = 204883,
            Renew = 139
        }


        public HolyPriestModel(GlobalConstants constants, BaseProfile profile)
            : base(constants, profile, Spec.HolyPriest)
        {
            Spells.Add(new Heal(this));
            Spells.Add(new FlashHeal(this));
            Spells.Add(new PrayerOfHealing(this));
            Spells.Add(new HolyNova(this));
            Spells.Add(new CircleOfHealing(this));
            Spells.Add(new Renew(this));
        }

        public object GetResults()
        {
            /// So what we want to do for this is calculate results for each spell.
            /// Get back result values such as raw direct/periodic healing, raw mastery healing, raw total healing
            /// number of targets hit, mana cost, cast time
            /// inputs that change the results are the profile, and number of targets hit by non-capped aoe spells


            return 0;
        }
    }
}
