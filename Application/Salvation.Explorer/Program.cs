using Newtonsoft.Json;
using Salvation.Core;
using Salvation.Core.Models;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.IO;

namespace Salvation.Explorer
{
    class Program
    {
        static void Main(string[] args)
        {
            TestHolyPriestModel();

            Console.ReadLine();
        }

        private static void TestHolyPriestModel()
        {
            var data = File.ReadAllText(@"constants.json");

            var globalConstants = ConstantsManager.ParseConstants(data);

            var basicProfile = new BaseProfile()
            {
                Intellect = 1001,
                MasteryRating = 242,
                VersatilityRating = 139,
                HasteRating = 242,
            };

            var hpriest = new HolyPriestModel(globalConstants, basicProfile);

            Console.WriteLine("------------[ Profile ]------------");
            Console.WriteLine(JsonConvert.SerializeObject(basicProfile, Formatting.Indented));

            Console.WriteLine("------------[ Results ]------------");
            var spellsRaw = JsonConvert.SerializeObject(hpriest.Spells, Formatting.Indented);

            Console.WriteLine(spellsRaw);
        }
    }
}
