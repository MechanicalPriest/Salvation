using Newtonsoft.Json;
using Salvation.Core;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Salvation.CoreTests
{
    public class HolyPriestModelTests
    {
        [Fact]
        public void HolyPriestModelGivesResults()
        {
            var data = File.ReadAllText(@"constants.json");

            var globalConstants = ConstantsManager.ParseConstants(data);

            var basicProfile = DefaultProfiles.GetDefaultProfile(Core.Models.Spec.HolyPriest);

            var hpriest = new HolyPriestModel(globalConstants, basicProfile);

            var spellsRaw = JsonConvert.SerializeObject(hpriest.Spells, Formatting.Indented);

            Console.WriteLine(spellsRaw);
        }
    }
}
