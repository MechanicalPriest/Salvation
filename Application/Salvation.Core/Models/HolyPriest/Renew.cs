﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class Renew 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public Renew(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.Renew);
        }

        private decimal calcAverageRawDirectHeal()
        {
            // Renews's average heal is initial + HoT portion:
            decimal averageHealFirstTick = SpellData.Coeff1 
                * model.RawInt
                * model.GetVersMultiplier(model.RawVers)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus;

            // HoT is affected by haste
            decimal averageHealTicks = SpellData.Coeff1
                * model.RawInt
                * model.GetVersMultiplier(model.RawVers)
                * model.GetHasteMultiplier(model.RawHaste)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus
                * 5;

            return (averageHealFirstTick + averageHealTicks) * NumberOfTargets;
        }

        protected override decimal calcCastsPerMinute()
        {
            // If it's instant cast, instead use the hasted GCD as the limiting factor
            decimal fillerCastTime = HastedCastTime == 0
                ? HastedGcd
                : HastedCastTime;

            decimal maximumPotentialCasts = 60m / fillerCastTime;

            decimal castsPerMinute = CastProfile.Efficiency * maximumPotentialCasts;

            return castsPerMinute;
        }
    }
}
