using Salvation.Core.Models.Common;
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
        public override AveragedSpellCastResult CastAverageSpell()
        {
            AveragedSpellCastResult result = base.CastAverageSpell();

            // Salv first applies renew to all targets
            var renew = new Renew(HolyModel, NumberOfTargets);

            var renewResults = renew.CastAverageSpell();

            renewResults.MakeCastFree();
            renewResults.MakeCastHaveNoGcd();
            renewResults.MakeCastInstant();
            renewResults.MakeSpellHaveNoCasts();

            result.AdditionalCasts.Add(renewResults);

            // Calculate a PoM with only 1 additional bounce (Salv gives 2 stacks)
            var pom = new PrayerOfMending(HolyModel, NumberOfTargets);
            pom.PrayerOfMendingBounces = 1;

            var pomResults = pom.CastAverageSpell();
            pomResults.MakeCastFree();
            pomResults.MakeCastHaveNoGcd();
            pomResults.MakeCastInstant();
            pomResults.MakeSpellHaveNoCasts();

            result.AdditionalCasts.Add(pomResults);

            return result;
        }


        protected override decimal calcAverageRawDirectHeal()
        {
            // Finally it casts a direct heal on all targets
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
