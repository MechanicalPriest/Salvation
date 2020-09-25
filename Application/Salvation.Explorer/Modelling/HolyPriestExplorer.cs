using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Salvation.Core.Constants;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Modelling;
using Salvation.Core.Modelling.Common;
using Salvation.Core.Modelling.HolyPriest;
using Salvation.Core.Profile;
using Salvation.Core.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Salvation.Explorer.Modelling
{
    public interface IHolyPriestExplorer
    {
        public void TestNewHolyPriestModel();
        public void TestHolyPriestModel();
        public void CompareCovenants();
    }

    class HolyPriestExplorer : IHolyPriestExplorer
    {
        private readonly IConstantsService constantsService;
        private readonly IModellingService modellingService;
        private readonly IProfileGenerationService holyPriestProfileGeneratior;
        private readonly IComparisonModeller<CovenantComparisons> comparisonModellerCovenant;

        public HolyPriestExplorer(IConstantsService constantsService, 
            IModellingService modellingService,
            IProfileGenerationService holyPriestProfileGeneratior,
            IComparisonModeller<CovenantComparisons> comparisonModellerCovenant)
        {
            this.constantsService = constantsService;
            this.modellingService = modellingService;
            this.holyPriestProfileGeneratior = holyPriestProfileGeneratior;
            this.comparisonModellerCovenant = comparisonModellerCovenant;
        }

        public void TestHolyPriestModel()
        {
            var globalConstants = constantsService.LoadConstantsFromFile();

            GenerateStatWeights(constantsService);
        }

        public void GenerateStatWeights(IConstantsService constantsManager)
        {
            var profileGen = new ProfileGenerationService();
            var basicProfile = profileGen.GetDefaultProfile(Spec.HolyPriest);

            StatWeightGenerator sw = new StatWeightGenerator(constantsManager);
            var results = sw.Generate(basicProfile, 100,
                StatWeightGenerator.StatWeightType.EffectiveHealing);

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
        }

        public void CompareCovenants()
        {
            var results = comparisonModellerCovenant.RunComparison() as Dictionary<string, BaseModelResults>;

            StringBuilder sb = new StringBuilder();

            var baselineResults = results.Where(a => a.Key == "Baseline").FirstOrDefault().Value;

            foreach (var result in results)
            {
                sb.AppendLine($"{result.Key}, {result.Value.TotalRawHPS - baselineResults.TotalRawHPS:0.##}");
            }

            File.WriteAllText("covenant_results.csv", sb.ToString());
        }

        public void TestNewHolyPriestModel()
        {
            GameState state = new GameState();
            state.Constants = constantsService.LoadConstantsFromFile();
            state.Profile = holyPriestProfileGeneratior.GetDefaultProfile(Spec.HolyPriest);

            var results = modellingService.GetResults(state);
        }
    }
}
