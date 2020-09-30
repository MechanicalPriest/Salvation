using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Salvation.Core.Modelling.HolyPriest
{
    public class HolyPriestModellingService : IModellingService
    {
        private readonly IGameStateService _gameStateService;
        private readonly IModellingJournal _journal;

        public List<ISpellService> Spells { get; private set; }

        public HolyPriestModellingService(IGameStateService gameStateService,
            IModellingJournal journal,
            IFlashHealSpellService flashHealService,
            IHolyWordSerenitySpellService holyWordSerenitySpellService,
            IHolyWordSalvationSpellService holyWordSalvationSpellService,
            IRenewSpellService renewSpellService,
            IPrayerOfMendingSpellService prayerOfMendingSpellService,
            IPrayerOfHealingSpellService prayerOfHealingSpellService,
            IHealSpellService healSpellService,
            IBindingHealSpellService bindingHealSpellService,
            IHolyWordSanctifySpellService holyWordSanctifySpellService,
            ICircleOfHealingSpellService circleOfHealingSpellService,
            IDivineHymnSpellService divineHymnSpellService,
            IDivineStarSpellService divineStarSpellService,
            IHaloSpellService haloSpellService,
            IHolyNovaSpellService holyNovaSpellService,
            IPowerWordShieldSpellService powerWordShieldSpellService,
            IFaeGuardiansSpellService faeGuardiansSpellService,
            IMindgamesSpellService mindgamesSpellService,
            IUnholyNovaSpellService unholyNovaSpellService,
            IBoonOfTheAscendedSpellService boonOfTheAscendedSpellService)
        {
            _gameStateService = gameStateService;
            _journal = journal;

            Spells = new List<ISpellService>
            {
                flashHealService,
                healSpellService,
                renewSpellService,
                prayerOfHealingSpellService,
                holyNovaSpellService,
                powerWordShieldSpellService,
                bindingHealSpellService,
                prayerOfMendingSpellService,
                circleOfHealingSpellService,
                divineStarSpellService,
                haloSpellService,
                holyWordSerenitySpellService,
                holyWordSanctifySpellService,
                divineHymnSpellService,
                holyWordSalvationSpellService,
                faeGuardiansSpellService,
                mindgamesSpellService,
                unholyNovaSpellService,
                boonOfTheAscendedSpellService
            };
        }

        public BaseModelResults GetResults(GameState state)
        {
            var results = new BaseModelResults
            {
                Profile = state.Profile
            };

            _journal.Entry($"Results run started at {DateTime.Now:yyyy.MM.dd HH:mm:ss:ffff}.");
            var sw = new Stopwatch();
            sw.Start();

            foreach (var spell in Spells)
            {
                if (IsSpellBeingCast(state, (Spell)spell.SpellId))
                {
                    var castResults = spell.GetCastResults(state);
                    results.SpellCastResults.Add(castResults);
                }
                else
                {
                    _journal.Entry($"[{spell.SpellId}] Skipped casting due to profile.");
                }
            }

            // Create a sumamry for each spell cast that's a sum of its children
            RollUpResults(results, results.SpellCastResults);

            results.TotalRawHPM = results.TotalRawHPS / results.TotalMPS;
            results.TotalActualHPM = results.TotalActualHPS / results.TotalMPS;

            // Mana regen / time to oom
            // TODO: re-implement Enlightenment
            //var hasEnlightenment = Profile.IsTalentActive(Talent.Enlightenment);
            //var regenCoeff = hasEnlightenment ? 1.1m : 1m; // This is the 1 below

            // TODO: Add a get total mana pool amount for cases where mana pool isn't base
            var rawMana = _gameStateService.GetBaseManaAmount(state);
            decimal totalRegenPerSecond = rawMana * 0.04m * 1 / 5m;

            var totalNegativeManaPerSecond = results.TotalMPS - totalRegenPerSecond;
            results.TimeToOom = rawMana / totalNegativeManaPerSecond;

            sw.Stop();
            _journal.Entry($"Results: RawHPS ({results.TotalRawHPS:0.##}) HPS ({results.TotalActualHPS:0.##}) " +
                $"MPS ({results.TotalMPS:0.##})");
            _journal.Entry($"Results: RawHPM ({results.TotalRawHPM:0.##}) HPM ({results.TotalActualHPM:0.##})");
            _journal.Entry($"Results: TtOoM {results.TimeToOom}s.");

            _journal.Entry($"Results run done in {sw.ElapsedMilliseconds}ms.");

            return results;
        }

        /// <summary>
        /// Check to see if this spell should be cast as part of the modelling
        /// </summary>
        public bool IsSpellBeingCast(GameState state, Spell spellId)
        {
            switch (spellId)
            {
                case Spell.BoonOfTheAscended:
                    return _gameStateService.GetActiveCovenant(state) == Covenant.Kyrian;

                case Spell.Mindgames:
                    return _gameStateService.GetActiveCovenant(state) == Covenant.Venthyr;

                case Spell.FaeGuardians:
                    return _gameStateService.GetActiveCovenant(state) == Covenant.NightFae;

                case Spell.UnholyNova:
                    return _gameStateService.GetActiveCovenant(state) == Covenant.Necrolord;

                default:
                    return true;
            }
        }

        private void RollUpResults(BaseModelResults results, List<AveragedSpellCastResult> spells)
        {
            var newSpells = new List<AveragedSpellCastResult>();

            foreach (var spellResult in spells)
            {
                newSpells.Add(RollUpResultsSummary(spellResult));
            }

            results.RolledUpResultsSummary = newSpells;

            foreach (var spellResult in results.RolledUpResultsSummary)
            {
                results.TotalActualHPS += spellResult.HPS;
                results.TotalRawHPS += spellResult.RawHPS;
                results.TotalMPS += spellResult.MPS;
            }
        }

        /// <summary>
        /// Create a AveragedSpellCastResult that is a combined total of all its children
        /// </summary>
        /// <param name="castResult"></param>
        /// <returns></returns>
        private AveragedSpellCastResult RollUpResultsSummary(AveragedSpellCastResult castResult)
        {
            //Console.WriteLine($"[{castResult.SpellName}] Rolling up");

            // Things that are properties of the cast
            var resultSummary = new AveragedSpellCastResult
            {
                CastsPerMinute = castResult.CastsPerMinute,
                CastTime = castResult.CastTime,
                Cooldown = castResult.Cooldown,
                Duration = castResult.Duration,
                Gcd = castResult.Gcd,
                MaximumCastsPerMinute = castResult.MaximumCastsPerMinute,
                NumberOfDamageTargets = castResult.NumberOfDamageTargets,
                NumberOfHealingTargets = castResult.NumberOfHealingTargets,
                SpellId = castResult.SpellId,
                SpellName = castResult.SpellName
            };

            // Properties that are sums of all the parts
            var spellParts = new List<AveragedSpellCastResult>
            {
                castResult
            };

            // If this spell is actually being cast, roll up its parts to calculate total HPS
            if (castResult.CastsPerMinute > 0)
                RollUpSpellParts(resultSummary, spellParts);
            else
            {
                // If it's not, set a bunch of "Per cast" values.
                resultSummary.Healing = castResult.Healing;
                resultSummary.RawHealing = castResult.RawHealing;
                resultSummary.Overhealing = castResult.Overhealing;
                resultSummary.Damage = castResult.Damage;
                resultSummary.ManaCost = castResult.ManaCost;
            }

            resultSummary.AdditionalCasts.Add(castResult);

            //Console.WriteLine($"[{resultSummary.SpellName}] added doing {resultSummary.RawHealing:0.##} raw healing");

            return resultSummary;
        }

        /// <summary>
        /// Roll up recursive subchildren into the main result summary for the spell
        /// </summary>
        private void RollUpSpellParts(AveragedSpellCastResult resultSummary, List<AveragedSpellCastResult> spellParts, decimal parentCPM = 0)
        {
            // Loop over each child and figure out how much it provides to the ResultSUmmary for this spell
            foreach (var part in spellParts)
            {
                var partCPM = part.CastsPerMinute;
                // Try to set the CPM from the parent if its 0
                if (part.CastsPerMinute == 0)
                {
                    // If there is a parent set, use its CPM then.
                    partCPM = parentCPM;
                }

                // Add the amount of healing per cast OF THE SUMMARY SPELL to it
                resultSummary.Healing += part.Healing * partCPM / resultSummary.CastsPerMinute;
                resultSummary.RawHealing += part.RawHealing * partCPM / resultSummary.CastsPerMinute;
                resultSummary.Overhealing += part.Overhealing * partCPM / resultSummary.CastsPerMinute;
                resultSummary.Damage += part.Damage * partCPM / resultSummary.CastsPerMinute;
                resultSummary.ManaCost += part.ManaCost * partCPM / resultSummary.CastsPerMinute;

                //Console.WriteLine($"[{resultSummary.SpellName}] child added doing {part.RawHealing:0.##} " +
                //        $"raw healing (total now: {resultSummary.RawHealing:0.##}) from {part.SpellName} " +
                //        $"with {partCPM:0.##}CPM (actual = {part.CastsPerMinute:0.##})");

                // Now roll up all of this parts children, setting this part as the parent for CPM purposes
                if (part.AdditionalCasts.Count > 0)
                    RollUpSpellParts(resultSummary, part.AdditionalCasts, partCPM);
            }
        }
    }
}
