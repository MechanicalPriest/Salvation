using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class HolyWordSanctify
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public HolyWordSanctify(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.HolyWordSanctify);
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
            decimal castsPerMinute = CastProfile.Efficiency * MaximumCastsPerMinute;

            return castsPerMinute;
        }

        protected override decimal calcMaximumCastsPerMinute()
        {
            // Max casts per minute is (60 + (PoH + BH * 0.5 + Renew * 1/3) * HwCDR) / CD + 1 / (FightLength / 60)
            // HWCDR is 6 base, more with LOTN/other effects
            // 1 from regular CD + reductions from fillers divided by the cooldown to get base CPM
            // Then add the one charge we start with, 1 per fight, into seconds.

            var poh = model.GetSpell<PrayerOfHealing>(HolyPriestModel.SpellIds.PrayerOfHealing).CastAverageSpell();
            var renew = model.GetSpell<Renew>(HolyPriestModel.SpellIds.Renew).CastAverageSpell();
            var bh = model.GetSpell<BindingHeal>(HolyPriestModel.SpellIds.BindingHeal).CastAverageSpell();

            // TODO: Add other HW CDR increasing effects, likely as a HolyPriestModel method.
            var hwCDRBase = model.GetModifierbyName("HolyWordsBaseCDR").Value;

            decimal hwCDR = (poh.CastsPerMinute + bh.CastsPerMinute * 0.5m
                + renew.CastsPerMinute * 1m / 3m) * hwCDRBase;

            decimal maximumPotentialCasts = (60m + hwCDR) / HastedCooldown
                + 1m / (model.FightLengthSeconds / 60m);

            return maximumPotentialCasts;
        }
    }
}
