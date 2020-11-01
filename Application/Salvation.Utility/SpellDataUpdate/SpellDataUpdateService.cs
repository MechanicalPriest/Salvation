using Newtonsoft.Json;
using Salvation.Core.Constants;
using SimcProfileParser.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace Salvation.Utility.SpellDataUpdate
{
    public class SpellDataUpdateService : ISpellDataUpdateService
    {
        private readonly ISpellDataService<HolyPriestSpellDataService> _hpriestSpellData;
        private readonly ISimcGenerationService _simcGenerationService;

        public SpellDataUpdateService(ISpellDataService<HolyPriestSpellDataService> hpriestSpellData,
            ISimcGenerationService simcGenerationService)
        {
            _hpriestSpellData = hpriestSpellData;
            _simcGenerationService = simcGenerationService;
        }

        public async Task UpdateSpellData()
        {
            // Load the constants file into an object
            var globalConstants = new GlobalConstants();

            // Create and add each spec
            var hpriestSpellData = await _hpriestSpellData.Generate();
            globalConstants.Specs.Add(hpriestSpellData);

            // TODO: Update the game version string
            globalConstants.GameVersion = await _simcGenerationService.GetGameDataVersionAsync();

            // Save the constants to file
            File.WriteAllText(@"..\..\..\..\Salvation.Core\constants.json",
                JsonConvert.SerializeObject(globalConstants, Formatting.Indented));
        }
    }
}
