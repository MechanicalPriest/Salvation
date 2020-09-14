using Salvation.Core.Constants;
using Salvation.Core.Models.HolyPriest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class UnholyTransfusion
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

            // For each healing target, heal every ~1.5s for heal amt

            return averageHeal * SpellData.NumberOfHealingTargets * (SpellData.Duration / 1.5m);
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

            return averageDamage * SpellData.NumberOfDamageTargets;
        }
    }
}
