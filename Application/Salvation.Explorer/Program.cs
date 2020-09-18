using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Salvation.Core;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Models;
using Salvation.Core.Models.Common;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Salvation.Explorer
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            TestHolyPriestModel();

            Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IConstantsService, ConstantsService>();
                    //services.AddHostedService<StripeHostedService>();
                });
    

        private static void TestHolyPriestModel()
        {
            var constantsManager = new ConstantsService();
            var globalConstants = constantsManager.LoadConstantsFromFile();

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

            GenerateStatWeights(constantsManager);
        }

        private static void GenerateStatWeights(ConstantsService constantsManager)
        {
            var basicProfile = DefaultProfiles.GetDefaultProfile(Spec.HolyPriest);

            var sw = new StatWeightGenerator(constantsManager);
            var results = sw.Generate(basicProfile, 100, 
                StatWeightGenerator.StatWeightType.EffectiveHealing);

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
        }
    }
}
