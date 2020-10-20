using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;
using System.Collections.Generic;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class FaeGuardians : SpellService, IFaeGuardiansSpellService
    {
        private readonly IDivineHymnSpellService _divineHymnSpellService;

        public FaeGuardians(IGameStateService gameStateService,
            IModellingJournal journal,
            IDivineHymnSpellService divineHymnSpellService)
            : base(gameStateService, journal)
        {
            SpellId = (int)Spell.FaeGuardians;
            _divineHymnSpellService = divineHymnSpellService;
        }

        public override AveragedSpellCastResult GetCastResults(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.FaeGuardians);

            AveragedSpellCastResult result = base.GetCastResults(gameState, spellData);

            // Only real way to model any kind of healing contribution from this is presuming it
            // grants you additional CDR on hymn, rather than being cast on other targets.
            var divineHymnSpellData = _gameStateService.GetSpellData(gameState, Spell.DivineHymn);
            divineHymnSpellData.ManaCost = 0;

            var divineHymnResults = _divineHymnSpellService.GetCastResults(gameState, divineHymnSpellData);

            // Coeff2 is the "100" of 100% CDR.
            var duration = GetDuration(gameState, spellData);

            // Adjust the self duration based on config
            decimal selfUptime = _gameStateService.GetModifier(gameState, "FaeBenevolentFaerieSelfUptime").Value;
            duration *= selfUptime;

            var reducedCooldownSeconds = (spellData.Coeff2 / 100) * duration;

            // Figure out how much extra hymn we get, best case
            var percentageOfCast = reducedCooldownSeconds / divineHymnResults.Cooldown;

            divineHymnResults.RawHealing *= percentageOfCast;
            divineHymnResults.Healing *= percentageOfCast;

            foreach (var subCast in divineHymnResults.AdditionalCasts)
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

        public override decimal GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.FaeGuardians);

            // Night Fae has 3 components:
            // Wrathful - returns some mana so healing from that returned mana?
            // Guardian - 10% DR on target, so "healing" from damage saved?
            // Benevolent - CDR, we can potentially presume it means more Hymn casts?

            // Wrathful
            // We can ignore this one as it costs more mana to cast sw:p and smite for the duration 
            // than what it returns. Zero healing impact and no dps benefit

            // Guardian
            // 10% DR on a target requires a known DTPS. 
            // Duration * DTPS * DR
            // DR comes in as -10, so / -100.
            // Duration should be minus the GCD of the initial cast + gcd to move pw:s over.

            // TODO: Move this to configuration
            decimal targetDamageTakenPerSecond = _gameStateService.GetModifier(gameState, "FaeGuardianFaerieDTPS").Value;

            var duration = GetDuration(gameState, spellData);

            _journal.Entry($"[{spellData.Name}] DR: {spellData.Coeff1}% DTPS: {targetDamageTakenPerSecond} Duration: {duration}s");

            decimal averageDRPC = duration
                * targetDamageTakenPerSecond
                * (spellData.Coeff1 / -100);

            // Benevolent
            // See GetAverageSpell()

            return averageDRPC;
        }

        public override decimal GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.FaeGuardians);

            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            decimal maximumPotentialCasts = 60m / hastedCd
                + 1m / (fightLength / 60m);

            return maximumPotentialCasts;
        }

        public override decimal GetDuration(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.FaeGuardians);

            var baseDuration = base.GetDuration(gameState, spellData);

            // Apply the Fae Fermata conduit if applicable
            // TODO: Shift this out to another method maybe, for testing?
            if (_gameStateService.IsConduitActive(gameState, Conduit.FaeFermata))
            {

                var rank = _gameStateService.GetConduitRank(gameState, Conduit.FaeFermata);
                var conduitData = _gameStateService.GetConduitData(gameState, Conduit.FaeFermata);

                var addedDuration = conduitData.Ranks[rank] / 1000;
                baseDuration += addedDuration;
            }
            return baseDuration;
        }
    }
}
