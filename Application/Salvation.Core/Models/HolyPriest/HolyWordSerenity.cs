using Salvation.Core.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class HolyWordSerenity 
        : BaseHolyPriestHealingSpell
    {
        public HolyWordSerenity(BaseModel model, BaseSpellData spellData = null)
            : base(model, spellData)
        {
            if (spellData == null)
                SpellData = model.GetSpecSpellDataById((int)HolyPriestModel.SpellIds.HolyWordSerenity);
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            decimal averageHeal = SpellData.Coeff1 
                * model.RawInt 
                * model.GetVersMultiplier(model.RawVers)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus;

            return averageHeal * SpellData.NumberOfHealingTargets;
        }

        protected override decimal calcCastsPerMinute()
        {
            decimal castsPerMinute = CastProfile.Efficiency * MaximumCastsPerMinute;

            return castsPerMinute;
        }

        protected override decimal calcMaximumCastsPerMinute()
        {
            // Max casts per minute is (60 + (FH + Heal + BH * 0.5) * HwCDR) / CD + 1 / (FightLength / 60)
            // HWCDR is 6 base, more with LOTN/other effects
            // 1 from regular CD + reductions from fillers divided by the cooldown to get base CPM
            // Then add the one charge we start with, 1 per fight, into seconds.

            var fh = model.GetSpell<FlashHeal>(HolyPriestModel.SpellIds.FlashHeal).CastAverageSpell();
            var heal = model.GetSpell<Heal>(HolyPriestModel.SpellIds.Heal).CastAverageSpell();
            var bh = model.GetSpell<BindingHeal>(HolyPriestModel.SpellIds.BindingHeal).CastAverageSpell();

            // TODO: Add other HW CDR increasing effects, likely as a HolyPriestModel method.
            var hwCDRBase = model.GetModifierbyName("HolyWordsBaseCDR").Value;

            decimal hwCDR = (fh.CastsPerMinute + heal.CastsPerMinute
                + bh.CastsPerMinute * 0.5m) * hwCDRBase;

            decimal maximumPotentialCasts = (60m + hwCDR) / HastedCooldown
                + 1m / (model.FightLengthSeconds / 60m);

            return maximumPotentialCasts;
        }
    }
}
