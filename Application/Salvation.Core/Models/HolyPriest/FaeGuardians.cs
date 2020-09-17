using Salvation.Core.Constants;
using Salvation.Core.Models.Common;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    internal class FaeGuardians
        : BaseHolyPriestHealingSpell
    {
        public FaeGuardians(BaseModel model, BaseSpellData spellData = null)
            : base(model, spellData)
        {
            if (spellData == null)
                SpellData = model.GetSpecSpellDataById((int)HolyPriestModel.SpellIds.FaeGuardians);
        }
        public override AveragedSpellCastResult CastAverageSpell()
        {
            AveragedSpellCastResult result = base.CastAverageSpell();

            // Only real way to model any kind of healing contribution from this is persuming it
            // grants you additional CDR on hymn, rather than being cast on other targets.
            var divineHymn = model.GetSpell<DivineHymn>(HolyPriestModel.SpellIds.DivineHymn);
            var divineHymnResults = divineHymn.CastAverageSpell();

            // Coeff2 is the "100" of 100% CDR.
            var duration = applyFaeFermataConduitDuration(Duration);
            var reducedCooldownSeconds = (SpellData.Coeff2 / 100) * duration;

            // Figure out how much extra hymn we get, best case
            var percentageOfCast = reducedCooldownSeconds / divineHymnResults.Cooldown;

            divineHymnResults.RawHealing *= percentageOfCast;
            divineHymnResults.Healing *= percentageOfCast;

            foreach(var subCast in divineHymnResults.AdditionalCasts)
            {
                subCast.RawHealing *= percentageOfCast;
                subCast.Healing *= percentageOfCast;
            }

            divineHymnResults.MakeCastFree();
            divineHymnResults.MakeCastHaveNoGcd();
            divineHymnResults.MakeCastInstant();
            divineHymnResults.MakeSpellHaveNoCasts();

            result.AdditionalCasts.Add(divineHymnResults);

            return result;
        }

        protected override decimal calcAverageRawDirectHeal()
        {
            // Night Fae has 3 components:
            // Wrathful - returns some mana so healing from that returned mana?
            // Guardian - 10% DR on target, so "healing" from damage saved?
            // Benevolent - CDR, we can potentially presume it means more Hymn casts?

            // Wrathful
            // We can ignore this one as it costs more mana to cast sw:p and smite for the duration 
            // than what it returns. Zero healing impact

            // Guardian
            // 10% DR on a target requires a known DTPS. 
            // Duration * DTPS * DR
            // DR comes in as -10, so / -100.
            // Duration should be minus the GCD of the initial cast + gcd to move pw:s over.

            // TODO: Move this to configuration
            decimal targetDamageTakenPerSecond = 3000.0m;
            var pwsCast = model.GetSpell<PowerWordShield>(HolyPriestModel.SpellIds.PowerWordShield);
            var duration = applyFaeFermataConduitDuration(Duration);

            decimal averageDRPC = (duration - pwsCast.CastAverageSpell().CastTime - HastedGcd) 
                * targetDamageTakenPerSecond 
                * (SpellData.Coeff1 / -100);

            // Benevolent
            // See GetAverageSpell()

            return averageDRPC;
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
            // Mindgames CD isn't haste affected so it isis simply 
            // 60 / CD + 1 / (FightLength / 60)
            // Number of casts per minute plus one cast at the start of the encounter
            decimal maximumPotentialCasts = 60m / HastedCooldown
                + 1m / (model.FightLengthSeconds / 60m);

            return maximumPotentialCasts;
        }
        private decimal applyFaeFermataConduitDuration(decimal duration)
        {
            if (model.Profile.IsConduitActive(Conduit.FaeFermata))
            {

                var rank = model.Profile.Conduits[Conduit.FaeFermata];
                var conduitData = model.GetConduitDataById((int)Conduit.FaeFermata);

                duration += conduitData.Ranks[rank] / 1000;
            }
            
            return duration;
        }
    }
}
