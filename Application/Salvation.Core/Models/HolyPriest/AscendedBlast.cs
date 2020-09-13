using Salvation.Core.Constants;
using Salvation.Core.Models.HolyPriest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class AscendedBlast
        : BaseHolyPriestHealingSpell
    {
        private decimal allowedDuration;
        private decimal boonCPM;

        public AscendedBlast(BaseModel model, BaseSpellData spellData = null)
            : base(model, spellData)
        {
            if(spellData == null)
                SpellData = model.GetSpecSpellDataById((int)HolyPriestModel.SpellIds.AscendedBlast);
        }

        /// <summary>
        /// Used to set the duration of time availablt to AN 
        /// </summary>
        /// <param name="allowedDuration">Duration in seconds</param>
        internal void SetAvailableCastTime(decimal allowedDuration)
        {
            this.allowedDuration = allowedDuration;
        }
        internal void SetBoonCPM(decimal cooldown)
        {
            this.boonCPM = cooldown;
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            // --Boon of the Ascended--
            // AB does ST damage and heals a random friendly (5 stack)
            // Coeff2 being 100 = 100%.

            decimal averageHeal = (SpellData.Coeff2 /100)
                * AverageDamage;

            return averageHeal * SpellData.NumberOfHealingTargets;
        }

        protected override decimal calcAverageDamage()
        {
            // coeff3 * int * hpriest dmg mod * vers
            decimal averageDamage = SpellData.Coeff1
                * model.RawInt
                * holyPriestAuraDamageBonus
                * model.GetVersMultiplier(model.RawVers);

            averageDamage *= model.GetCritMultiplier(model.RawCrit);

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
            // Initial cast, and divide the remaining duration up by cooldown for remaining casts
            decimal maximumPotentialCasts = 1 + (allowedDuration - HastedGcd) / HastedCooldown;

            // This is the maximum potential casts per Boon CD
            maximumPotentialCasts = maximumPotentialCasts * boonCPM;

            return maximumPotentialCasts;
        }
    }
}
