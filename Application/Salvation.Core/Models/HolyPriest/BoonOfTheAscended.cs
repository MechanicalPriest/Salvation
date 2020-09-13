using Salvation.Core.Models.Common;
using Salvation.Core.Models.HolyPriest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class BoonOfTheAscended
        : BaseHolyPriestHealingSpell
    {
        public BoonOfTheAscended(BaseModel model, decimal numberOfTargetsHit = 0)
            : base (model, numberOfTargetsHit)
        {
            SpellData = model.GetSpecSpellDataById((int)HolyPriestModel.SpellIds.BoonOfTheAscended);
        }

        public override AveragedSpellCastResult CastAverageSpell()
        {
            AveragedSpellCastResult result = base.CastAverageSpell();

            // Figure out what's being cast during boon then add them all together.

            var ascBlast = new AscendedBlast(HolyModel);
            ascBlast.SetAvailableCastTime(SpellData.Duration);
            ascBlast.SetBoonCPM(CastsPerMinute);

            var ascBlastAvgCast = ascBlast.CastAverageSpell();

            result.AdditionalCasts.Add(ascBlastAvgCast);

            var ascNova = new AscendedNova(HolyModel);

            // Asc nova needs to know how much free time it has to cast 
            // For this we can set its efficiency. 100% efficiency is casting for the entire 10-seconds.
            ascNova.SetAvailableCastTime(SpellData.Duration - 
                (ascBlastAvgCast.CastsPerMinute * ascBlastAvgCast.Gcd));
            ascNova.SetBoonCPM(CastsPerMinute);

            result.AdditionalCasts.Add(ascNova.CastAverageSpell());

            // Asc Erruption needs to know how many stacks we built up.
            var ascErruption = new AscendedEruption(HolyModel);
            ascErruption.SetCPM(CastsPerMinute);

            result.AdditionalCasts.Add(ascErruption.CastAverageSpell());

            return result;
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            // --Boon of the Ascended--
            // When you cast it for 10-seconds Smite turns into AB and Boon turns into AN
            // AB does ST damage and heals a random friendly (5 stack)
            // AN does AOE damage and heals nearby (8y) friendlies (1 stack/target)
            // AE explodes at the end healing 3% more per stack to all friendlies (15y)
            // You might also cast something else if things are dire (dispel/serenity)

            // Potential situations:
            // Cast Boon > AN x 11 
            // Cast Boon > AB > ??? > AB until expiry (AB x4).
            // Cast Boon > (AB > ANx2) x3 > AB > AN > AE

            // Boon itself does no healing.

            return 0;
        }

        protected override decimal calcAverageDamage()
        {
            // coeff3 * int * hpriest dmg mod * vers
            decimal averageDamage = SpellData.Coeff3
                * model.RawInt
                * holyPriestAuraDamageBonus
                * model.GetVersMultiplier(model.RawVers);

            return averageDamage;
        }

        protected override decimal calcCastsPerMinute()
        {
            if (CastProfile.Efficiency == 0)
                return 0;

            decimal castsPerMinute = CastProfile.Efficiency * MaximumCastsPerMinute;

            return castsPerMinute;
        }

        protected override decimal calcMaximumCastsPerMinute()
        {
            // Mindgames CD isn't haste affected so it is simply 
            // 60 / CD + 1 / (FightLength / 60)
            // Number of casts per minute plus one cast at the start of the encounter
            decimal maximumPotentialCasts = 60m / HastedCooldown
                + 1m / (model.FightLengthSeconds / 60m);

            return maximumPotentialCasts;
        }

        protected override decimal getActualManaCost()
        {
            // Mindgames restores 1,000 mana (0.2%) if it actually heals you. lets presume it does still
            return base.getActualManaCost() - (model.RawMana * SpellData.Coeff2);
        }
    }
}
