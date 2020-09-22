using Salvation.Core.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    internal class DivineHymn 
        : BaseHolyPriestHealingSpell
    {
        public DivineHymn(BaseModel model, BaseSpellData spellData = null)
            : base(model, spellData)
        {
            if (spellData == null)
                SpellData = model.GetSpecSpellDataById((int)HolyPriestModel.SpellIds.DivineHymn);
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            // DH's average heal for the first tick is:
            // SP% * Intellect * Vers * Hpriest Aura
            decimal firstTick = SpellData.Coeff1 
                * model.RawInt 
                * model.GetVersMultiplier(model.RawVers)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus;

            // For the remaining ticks they're all modified by the +healing% aura
            var divineHymnAura = model.GetModifierbyName("DivineHymnBonusHealing");

            // Now the rest of the 4 ticks including the aura:
            decimal averageHeal = firstTick + (firstTick * 4 * (1 + divineHymnAura.Value));

            // double it if we have 5 or less (dungeon group buff)
            averageHeal *= SpellData.NumberOfHealingTargets <= 5 ? 2 : 1;

            return averageHeal * SpellData.NumberOfHealingTargets;
        }

        protected override decimal calcCastsPerMinute()
        {
            decimal castsPerMinute = CastProfile.Efficiency * MaximumCastsPerMinute;

            return castsPerMinute;
        }

        protected override decimal calcMaximumCastsPerMinute()
        {
            // DH is simply 60 / CD + 1 / (FightLength / 60)
            // Number of casts per minute plus one cast at the start of the encounter
            decimal maximumPotentialCasts = 60m / HastedCooldown
                + 1m / (model.FightLengthSeconds / 60m);

            return maximumPotentialCasts;
        }
    }
}
