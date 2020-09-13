using Salvation.Core.Models.HolyPriest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class AscendedEruption
        : BaseHolyPriestHealingSpell
    {
        private decimal cpmFromBoon;
        public AscendedEruption(BaseModel model, decimal numberOfTargetsHit = 0)
            : base (model, numberOfTargetsHit)
        {
            SpellData = model.GetSpecSpellDataById((int)HolyPriestModel.SpellIds.AscendedEruption);
        }

        internal void SetCPM(decimal castsPerMinute)
        {
            cpmFromBoon = castsPerMinute;
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            // --Boon of the Ascended--
            // AE explodes at the end healing 3% more per stack to all friendlies (15y)

            decimal averageHeal = SpellData.Coeff2
                * model.RawInt
                * model.GetVersMultiplier(model.RawVers)
                * holyPriestAuraHealingBonus // This may not affect it? No way to test though.
                * holyPriestAuraDamageBonus; // ???

            averageHeal *= model.GetCritMultiplier(model.RawCrit);

            averageHeal *= 1 / (decimal)Math.Sqrt((double)NumberOfTargets);                

            return averageHeal * NumberOfTargets;
        }

        protected override decimal calcAverageDamage()
        {
            // coeff3 * int * hpriest dmg mod * vers
            decimal averageDamage = SpellData.Coeff1
                * model.RawInt
                * holyPriestAuraDamageBonus
                * model.GetVersMultiplier(model.RawVers);

            averageDamage *= model.GetCritMultiplier(model.RawCrit);

            averageDamage *= 1 / (decimal)Math.Sqrt((double)NumberOfTargets);

            return averageDamage * NumberOfTargets;
        }

        protected override decimal calcCastsPerMinute()
        {
            // This has been overridden as it's cast everytime you cast Boon.
            return cpmFromBoon;
        }

        protected override decimal calcMaximumCastsPerMinute()
        {
            // This has been overridden as it's cast everytime you cast Boon.
            return cpmFromBoon;
        }
    }
}
