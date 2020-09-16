using Salvation.Core.Constants;
using Salvation.Core.Models.Common;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    internal class PowerWordShield 
        : BaseHolyPriestHealingSpell
    {
        public PowerWordShield(BaseModel model, BaseSpellData spellData = null)
            : base(model, spellData)
        {
            if (spellData == null)
                SpellData = model.GetSpecSpellDataById((int)HolyPriestModel.SpellIds.PowerWordShield);
        }

        public override AveragedSpellCastResult CastAverageSpell()
        {
            AveragedSpellCastResult result = base.CastAverageSpell();

            if(model.Profile.IsConduitActive(Conduit.CharitableSoul))
            {
                var csSpellData = model.GetConduitDataById((int)Conduit.CharitableSoul);

                // Turn the rank value into a multiplier. "Rank" 10 = 0.10
                var rank = model.Profile.Conduits[Conduit.CharitableSoul];
                var rankMulti = csSpellData.Ranks[rank] / 100;

                AveragedSpellCastResult csComponent = new AveragedSpellCastResult();
                csComponent.SpellId = csSpellData.Id;
                csComponent.SpellName = csSpellData.Name;
                csComponent.RawHealing = result.RawHealing * rankMulti;
                csComponent.Healing = result.Healing * rankMulti;
                csComponent.Cooldown = 0;
                csComponent.Duration = 0;
                csComponent.Gcd = 0;
                csComponent.ManaCost = 0;
                csComponent.NumberOfHealingTargets = 1;
                csComponent.MakeSpellHaveNoCasts();

                result.AdditionalCasts.Add(csComponent);
            }

            return result;
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            decimal averageHeal = SpellData.Coeff1 
                * model.RawInt
                * model.GetVersMultiplier(model.RawVers)
                * model.GetCritMultiplier(model.RawCrit)
                * holyPriestAuraHealingBonus
                * 2; // PW:S has a x2 SP% multiplier built in to it

            return averageHeal * SpellData.NumberOfHealingTargets;
        }

        protected override decimal calcCastsPerMinute()
        {
            decimal castsPerMinute = CastProfile.Efficiency * MaximumCastsPerMinute;

            return castsPerMinute;
        }

        protected override decimal calcMaximumCastsPerMinute()
        {
            // If it's instant cast, instead use the hasted GCD as the limiting factor
            decimal fillerCastTime = HastedCastTime == 0
                ? HastedGcd
                : HastedCastTime;

            decimal maximumPotentialCasts = 60m / fillerCastTime;

            return maximumPotentialCasts;
        }
    }
}
