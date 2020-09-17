using Salvation.Core.Constants;
using Salvation.Core.Models.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static Salvation.Core.Models.HolyPriest.HolyPriestModel;

[assembly: InternalsVisibleTo("Salvation.CoreTests")]
namespace Salvation.Core.Models.HolyPriest
{
    internal class BaseHolyPriestHealingSpell
        : BaseHealingSpell
    {
        protected HolyPriestModel HolyModel { get { return model as HolyPriestModel; } }
        protected decimal holyPriestAuraHealingBonus { get; }
        protected decimal holyPriestAuraDamageBonus { get; }

        public virtual decimal AverageRawMasteryHeal { get => calcAverageRawMasteryHeal(); }

        public BaseHolyPriestHealingSpell(BaseModel model, BaseSpellData spellData)
            : base(model, spellData)
        {
            holyPriestAuraHealingBonus = model.GetModifierbyName("HolyPriestAuraHealingMultiplier").Value;
            holyPriestAuraDamageBonus = model.GetModifierbyName("HolyPriestAuraDamageMultiplier").Value;

            // Some notes on holy priest spells:
            // - Everything basically interacts with mastery except renew/pw:s
            // - Most spells are modified by crit/vers
            // - At time of writing the hpriest spec aura is nuked and not relevant
        }

        public override AveragedSpellCastResult CastAverageSpell()
        {
            AveragedSpellCastResult result = base.CastAverageSpell();

            if (SpellData.IsMasteryTriggered)
            {
                // Add the echo spell component
                var echoOfLightProfile = model.GetCastProfile((int)HolyPriestModel.SpellIds.EchoOfLight);
                AveragedSpellCastResult echo = new AveragedSpellCastResult();

                echo.SpellId = (int)SpellIds.EchoOfLight;
                echo.SpellName = "Echo of Light";
                echo.RawHealing = AverageRawMasteryHeal;
                echo.Healing = AverageRawMasteryHeal * (1 - echoOfLightProfile.OverhealPercent);

                result.AdditionalCasts.Add(echo);
            }

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

        /// <summary>
        /// Factor in overhealing
        /// </summary>
        /// <returns></returns>
        protected override decimal calcAverageTotalHeal()
        {
           
            var totalDirectHeal = AverageRawDirectHeal * (1 - CastProfile.OverhealPercent);

            //var totalMasteryHeal = AverageRawMasteryHeal * (1 - echoOfLightProfile.OverhealPercent);

            return totalDirectHeal;// + totalMasteryHeal;
        }
    }
}
