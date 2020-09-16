using Salvation.Core.Constants;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    internal class UnholyTransfusion
        : BaseHolyPriestHealingSpell
    {
        public UnholyTransfusion(BaseModel model, BaseSpellData spellData = null)
            : base(model, spellData)
        {
            if (spellData == null)
                SpellData = model.GetSpecSpellDataById((int)HolyPriestModel.SpellIds.UnholyTransfusion);
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            // Healing from the HoT
            decimal averageHeal = SpellData.Coeff1 
                * model.RawInt
                * model.GetVersMultiplier(model.RawVers)
                * holyPriestAuraHealingBonus;

            averageHeal *= model.GetCritMultiplier(model.RawCrit);

            // Apply the Festering Transfusion conduit
            averageHeal *= getFesteringTransfusionConduitMultiplier();
            var duration = applyFesteringTransfusionConduitDuration(SpellData.Duration);

            // For each healing target, heal every ~1.5s for heal amt
            // TODO: Get a better number on healing events per player for the duration of UT
            return averageHeal * SpellData.NumberOfHealingTargets * (duration / 1.5m);
        }

        protected override decimal calcAverageDamage()
        {
            // coeff2 * int * hpriest dmg mod * vers
            decimal averageDamage = SpellData.Coeff2
                * model.RawInt
                * holyPriestAuraDamageBonus
                * model.GetVersMultiplier(model.RawVers)
                * 5; // Number of ticks

            averageDamage *= model.GetCritMultiplier(model.RawCrit);
            averageDamage *= model.GetHasteMultiplier(model.RawHaste);

            // Apply the Festering Transfusion conduit
            averageDamage *= getFesteringTransfusionConduitMultiplier();

            return averageDamage * SpellData.NumberOfDamageTargets;
        }

        /// <summary>
        /// Implements the Festering Transfusion conduit
        /// </summary>
        /// <param name="duration">Current duration of Unholy Transfusion</param>
        /// <returns></returns>
        private decimal applyFesteringTransfusionConduitDuration(decimal duration)
        {
            if (model.Profile.IsConduitActive(Conduit.FesteringTransfusion))
            {
                var conduitData = model.GetConduitDataById((int)Conduit.FesteringTransfusion);

                duration += conduitData.Coeff1;
            }

            return duration;
        }

        private decimal getFesteringTransfusionConduitMultiplier()
        {
            if (model.Profile.IsConduitActive(Conduit.FesteringTransfusion))
            {
                var rank = model.Profile.Conduits[Conduit.FesteringTransfusion];
                var conduitData = model.GetConduitDataById((int)Conduit.FesteringTransfusion);

                return 1 + (conduitData.Ranks[rank] / 100);
            }

            return 1;
        }
    }
}
