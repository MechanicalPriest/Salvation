﻿using Newtonsoft.Json;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Interfaces.State;
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
        private readonly IModellingService _modellingService;
        private readonly IProfileService _profileService;
        private readonly IComparisonModeller<CovenantComparisonsResult> _comparisonModellerCovenant;
        private readonly IStatWeightGenerationService _statWeightGenerationService;
        private readonly IGameStateService _gameStateService;

        public HolyPriestExplorer(IModellingService modellingService,
            IProfileService profileService,
            IComparisonModeller<CovenantComparisonsResult> comparisonModellerCovenant,
            IStatWeightGenerationService statWeightGenerationService,
            IGameStateService gameStateService)
        {
            _modellingService = modellingService;
            _profileService = profileService;
            _comparisonModellerCovenant = comparisonModellerCovenant;
            _statWeightGenerationService = statWeightGenerationService;
            _gameStateService = gameStateService;
        }

        public void GenerateStatWeights()
        {
            var state = _gameStateService.CreateValidatedGameState(
                _profileService.GetDefaultProfile(Spec.HolyPriest));

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
            GameState state = _gameStateService.CreateValidatedGameState(
                _profileService.GetDefaultProfile(Spec.HolyPriest));

            var results = _modellingService.GetResults(state);
            File.WriteAllText("hpriest_model_results.json",
                JsonConvert.SerializeObject(results, Formatting.Indented));
        }
    }
}
