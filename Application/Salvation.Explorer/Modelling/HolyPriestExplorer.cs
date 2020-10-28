using Newtonsoft.Json;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Modelling;
using Salvation.Core.State;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Salvation.Explorer.Modelling
{
    public interface IHolyPriestExplorer
    {
        public void GenerateStatWeights();
        public void TestHolyPriestModel();
        public void CompareCovenants();
    }

    class HolyPriestExplorer : IHolyPriestExplorer
    {
        private readonly IConstantsService _constantsService;
        private readonly IModellingService _modellingService;
        private readonly IProfileGenerationService _holyPriestProfileGeneratior;
        private readonly IComparisonModeller<CovenantComparisonsResult> _comparisonModellerCovenant;
        private readonly IStatWeightGenerationService _statWeightGenerationService;
        private readonly IProfileGenerationService _profileGenerationService;

        public HolyPriestExplorer(IConstantsService constantsService,
            IModellingService modellingService,
            IProfileGenerationService holyPriestProfileGeneratior,
            IComparisonModeller<CovenantComparisonsResult> comparisonModellerCovenant,
            IStatWeightGenerationService statWeightGenerationService,
            IProfileGenerationService profileGenerationService)
        {
            _constantsService = constantsService;
            _modellingService = modellingService;
            _holyPriestProfileGeneratior = holyPriestProfileGeneratior;
            _comparisonModellerCovenant = comparisonModellerCovenant;
            _statWeightGenerationService = statWeightGenerationService;
            _profileGenerationService = profileGenerationService;
        }

        public void GenerateStatWeights()
        {
            var state = new GameState(_profileGenerationService.GetDefaultProfile(Spec.HolyPriest),
                _constantsService.LoadConstantsFromFile());

            var results = _statWeightGenerationService.Generate(state, 100,
                StatWeightGenerator.StatWeightType.EffectiveHealing);

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
        }

        public void CompareCovenants()
        {
            var results = _comparisonModellerCovenant.RunComparison().Results;

            StringBuilder sb = new StringBuilder();

            var baselineResults = results.Where(a => a.Key == "Baseline").FirstOrDefault().Value;

            foreach (var result in results)
            {
                sb.AppendLine($"{result.Key}, {result.Value.TotalRawHPS - baselineResults.TotalRawHPS:0.##}");
            }

            File.WriteAllText("covenant_results.csv", sb.ToString());
        }

        public void TestHolyPriestModel()
        {
            GameState state = new GameState
            {
                Constants = _constantsService.LoadConstantsFromFile(),
                Profile = _holyPriestProfileGeneratior.GetDefaultProfile(Spec.HolyPriest)
            };

            var results = _modellingService.GetResults(state);
            File.WriteAllText("hpriest_model_results.json", 
                JsonConvert.SerializeObject(results, Formatting.Indented));
        }
    }
}
