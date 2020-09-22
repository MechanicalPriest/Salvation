using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
using Salvation.Core.Models.Common;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    public class HolyPriestModellingService : IModellingService
    {
        private readonly IConstantsService constantsService;
        public List<ISpellService> Spells { get; private set; }

        public HolyPriestModellingService(IConstantsService constantsService,
            IFlashHealSpellService flashHealService,
            IHolyWordSerenitySpellService serenitySpellService,
            IHolyWordSalvationSpellService holyWordSalvationSpellService,
            IRenewSpellService renewSpellService,
            IPrayerOfMendingSpellService prayerOfMendingSpellService,
            IPrayerOfHealingSpellService prayerOfHealingSpellService)
        {
            this.constantsService = constantsService;

            Spells = new List<ISpellService>();
            Spells.Add(flashHealService);
            Spells.Add(serenitySpellService);
            Spells.Add(holyWordSalvationSpellService);
            Spells.Add(renewSpellService);
            Spells.Add(prayerOfMendingSpellService);
            Spells.Add(prayerOfHealingSpellService);
        }

        public BaseModelResults GetResults(GameState state)
        {
            var results = new BaseModelResults();

            foreach(var spell in Spells)
            {
                var castResults = spell.GetCastResults(state, null);
                results.SpellCastResults.Add(castResults);
            }

            RollUpResults(results, results.SpellCastResults);

            results.TotalRawHPM = results.TotalRawHPS / results.TotalMPS;
            results.TotalActualHPM = results.TotalActualHPS / results.TotalMPS;

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
