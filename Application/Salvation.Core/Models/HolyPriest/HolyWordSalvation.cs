using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class HolyWordSalvation 
        : BaseHolyPriestHealingSpell
    {
        public override decimal AverageRawMasteryHeal { get => calcAverageRawMasteryHeal(); }

        public override decimal AverageRawDirectHeal { get => calcAverageRawDirectHeal(); }

        public HolyWordSalvation(HolyPriestModel holyPriestModel, decimal numberOfTargetsHit = 0)
            : base (holyPriestModel, numberOfTargetsHit)
        {
            SpellData = model.GetSpellDataById((int)HolyPriestModel.SpellIds.HolyWordSalvation);
        }

        private decimal calcAverageRawDirectHeal()
        {
            // Salv first applies renew to all targets
            var renew = model.GetSpell<Renew>(HolyPriestModel.SpellIds.Renew);
            // TODO: Should probably throw something here if renew isn't being defined, as HW:Salv relies on it.
            decimal renewHealing = renew == null ? 0 : renew.AverageRawDirectHeal;

            // Then it puts 2 stacks of PoM on all targets
            var pom = model.GetSpell<PrayerOfMending>(HolyPriestModel.SpellIds.PrayerOfMending);
            var pomBounces = model.GetModifierbyName("PrayerOfMendingBounces").Value;

            var pomhealing = pom == null ? 0 : pom.AverageRawDirectHeal / pomBounces * 2;

            // Finally it casts a direct heal on all targets
            decimal salvHealing = SpellData.Coeff1 * HolyModel.RawInt * HolyModel.GetVersMultiplier(HolyModel.RawVers);

            return (renewHealing + pomhealing + salvHealing) * NumberOfTargets;
        }

        /// <summary>
        /// Overriding mastery calculations as renew doesn't have a mastery component.
        /// </summary>
        /// <returns></returns>
        private decimal calcAverageRawMasteryHeal()
        {
            // PoM healing
            var pom = model.GetSpell<PrayerOfMending>(HolyPriestModel.SpellIds.PrayerOfMending);
            var pomBounces = model.GetModifierbyName("PrayerOfMendingBounces").Value;

            var pomHealing = pom == null ? 0 : pom.AverageRawDirectHeal / pomBounces * 2;

            // Salv Healing
            decimal salvHealing = SpellData.Coeff1 * HolyModel.RawInt * HolyModel.GetVersMultiplier(HolyModel.RawVers);

            // Apply mastery
            decimal retVal = (pomHealing + salvHealing) * (model.GetMasteryMultiplier(model.RawMastery) - 1);

            return retVal * NumberOfTargets;
        }
    }
}
