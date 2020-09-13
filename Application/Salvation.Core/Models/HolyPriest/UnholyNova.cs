﻿using Salvation.Core.Models.Common;
using Salvation.Core.Models.HolyPriest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class UnholyNova
        : BaseHolyPriestHealingSpell
    {
        public UnholyNova(BaseModel model, decimal numberOfTargetsHit = 0)
            : base (model, numberOfTargetsHit)
        {
            SpellData = model.GetSpecSpellDataById((int)HolyPriestModel.SpellIds.UnholyNova);
        }
        public override AveragedSpellCastResult CastAverageSpell()
        {
            AveragedSpellCastResult result = base.CastAverageSpell();

            // Apply the transufion DoT/HoT
            var unholyTransfusion = new UnholyTransfusion(HolyModel, NumberOfTargets);

            var uhtResults = unholyTransfusion.CastAverageSpell();

            result.AdditionalCasts.Add(uhtResults);

            return result;
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            // Healing from the initial heal
            decimal averageHeal = SpellData.Coeff1 
                * model.RawInt
                * model.GetVersMultiplier(model.RawVers)
                * holyPriestAuraHealingBonus;

            averageHeal *= model.GetCritMultiplier(model.RawCrit);

            return averageHeal * NumberOfTargets;
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
            // Number of casts per minute plus one cast at the start of the encounter
            decimal maximumPotentialCasts = 60m / HastedCooldown
                + 1m / (model.FightLengthSeconds / 60m);

            return maximumPotentialCasts;
        }
    }
}
