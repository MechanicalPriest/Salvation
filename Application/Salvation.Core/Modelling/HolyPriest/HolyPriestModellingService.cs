using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Salvation.Core.Modelling.HolyPriest
{
    public class HolyPriestModellingService : IModellingService
    {
        private readonly IGameStateService gameStateService;
        private readonly IModellingJournal journal;

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
            this.gameStateService = gameStateService;
            this.journal = journal;

            Spells = new List<ISpellService>();
            Spells.Add(flashHealService);
            Spells.Add(healSpellService);
            Spells.Add(renewSpellService);
            Spells.Add(prayerOfHealingSpellService);
            Spells.Add(holyNovaSpellService);
            Spells.Add(powerWordShieldSpellService);
            Spells.Add(bindingHealSpellService);
            Spells.Add(prayerOfMendingSpellService);
            Spells.Add(circleOfHealingSpellService);
            Spells.Add(divineStarSpellService);
            Spells.Add(haloSpellService);
            Spells.Add(holyWordSerenitySpellService);
            Spells.Add(holyWordSanctifySpellService);
            Spells.Add(divineHymnSpellService);
            Spells.Add(holyWordSalvationSpellService);
            Spells.Add(faeGuardiansSpellService);
            Spells.Add(mindgamesSpellService);
            Spells.Add(unholyNovaSpellService);
            Spells.Add(boonOfTheAscendedSpellService);
        }

        public BaseModelResults GetResults(GameState state)
        {
            var results = new BaseModelResults();
            results.Profile = state.Profile;

            journal.Entry($"Results run started at {DateTime.Now:yyyy.MM.dd HH:mm:ss:ffff}.");
            var sw = new Stopwatch();
            sw.Start();

            foreach (var spell in Spells)
            {
                if (IsSpellBeingCast(state, (SpellIds)spell.SpellId))
                {
                    var castResults = spell.GetCastResults(state);
                    results.SpellCastResults.Add(castResults);
                }
                else
                {
                    journal.Entry($"[{spell.SpellId}] Skipped casting due to profile.");
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
            var rawMana = gameStateService.GetBaseManaAmount(state);
            decimal totalRegenPerSecond = rawMana * 0.04m * 1 / 5m;

            var totalNegativeManaPerSecond = results.TotalMPS - totalRegenPerSecond;
            results.TimeToOom = rawMana / totalNegativeManaPerSecond;

            sw.Stop();
            journal.Entry($"Results: RawHPS ({results.TotalRawHPS:0.##}) HPS ({results.TotalActualHPS:0.##}) " +
                $"MPS ({results.TotalMPS:0.##})");
            journal.Entry($"Results: RawHPM ({results.TotalRawHPM:0.##}) HPM ({results.TotalActualHPM:0.##})");
            journal.Entry($"Results: TtOoM {results.TimeToOom}s.");

            journal.Entry($"Results run done in {sw.ElapsedMilliseconds}ms.");

            return results;
        }

        /// <summary>
        /// Check to see if this spell should be cast as part of the modelling
        /// </summary>
        public bool IsSpellBeingCast(GameState state, SpellIds spellId)
        {
            switch (spellId)
            {
                case SpellIds.BoonOfTheAscended:
                    return gameStateService.GetActiveCovenant(state) == Covenant.Kyrian;

                case SpellIds.Mindgames:
                    return gameStateService.GetActiveCovenant(state) == Covenant.Venthyr;
                
                case SpellIds.FaeGuardians:
                    return gameStateService.GetActiveCovenant(state) == Covenant.NightFae;
                
                case SpellIds.UnholyNova:
                    return gameStateService.GetActiveCovenant(state) == Covenant.Necrolord;
                
                default:
                    return true;
            }
        }

        private void RollUpResults(BaseModelResults results, List<AveragedSpellCastResult> spells)
        {
            var newSpells = new List<AveragedSpellCastResult>();

            foreach(var spellResult in spells)
            {
                newSpells.Add(RollUpResultsSummary(spellResult));
            }

            results.RolledUpResultsSummary = newSpells;

            foreach(var spellResult in results.RolledUpResultsSummary)
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
            var resultSummary = new AveragedSpellCastResult();

            //Console.WriteLine($"[{castResult.SpellName}] Rolling up");

            // Things that are properties of the cast
            resultSummary.CastsPerMinute = castResult.CastsPerMinute;
            resultSummary.CastTime = castResult.CastTime;
            resultSummary.Cooldown = castResult.Cooldown;
            resultSummary.Duration = castResult.Duration;
            resultSummary.Gcd = castResult.Gcd;
            resultSummary.MaximumCastsPerMinute = castResult.MaximumCastsPerMinute;
            resultSummary.NumberOfDamageTargets = castResult.NumberOfDamageTargets;
            resultSummary.NumberOfHealingTargets = castResult.NumberOfHealingTargets;
            resultSummary.SpellId = castResult.SpellId;
            resultSummary.SpellName = castResult.SpellName;

            // Properties that are sums of all the parts
            var spellParts = new List<AveragedSpellCastResult>();
            spellParts.Add(castResult);

            // If this spell is actually being cast, roll up its parts to calculate total HPS
            if(castResult.CastsPerMinute > 0)
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
