using SimcProfileParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
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
            // Create and add each spec
            var hpriestSpellData = await _hpriestSpellData.Generate();
            // Update the game version string
            // Save the constants to file
        }
    }
}
