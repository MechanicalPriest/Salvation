using Salvation.Core.Models.HolyPriest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class AscendedNova
        : BaseHolyPriestHealingSpell
    {
        private decimal allowedDuration;
        private decimal boonCPM;

        public AscendedNova(BaseModel model, decimal numberOfTargetsHit = 0)
            : base (model, numberOfTargetsHit)
        {
            SpellData = model.GetSpecSpellDataById((int)HolyPriestModel.SpellIds.AscendedNova);
            allowedDuration = 0;
        }

        /// <summary>
        /// Used to set the duration of time availablt to AN after AB is done being cast.
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
            // AN does AOE damage and heals nearby (8y) friendlies (1 stack/target)

            decimal averageHeal = SpellData.Coeff2 
                * model.RawInt
                * model.GetVersMultiplier(model.RawVers)
                * holyPriestAuraHealingBonus;

            averageHeal *= model.GetCritMultiplier(model.RawCrit);

            return averageHeal * NumberOfTargets;
        }

        protected override decimal calcAverageDamage()
        {
            decimal averageDamage = SpellData.Coeff1
                * model.RawInt
                * model.GetVersMultiplier(model.RawVers)
                * holyPriestAuraDamageBonus;

            averageDamage *= model.GetCritMultiplier(model.RawCrit);

            return averageDamage * NumberOfTargets;
        }

        protected override decimal calcCastsPerMinute()
        {
            return MaximumCastsPerMinute * CastProfile.Efficiency;
        }

        protected override decimal calcMaximumCastsPerMinute()
        {
            // Max casts is whatever time we have available multiplied by efficiency
            decimal maximumPotentialCasts = allowedDuration / HastedGcd;

            // This is the maximum potential casts per Boon CD
            maximumPotentialCasts = maximumPotentialCasts * boonCPM;

            return maximumPotentialCasts;
        }
    }
}
