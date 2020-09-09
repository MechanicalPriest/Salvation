using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class PrayerOfMending 
        : BaseHolyPriestHealingSpell
    {
        public PrayerOfMending(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.PrayerOfMending);
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            var pomBounces = model.GetModifierbyName("PrayerOfMendingBounces").Value;
            // PoM bounces 4 times, healing 5 (1 + 4) people total. 
            decimal averageHeal = SpellData.Coeff1 
                * model.RawInt 
                * model.GetVersMultiplier(model.RawVers)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus
                * (1 + pomBounces);

            return averageHeal * NumberOfTargets;
        }

        protected override decimal calcCastsPerMinute()
        {
            decimal castsPerMinute = CastProfile.Efficiency * MaximumCastsPerMinute;

            return castsPerMinute;
        }

        protected override decimal calcMaximumCastsPerMinute()
        {
            /* Efficiency is:
            * =F294/(
            *   IF(
            *     (tCDPoM/(1+(S290/tCostHaste)/100)) > 0, 
            *     60 / ((tCastTimePoM/(1+(S290/tCostHaste)/100))+(tCDPoM/(1+(S290/tCostHaste)/100)))
            *     ,"") 
            *   + 1/(J316/60)
            * ) 
            * 
            * CPM / ((60 / (HastedPoMCT + HastedPoMCD)) + 1 / FightLengthSeconds / 60)
            */

            /* Casts per minute is:
             * Efficiency * MaximumPotentialCasts
             * 
             * MaximumPotentialCasts is:
             * 60 / (CastTime + Cooldown)
             */


            // Yes I'm aware hasted cast time for PoM is 0. The CD starts immediately.
            // TODO: Decide if PoM should have a component that includes the initial cast you get
            // or not given you have the ability to get full value out of it pre-combat.
            decimal maximumPotentialCasts = 60m / (HastedCastTime + HastedCooldown);

            return maximumPotentialCasts;
        }
    }
}
