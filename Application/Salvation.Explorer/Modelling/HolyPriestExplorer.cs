using Newtonsoft.Json;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Models;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Salvation.Explorer.Modelling
{
    public interface IHolyPriestExplorer
    {
        public void TestHolyPriestModel();
    }

    class HolyPriestExplorer : IHolyPriestExplorer
    {
        private readonly IConstantsService constantsService;

        public HolyPriestExplorer(IConstantsService constantsService)
        {
            this.constantsService = constantsService;
        }

        public void TestHolyPriestModel()
        {
            var globalConstants = constantsService.LoadConstantsFromFile();

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

            GenerateStatWeights(constantsService);
        }

        public void GenerateStatWeights(IConstantsService constantsManager)
        {
            var basicProfile = DefaultProfiles.GetDefaultProfile(Spec.HolyPriest);

            StatWeightGenerator sw = new StatWeightGenerator(constantsManager);
            var results = sw.Generate(basicProfile, 100,
                StatWeightGenerator.StatWeightType.EffectiveHealing);

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
        }
    }
}
