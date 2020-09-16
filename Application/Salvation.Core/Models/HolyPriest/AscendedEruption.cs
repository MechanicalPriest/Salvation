using Salvation.Core.Constants;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    internal class AscendedEruption
        : BaseHolyPriestHealingSpell
    {
        private decimal cpmFromBoon;
        private decimal numberOfBoonStacks;
        public AscendedEruption(BaseModel model, BaseSpellData spellData = null)
            : base(model, spellData)
        {
            if (spellData == null)
                SpellData = model.GetSpecSpellDataById((int)HolyPriestModel.SpellIds.AscendedEruption);

            // TODO: Implement this properly through Boon as part of #14
            numberOfBoonStacks = 10;
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

            var bonusPerStack = applyCourageousAscensionConduit(SpellData.Coeff3);

            averageHeal *= 1 + ((bonusPerStack / 100) * numberOfBoonStacks);

            averageHeal *= 1 / (decimal)Math.Sqrt((double)SpellData.NumberOfHealingTargets);                

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

            averageDamage *= 1 / (decimal)Math.Sqrt((double)SpellData.NumberOfDamageTargets);

            return averageDamage * SpellData.NumberOfDamageTargets;
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

        /// <summary>
        /// Implements covenant ability Courageous Ascension
        /// </summary>
        /// <param name="averageDamage">Current bonus per stack</param>
        /// <returns></returns>

        private decimal applyCourageousAscensionConduit(decimal bonusPerStack)
        {
            if (model.Profile.IsConduitActive(Conduit.CourageousAscension))
            {
                var conduitData = model.GetConduitDataById((int)Conduit.CourageousAscension);

                bonusPerStack += conduitData.Coeff1;
            }

            return bonusPerStack;
        }
    }
}
