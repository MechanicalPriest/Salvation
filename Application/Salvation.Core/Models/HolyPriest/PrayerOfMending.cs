using Salvation.Core.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    internal class PrayerOfMending 
        : BaseHolyPriestHealingSpell
    {
        public decimal PrayerOfMendingBounces { get; set; }

        public PrayerOfMending(BaseModel model, BaseSpellData spellData = null)
            : base(model, spellData)
        {
            if (spellData == null)
                SpellData = model.GetSpecSpellDataById((int)HolyPriestModel.SpellIds.PrayerOfMending);

            PrayerOfMendingBounces = model.GetModifierbyName("PrayerOfMendingBounces").Value;
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            
            // PoM bounces 4 times, healing 5 (1 + 4) people total. 
            decimal averageHeal = SpellData.Coeff1 
                * model.RawInt 
                * model.GetVersMultiplier(model.RawVers)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus
                * (1 + PrayerOfMendingBounces);

            return averageHeal * SpellData.NumberOfHealingTargets;
        }

        protected override decimal calcCastsPerMinute()
        {
            decimal castsPerMinute = CastProfile.Efficiency * MaximumCastsPerMinute;

            return castsPerMinute;
        }

        protected override decimal calcMaximumCastsPerMinute()
        {
            // A fix to the spell being modified to have no cast time and no gcd and no CD
            // This can happen if it's a component in another spell
            if (HastedCastTime == 0 && HastedGcd == 0 && HastedCooldown == 0)
                return base.calcMaximumCastsPerMinute();

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
