using Microsoft.Extensions.Logging;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Modelling.HolyPriest.Spells;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Salvation.Core.Modelling.HolyPriest
{
    public class HolyPriestModellingService : IModellingService
    {
        private readonly IGameStateService _gameStateService;
        private readonly ILogger<HolyPriestModellingService> _logger;

        public HolyPriestModellingService(IGameStateService gameStateService,
            ILogger<HolyPriestModellingService> logger)
        {
            _gameStateService = gameStateService;
            _logger = logger;
        }

        public BaseModelResults GetResults(GameState state)
        {
            var results = new BaseModelResults
            {
                Profile = state.Profile
            };

            _gameStateService.JournalEntry(state, $"Results run started at {DateTime.Now:yyyy.MM.dd HH:mm:ss:ffff}.");
            var sw = new Stopwatch();
            sw.Start();

            // Display some basic information.
            DisplayState(state);
            
            _gameStateService.RegisterSpells(state, GetBaseSpells());

            foreach (var spell in _gameStateService.GetRegisteredSpells(state))
            {
                _gameStateService.JournalEntry(state, $"[{spell.Spell}] Generating result...");

                if (spell.SpellService != null)
                {
                    var castResults = spell.SpellService.GetCastResults(state, spell.SpellData);
                    results.SpellCastResults.Add(castResults);
                }
                else
                {
                    _gameStateService.JournalEntry(state, $"[{spell.Spell}] Skipped casting due to no spellservice.");
                }
            }

            // Create a sumamry for each spell cast that's a sum of its children
            RollUpResults(results);
            results.RolledUpResultsSummary = results.RolledUpResultsSummary.OrderByDescending(r => r.HPS).ToList();

            results.TotalRawHPM = results.TotalRawHPS / results.TotalMPS;
            results.TotalActualHPM = results.TotalActualHPS / results.TotalMPS;

            // Mana regen / time to oom
            // TODO: re-implement Enlightenment
            //var hasEnlightenment = Profile.IsTalentActive(Talent.Enlightenment);
            //var regenCoeff = hasEnlightenment ? 1.1m : 1m; // This is the 1 below

            // TODO: Add a get total mana pool amount for cases where mana pool isn't base
            var rawMana = _gameStateService.GetBaseManaAmount(state);
            double totalRegenPerSecond = rawMana * 0.04d * 1 / 5d;

            var totalNegativeManaPerSecond = results.TotalMPS - totalRegenPerSecond;
            results.TimeToOom = rawMana / totalNegativeManaPerSecond;

            sw.Stop();
            _gameStateService.JournalEntry(state, $"Results: RawHPS ({results.TotalRawHPS:0.##}) HPS ({results.TotalActualHPS:0.##}) " +
                $"MPS ({results.TotalMPS:0.##})");
            _gameStateService.JournalEntry(state, $"Results: RawHPM ({results.TotalRawHPM:0.##}) HPM ({results.TotalActualHPM:0.##})");
            _gameStateService.JournalEntry(state, $"Results: TtOoM {results.TimeToOom}s.");

            _gameStateService.JournalEntry(state, $"Results run done in {sw.ElapsedMilliseconds}ms.");

            return results;
        }

        private void DisplayState(GameState state)
        {
            // Show some basic info.
            // Stats
            _gameStateService.JournalEntry(state, $"Intellect: {_gameStateService.GetIntellect(state)}");
            _gameStateService.JournalEntry(state, $"Crit: {_gameStateService.GetCriticalStrikeRating(state)} ({_gameStateService.GetCriticalStrikeMultiplier(state)}%)");
            _gameStateService.JournalEntry(state, $"Haste: {_gameStateService.GetHasteRating(state)} ({_gameStateService.GetHasteMultiplier(state)}%)");
            _gameStateService.JournalEntry(state, $"Mastery: {_gameStateService.GetMasteryRating(state)} ({_gameStateService.GetMasteryMultiplier(state)}%)");
            _gameStateService.JournalEntry(state, $"Versatility: {_gameStateService.GetVersatilityRating(state)} ({_gameStateService.GetVersatilityMultiplier(state)}%)");

            // Gear
            foreach(var item in state.Profile.Items)
            {
                var statsMessage = "";
                foreach(var stat in item.Mods.Select(m => m.Type).Distinct())
                {
                    var rating = item.Mods.Where(m => m.Type == stat).Select(s => s.StatRating).Sum();
                    if(rating > 0)
                        statsMessage += String.Format("{0}: {1} ", 
                            stat.ToString().Replace("ITEM_MOD_", "").Replace("_RATING", "").Replace("STRENGTH_AGILITY_", "").ToLower(),
                            rating);
                }

                var message = String.Format("{2}: {0} ({1}) - {3}",
                    item.Name,
                    item.ItemLevel,
                    item.Slot,
                    statsMessage);

                _gameStateService.JournalEntry(state, message);
            }
        }

        internal List<RegisteredSpell> GetBaseSpells()
        {
            var result = new List<RegisteredSpell>()
            {
                // Healing Spells
                new RegisteredSpell(Spell.FlashHeal),
                new RegisteredSpell(Spell.Heal),
                new RegisteredSpell(Spell.PowerWordShield),
                new RegisteredSpell(Spell.PrayerOfMending), // Default talent, but can be taken with no talents chosen still
                new RegisteredSpell(Spell.Renew), // Default talent, but can be taken with no talents chosen still

                // DPS Spells
                new RegisteredSpell(Spell.HolyFire),
                new RegisteredSpell(Spell.ShadowWordPain),
                new RegisteredSpell(Spell.Smite),
            };

            return result;
        }

        #region Spell Result Calculatoins

        private void RollUpResults(BaseModelResults results)
        {
            var newSpells = new List<AveragedSpellCastResult>();

            foreach (var spellResult in results.SpellCastResults)
            {
                newSpells.Add(RollUpResultsSummary(spellResult));
            }

            results.RolledUpResultsSummary = newSpells;

            foreach (var spellResult in results.RolledUpResultsSummary)
            {
                if (!double.IsNaN(spellResult.HPS))
                    results.TotalActualHPS += spellResult.HPS;
                else
                    Console.WriteLine($"Error getting spell result HPS from {spellResult.SpellName}");

                if (!double.IsNaN(spellResult.RawHPS))
                    results.TotalRawHPS += spellResult.RawHPS;
                else
                    Console.WriteLine($"Error getting spell result RawHPS from {spellResult.SpellName}");

                if (!double.IsNaN(spellResult.MPS))
                    results.TotalMPS += spellResult.MPS;
                else
                    Console.WriteLine($"Error getting spell result MPS from {spellResult.SpellName}");
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
                SpellName = castResult.SpellName,
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
        private void RollUpSpellParts(AveragedSpellCastResult resultSummary, List<AveragedSpellCastResult> spellParts, double parentCPM = 0)
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
                resultSummary.Mp5 += part.Mp5;

                //Console.WriteLine($"[{resultSummary.SpellName}] child added doing {part.RawHealing:0.##} " +
                //        $"raw healing (total now: {resultSummary.RawHealing:0.##}) from {part.SpellName} " +
                //        $"with {partCPM:0.##}CPM (actual = {part.CastsPerMinute:0.##})");

                // Now roll up all of this parts children, setting this part as the parent for CPM purposes
                if (part.AdditionalCasts.Count > 0)
                    RollUpSpellParts(resultSummary, part.AdditionalCasts, partCPM);
            }
        }

        #endregion
    }
}
