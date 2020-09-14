using Salvation.Core.Constants;
using Salvation.Core.Models.HolyPriest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class MindGames
        : BaseHolyPriestHealingSpell
    {
        public MindGames(BaseModel model, BaseSpellData spellData = null)
            : base(model, spellData)
        {
            if (spellData == null)
                SpellData = model.GetSpecSpellDataById((int)HolyPriestModel.SpellIds.MindGames);
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            // Mind Game's average heal is:
            // $damage=${($SPS*$s2/100)*(1+$@versadmg)}
            // (SP% * Coeff1 / 100) * Vers
            decimal averageHeal = (SpellData.Coeff1 * model.RawInt / 100)
                * model.GetVersMultiplier(model.RawVers)
                * holyPriestAuraHealingBonus;

            // Mindgames absorbs the incoming hit 323701, and heals for the amount absorbed 323706. 
            // The order of events though is Heal then Absorb.

            return averageHeal * 2 * SpellData.NumberOfHealingTargets;
        }

        protected override decimal calcAverageDamage()
        {
            // coeff3 * int * hpriest dmg mod * vers
            decimal averageDamage = SpellData.Coeff3
                * model.RawInt
                * holyPriestAuraDamageBonus
                * model.GetVersMultiplier(model.RawVers);

            return averageDamage * SpellData.NumberOfDamageTargets;
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
