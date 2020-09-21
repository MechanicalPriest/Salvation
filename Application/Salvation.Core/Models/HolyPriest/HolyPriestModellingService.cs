using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Models;
using Salvation.Core.Interfaces.Models.HolyPriest.Spells;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Salvation.Core.Models.HolyPriest
{
    class HolyPriestModellingService : IModellingService
    {
        private readonly IConstantsService constantsService;
        public List<ISpellService> Spells { get; private set; }

        public HolyPriestModellingService(IConstantsService constantsService,
            IFlashHealSpellService fhService)
        {
            this.constantsService = constantsService;

            Spells.Add(fhService);
        }

        public BaseModelResults GetResults(BaseProfile profile)
        {
            var results = new BaseModelResults();
            var constants = constantsService.LoadConstantsFromFile();

            foreach(var spell in Spells)
            {
                var castResults = spell.GetCastResults(null, null);
                results.SpellCastResults.Add(castResults);
            }

            return results;
        }
    }
}
