using Salvation.Core.Models.HolyPriest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class MindGames
        : BaseHolyPriestHealingSpell
    {
        public MindGames(BaseModel model, decimal numberOfTargetsHit = 0)
            : base (model, numberOfTargetsHit)
        {
            SpellData = model.GetSpecSpellDataById((int)BaseModel.SpellIds.MindGames);
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            // Midn Game's average heal is:
            // $damage=${($SPS*$s2/100)*(1+$@versadmg)}
            // (SP% * Coeff1 / 100) * Vers
            decimal averageHeal = (SpellData.Coeff1 * model.RawInt / 100)
                * model.GetVersMultiplier(model.RawVers)
                * holyPriestAuraHealingBonus;

            return averageHeal * NumberOfTargets;
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
            // Mindgames CD isn't haste affected so it isis simply 
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
