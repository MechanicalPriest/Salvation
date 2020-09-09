using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class HolyWordSalvation 
        : BaseHolyPriestHealingSpell
    {
        public HolyWordSalvation(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.HolyWordSalvation);
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            // Salv first applies renew to all targets
            var renew = model.GetSpell<Renew>(HolyPriestModel.SpellIds.Renew).CastAverageSpell();
            
            decimal renewHealing = renew.RawHealing;

            // Then it puts 2 stacks of PoM on all targets
            var pom = model.GetSpell<PrayerOfMending>(HolyPriestModel.SpellIds.PrayerOfMending).CastAverageSpell();
            var pomBounces = model.GetModifierbyName("PrayerOfMendingBounces").Value;

            var pomhealing = pom == null ? 0 : pom.RawHealing / pomBounces * 2;

            // Finally it casts a direct heal on all targets
            decimal salvHealing = SpellData.Coeff1 
                * model.RawInt 
                * model.GetVersMultiplier(model.RawVers)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus;

            return (renewHealing + pomhealing + salvHealing) * NumberOfTargets;
        }

        /// <summary>
        /// Overriding mastery calculations as renew doesn't have a mastery component.
        /// </summary>
        /// <returns></returns>
        protected override decimal calcAverageRawMasteryHeal()
        {
            // TODO: split the result up to include these mini casts as children.
            // PoM healing
            var pom = model.GetSpell<PrayerOfMending>(HolyPriestModel.SpellIds.PrayerOfMending).CastAverageSpell();
            var pomBounces = model.GetModifierbyName("PrayerOfMendingBounces").Value;

            var pomHealing = pom == null ? 0 : pom.RawHealing / pomBounces * 2;

            // Salv Healing
            decimal salvHealing = SpellData.Coeff1 
                * model.RawInt 
                * model.GetVersMultiplier(model.RawVers)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus;

            // Apply mastery
            decimal retVal = (pomHealing + salvHealing) * (model.GetMasteryMultiplier(model.RawMastery) - 1);

            return retVal * NumberOfTargets;
        }


        protected override decimal calcCastsPerMinute()
        {

            decimal castsPerMinute = CastProfile.Efficiency * MaximumCastsPerMinute;

            return castsPerMinute;
        }

        protected override decimal calcMaximumCastsPerMinute()
        {
            // Salv is (60 + (SerenityCPM + SancCPM) * SalvCDR) / (CastTime + Cooldown) + 1 / (FightLength / 60)
            // Essentially the CDR per minute is 60 + the CDR from holy words.

            var serenity = model.GetSpell<HolyWordSerenity>(HolyPriestModel.SpellIds.HolyWordSerenity).CastAverageSpell();
            var sanc = model.GetSpell<HolyWordSanctify>(HolyPriestModel.SpellIds.HolyWordSanctify).CastAverageSpell();

            var salvCDRBase = model.GetModifierbyName("SalvationHolyWordCDR").Value;

            decimal salvCDRPerMin = 60m + (serenity.CastsPerMinute + sanc.CastsPerMinute) * salvCDRBase;
            decimal maximumPotentialCasts = salvCDRPerMin / (HastedCastTime + HastedCooldown)
                + 1m / (model.FightLengthSeconds / 60m);

            return maximumPotentialCasts;
        }
    }
}
