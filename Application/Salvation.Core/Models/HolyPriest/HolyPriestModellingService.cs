using Salvation.Core.Interfaces;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
using Salvation.Core.Models.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Salvation.Core.Models.HolyPriest
{
    public class HolyPriestModellingService : IModellingService
    {
        private readonly IConstantsService constantsService;
        private readonly IModellingJournal journal;

        public List<ISpellService> Spells { get; private set; }

        public HolyPriestModellingService(IConstantsService constantsService,
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
            IDivineStarSpellService divineStarSpellService)
        {
            this.constantsService = constantsService;
            this.journal = journal;
            Spells = new List<ISpellService>();
            Spells.Add(flashHealService);
            Spells.Add(healSpellService);
            Spells.Add(renewSpellService);
            Spells.Add(prayerOfHealingSpellService);
            Spells.Add(bindingHealSpellService);
            Spells.Add(prayerOfMendingSpellService);
            Spells.Add(circleOfHealingSpellService);
            Spells.Add(divineStarSpellService);
            Spells.Add(holyWordSerenitySpellService);
            Spells.Add(holyWordSanctifySpellService);
            Spells.Add(divineHymnSpellService);
            Spells.Add(holyWordSalvationSpellService);
        }

        public BaseModelResults GetResults(GameState state)
        {
            var results = new BaseModelResults();

            journal.Entry($"Beginning results run started at {DateTime.Now:yyyy.MM.dd HH:mm:ss:ffff}.");
            var sw = new Stopwatch();
            sw.Start();

            foreach (var spell in Spells)
            {
                var castResults = spell.GetCastResults(state, null);
                results.SpellCastResults.Add(castResults);
            }

            RollUpResults(results, results.SpellCastResults);

            results.TotalRawHPM = results.TotalRawHPS / results.TotalMPS;
            results.TotalActualHPM = results.TotalActualHPS / results.TotalMPS;

            sw.Stop();
            journal.Entry($"Results: RawHPS ({results.TotalRawHPS:0.##}) HPS ({results.TotalActualHPS:0.##}) " +
                $"MPS ({results.TotalMPS:0.##})");
            journal.Entry($"Results: RawHPM ({results.TotalRawHPM:0.##}) HPM ({results.TotalActualHPM:0.##})");
            journal.Entry($"Results run done in {sw.ElapsedMilliseconds}ms.");

            return results;
        }

        private BaseModelResults RollUpResults(BaseModelResults results, List<SpellCastResult> spells)
        {
            foreach(var spellResult in spells)
            {
                if (spellResult is AveragedSpellCastResult averageResult)
                {
                    results.TotalActualHPS += averageResult.HPS;
                    results.TotalRawHPS += averageResult.RawHPS;
                    results.TotalMPS += averageResult.MPS;

                    if(averageResult.AdditionalCasts.Count > 0)
                    {
                        RollUpResults(results, averageResult.AdditionalCasts);
                    }
                }
            }

            return results;
        }
    }
}
