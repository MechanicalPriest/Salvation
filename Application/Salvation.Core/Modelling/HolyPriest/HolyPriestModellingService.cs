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

        private BaseModelResults RollUpResults(BaseModelResults results, List<AveragedSpellCastResult> spells)
        {
            foreach(var spellResult in spells)
            {
                results.TotalActualHPS += spellResult.HPS;
                results.TotalRawHPS += spellResult.RawHPS;
                results.TotalMPS += spellResult.MPS;

                if(spellResult.AdditionalCasts.Count > 0)
                {
                    RollUpResults(results, spellResult.AdditionalCasts);
                }
            }

            return results;
        }
    }
}
