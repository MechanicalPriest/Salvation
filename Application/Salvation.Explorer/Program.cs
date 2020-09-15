using Newtonsoft.Json;
using Salvation.Core;
using Salvation.Core.Constants;
using Salvation.Core.Models;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
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

            var basicProfile = DefaultProfiles.GetDefaultProfile(Spec.HolyPriest);
            DefaultProfiles.SetToVenthyr(basicProfile);

            var hpriest = new HolyPriestModel(globalConstants, basicProfile);

            hpriest.GetResults();

            Console.WriteLine("------------[ Profile ]------------");
            Console.WriteLine(JsonConvert.SerializeObject(basicProfile, Formatting.Indented));

            Console.WriteLine("------------[ Results ]------------");
            var modelResults = hpriest.GetResults();
            var spellsRaw = JsonConvert.SerializeObject(modelResults, Formatting.Indented);

            Console.WriteLine(spellsRaw);

            GenerateStatWeights(globalConstants);
        }

        private static void GenerateStatWeights(GlobalConstants globalConstants)
        {
            List<BaseProfile> profiles = new List<BaseProfile>();

            var basicProfile = DefaultProfiles.GetDefaultProfile(Spec.HolyPriest);
            profiles.Add(basicProfile);

            var intProfile = DefaultProfiles.GetDefaultProfile(Spec.HolyPriest);
            intProfile.Intellect += 10;
            intProfile.Name = "Intellect Profile";
            profiles.Add(intProfile);

            var hasteProfile = DefaultProfiles.GetDefaultProfile(Spec.HolyPriest);
            hasteProfile.HasteRating += 10;
            hasteProfile.Name = "Haste Profile";
            profiles.Add(hasteProfile);

            var critProfile = DefaultProfiles.GetDefaultProfile(Spec.HolyPriest);
            critProfile.CritRating += 10;
            critProfile.Name = "Crit Profile";
            profiles.Add(critProfile);

            var versProfile = DefaultProfiles.GetDefaultProfile(Spec.HolyPriest);
            versProfile.VersatilityRating += 10;
            versProfile.Name = "Vers Profile";
            profiles.Add(versProfile);

            var masteryProfile = DefaultProfiles.GetDefaultProfile(Spec.HolyPriest);
            masteryProfile.MasteryRating += 10;
            masteryProfile.Name = "Mastery Profile";
            profiles.Add(masteryProfile);

            Console.WriteLine($"Name,RawHps,Hps,RawHpm,RawHps");

            foreach (var profile in profiles)
            {
                if (profile.SpecId == Spec.HolyPriest)
                {
                    var hpriest = new HolyPriestModel(globalConstants, profile);

                    var result = hpriest.GetResults();

                    Console.WriteLine($"{result.Profile.Name},{result.TotalRawHPS}," +
                        $"{result.TotalActualHPS},{result.TotalRawHPM},{result.TotalRawHPS}");
                }
            }
        }
    }
}
