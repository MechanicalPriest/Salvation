using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.State;

namespace Salvation.Core.Modelling.HolyPriest.Spells
{
    public class FaeGuardians : SpellService, IFaeGuardiansSpellService
    {
        private readonly IDivineHymnSpellService _divineHymnSpellService;

        public FaeGuardians(IGameStateService gameStateService,
            IDivineHymnSpellService divineHymnSpellService)
            : base(gameStateService)
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
            var buffDuration = GetDuration(gameState, spellData);

            // Apply some extra CDR time based on FaeFermata being active
            // Essentially its number_of_swaps * (buff_duration * effectiveness_percent)
            if (_gameStateService.IsConduitActive(gameState, Conduit.FaeFermata))
            {
                var numTargetSwaps = _gameStateService.GetPlaystyle(gameState, "FaeFermataNumberCDRSwaps").Value;
                buffDuration += GetFaeFermataBonus(gameState) * numTargetSwaps;
            }

            // Adjust the self duration based on config
            double selfUptime = _gameStateService.GetPlaystyle(gameState, "FaeBenevolentFaerieSelfUptime").Value;
            buffDuration *= selfUptime;

            var beneFaerieData = _gameStateService.GetSpellData(gameState, Spell.BenevolentFaerie);

            // This value is the CDR increase, 100 = 100%
            var cdrModifier = beneFaerieData.GetEffect(819312).BaseValue;

            var reducedCooldownSeconds = (cdrModifier / 100) * buffDuration;

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

        public override double GetAverageRawHealing(GameState gameState, BaseSpellData spellData = null)
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
            double targetDamageTakenPerSecond = _gameStateService.GetPlaystyle(gameState, "FaeGuardianFaerieDTPS").Value;

            var buffDuration = GetDuration(gameState, spellData);

            // Apply some extra DR time based on FaeFermata being active
            // Essentially its number_of_swaps * (buff_duration * effectiveness_percent)
            if (_gameStateService.IsConduitActive(gameState, Conduit.FaeFermata))
            {
                var numTargetSwaps = _gameStateService.GetPlaystyle(gameState, "FaeFermataNumberDRSwaps").Value;
                buffDuration += GetFaeFermataBonus(gameState) * numTargetSwaps;
            }

            var guardianFaerieData = _gameStateService.GetSpellData(gameState, Spell.GuardianFaerie);

            // This value is a negative integer. -10 = -10%
            var baseDamageReduction = guardianFaerieData.GetEffect(819281).BaseValue;

            _gameStateService.JournalEntry(gameState, $"[{spellData.Name}] DR: {baseDamageReduction}% DTPS: {targetDamageTakenPerSecond} Duration: {buffDuration}s");

            double averageDRPC = buffDuration
                * targetDamageTakenPerSecond
                * (baseDamageReduction / -100d);

            // Benevolent
            // See GetAverageSpell()

            return averageDRPC * GetNumberOfHealingTargets(gameState, spellData);
        }

        public override double GetMaximumCastsPerMinute(GameState gameState, BaseSpellData spellData = null)
        {
            if (spellData == null)
                spellData = _gameStateService.GetSpellData(gameState, Spell.FaeGuardians);

            var hastedCd = GetHastedCooldown(gameState, spellData);
            var fightLength = gameState.Profile.FightLengthSeconds;

            double maximumPotentialCasts = 60d / hastedCd
                + 1d / (fightLength / 60d);

            return maximumPotentialCasts;
        }

        public override double GetMinimumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumHealTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        public override double GetMaximumDamageTargets(GameState gameState, BaseSpellData spellData)
        {
            return 1;
        }

        /// <summary>
        /// This value should be multiplied by the number of refresh events.
        /// This is is the extra duration multiplied by the effectiveness of the conduit
        /// </summary>
        /// <param name="gameState"></param>
        /// <returns></returns>
        internal double GetFaeFermataBonus(GameState gameState)
        {
            var duration = 0d;

            // Apply the Fae Fermata conduit if applicable
            // For this we essentially just calculate the additional duration and 
            // multiply that by the effectiveness to get average added "duration"
            if (_gameStateService.IsConduitActive(gameState, Conduit.FaeFermata))
            {
                var rank = _gameStateService.GetConduitRank(gameState, Conduit.FaeFermata);
                var conduitData = _gameStateService.GetSpellData(gameState, Spell.FaeFermata);

                var effectiveness = conduitData.GetEffect(871353).BaseValue / 100;

                var addedDuration = conduitData.ConduitRanks[rank] / 1000;

                duration = addedDuration * effectiveness;
            }

            return duration;
        }
    }
}
