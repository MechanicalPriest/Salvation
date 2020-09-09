using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class HolyNova 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public HolyNova(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.HolyNova);

            // TODO: Implement the double cast frequency using the HolyNovaDoubleCastFrequency modifier
            // TODO: Implement damage component
            // Holy Nova has a secondary spellID to store the healing component: 281265
        }

        private decimal calcAverageRawDirectHeal()
        {
            decimal retVal = SpellData.Coeff1 
                * model.RawInt 
                * model.GetVersMultiplier(model.RawVers)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus;

            return retVal * NumberOfTargets;
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
