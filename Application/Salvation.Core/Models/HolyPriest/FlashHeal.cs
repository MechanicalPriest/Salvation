using Salvation.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class FlashHeal 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public FlashHeal(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.FlashHeal);
        }

        private decimal calcAverageRawDirectHeal()
        {
            // Flash Heal's average heal is:
            // SP% * Intellect * Vers * Hpriest Aura
            decimal averageHeal = SpellData.Coeff1 
                * model.RawInt 
                * model.GetVersMultiplier(model.RawVers)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus;

            return averageHeal * NumberOfTargets;
        }

        private decimal castSpell()
        {
            decimal baseHeal = SpellData.Coeff1
                * model.RawInt
                * model.GetVersMultiplier(model.RawVers)
                // * model.GetCritMultiplier(model.RawCrit) rng this
                * holyPriestAuraHealingBonus;

            // do some RNG and figure out if crit applies not
            // if crit applies, crit it

            return baseHeal * NumberOfTargets;
        }

        protected override decimal calcCastsPerMinute()
        {
            decimal castsPerMinute = CastProfile.Efficiency * MaximumCastsPerMinute;

            return castsPerMinute;
        }

        protected override decimal calcMaximumCastsPerMinute()
        {
            // If it's instant cast, instead use the hasted GCD as the limiting factor
            decimal fillerCastTime = HastedCastTime == 0
                ? HastedGcd
                : HastedCastTime;

            decimal maximumPotentialCasts = 60m / fillerCastTime;

            return maximumPotentialCasts;
        }
    }
}
