using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
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
            IHolyWordSerenitySpellService serenitySpellService)
        {
            this.constantsService = constantsService;

            Spells = new List<ISpellService>();
            Spells.Add(flashHealService);
            Spells.Add(serenitySpellService);
        }

        public BaseModelResults GetResults(GameState state)
        {
            var results = new BaseModelResults();

            foreach(var spell in Spells)
            {
                var castResults = spell.GetCastResults(state, null);
                results.SpellCastResults.Add(castResults);
            }

            return results;
        }
    }
}
