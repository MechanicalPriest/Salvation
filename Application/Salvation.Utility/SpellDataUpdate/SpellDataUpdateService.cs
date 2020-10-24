using Newtonsoft.Json;
using Salvation.Core.Constants;
using System.IO;
using System.Threading.Tasks;

namespace Salvation.Utility.SpellDataUpdate
{
    public class SpellDataUpdateService : ISpellDataUpdateService
    {
        private readonly ISpellDataService<HolyPriestSpellDataService> _hpriestSpellData;

        public SpellDataUpdateService(ISpellDataService<HolyPriestSpellDataService> hpriestSpellData)
        {
            _hpriestSpellData = hpriestSpellData;
        }

        public async Task UpdateSpellData()
        {
            // Load the constants file into an object
            var globalConstants = new GlobalConstants();

            // Create and add each spec
            var hpriestSpellData = await _hpriestSpellData.Generate();
            globalConstants.Specs.Add(hpriestSpellData);

            // TODO: Update the game version string
            globalConstants.GameVersion = "9.0.2.36294";

            // Save the constants to file
            File.WriteAllText(@"..\..\..\..\Salvation.Core\constants.json",
                JsonConvert.SerializeObject(globalConstants, Formatting.Indented));
        }
    }
}
