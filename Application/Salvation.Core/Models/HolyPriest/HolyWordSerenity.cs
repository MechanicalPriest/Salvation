﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class HolyWordSerenity 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public HolyWordSerenity(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.HolyWordSerenity);
        }

        private decimal calcAverageRawDirectHeal()
        {
            decimal averageHeal = SpellData.Coeff1 
                * model.RawInt 
                * model.GetVersMultiplier(model.RawVers)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus;

            return averageHeal * NumberOfTargets;
        }

        protected override decimal calcCastsPerMinute()
        {
            // Max casts per minute is (60 + (FH + Heal + BH * 0.5) * HwCDR) / CD + 1 / (FightLength / 60)
            // HWCDR is 6 base, more with LOTN/other effects
            // 1 from regular CD + reductions from fillers divided by the cooldown to get base CPM
            // Then add the one charge we start with, 1 per fight, into seconds.

            var fh = model.GetSpell<FlashHeal>(HolyPriestModel.SpellIds.FlashHeal);
            var heal = model.GetSpell<Heal>(HolyPriestModel.SpellIds.Heal);
            var bh = model.GetSpell<BindingHeal>(HolyPriestModel.SpellIds.BindingHeal);

            // TODO: Add other HW CDR increasing effects, likely as a HolyPriestModel method.
            var hwCDRBase = model.GetModifierbyName("HolyWordsBaseCDR").Value;

            decimal hwCDR = (fh.CastsPerMinute + heal.CastsPerMinute 
                + bh.CastsPerMinute * 0.5m) * hwCDRBase;

            decimal maximumPotentialCasts = (60m + hwCDR) / HastedCooldown 
                + 1m / (model.FightLengthSeconds / 60m);

            decimal castsPerMinute = CastProfile.Efficiency * maximumPotentialCasts;

            return castsPerMinute;
        }
    }
}
