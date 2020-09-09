using Salvation.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class BaseHolyPriestHealingSpell
        : BaseHealingSpell
    {
        protected HolyPriestModel HolyModel { get { return model as HolyPriestModel; } }
        protected decimal holyPriestAuraHealingBonus { get; }

        public virtual decimal AverageRawMasteryHeal { get => calcAverageRawMasteryHeal(); }

        public BaseHolyPriestHealingSpell(BaseModel model, decimal numberOfTargetsHit)
            : base(model, numberOfTargetsHit)
        {
            holyPriestAuraHealingBonus = model.GetModifierbyName("HolyPriestAuraHealingBonus").Value;

            // Some notes on holy priest spells:
            // - Everything basically interacts with mastery except renew/pw:s
            // - Most spells are modified by crit/vers
            // - At time of writing the hpriest spec aura is nuked and not relevant
        }

        public override AveragedSpellCastResult CastAverageSpell()
        {
            AveragedSpellCastResult result = base.CastAverageSpell();

            result.RawHealing = AverageRawDirectHeal + AverageRawMasteryHeal;

            return result;
        }

        protected virtual decimal calcAverageRawMasteryHeal()
        {
            if (SpellData.IsMasteryTriggered)
            {
                // TODO: Clean this up a bit, another method maybe?
                decimal retVal = AverageRawDirectHeal * (model.GetMasteryMultiplier(model.RawMastery) - 1);

                return retVal;
            }

            return 0;
        }

        protected override decimal calcAverageTotalHeal()
        {
            var echoOfLightProfile = model.GetCastProfile((int)HolyPriestModel.SpellIds.EchoOfLight);

            var totalDirectHeal = AverageRawDirectHeal * (1 - CastProfile.OverhealPercent);

            var totalMasteryHeal = AverageRawMasteryHeal * (1 - echoOfLightProfile.OverhealPercent);

            return totalDirectHeal + totalMasteryHeal;
        }
    }
}
