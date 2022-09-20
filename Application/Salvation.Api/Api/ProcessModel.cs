using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using Salvation.Core.ViewModel;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Salvation.Api.Api
{
    public class ProcessModel
    {
        private readonly IConstantsService _constantsService;
        private readonly IModellingService _modellingService;
        private readonly IStatWeightGenerationService _statWeightGenerationService;
        private readonly IGameStateService _gameStateService;

        public ProcessModel(IConstantsService constantService,
            IModellingService modellingService,
            IStatWeightGenerationService statWeightGenerationService,
            IGameStateService gameStateService)
        {
            _constantsService = constantService;
            _modellingService = modellingService;
            _statWeightGenerationService = statWeightGenerationService;
            _gameStateService = gameStateService;
        }

        [FunctionName("ProcessModel")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request,
            ILogger log, ExecutionContext context)
        {
            // Parse the incoming profile
            PlayerProfileViewModel profileVM;
            try
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                profileVM = await JsonSerializer.DeserializeAsync<PlayerProfileViewModel>(request.Body, jsonOptions);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Unable to process request body, wrong format?");
                return new BadRequestResult();
            }

            if (profileVM == null)
            {
                log.LogError("Profile needs to be provided");
                return new BadRequestResult();
            }

            log.LogInformation("Processing a new profile: {0}", JsonSerializer.Serialize(profileVM));

            // Load the profile into the model and return the results
            try
            {
                _constantsService.SetDefaultDirectory(context.FunctionAppDirectory);

                //------------------------------
                var profile = profileVM.ToModel();
                GameState state = _gameStateService.CreateValidatedGameState(profile);

                var results = _modellingService.GetResults(state);

                var ehSWState = _gameStateService.CloneGameState(state);

                var effectiveHealingStatWeights = _statWeightGenerationService.Generate(ehSWState, 100,
                    StatWeightGenerator.StatWeightType.EffectiveHealing);

                var rhSWState = _gameStateService.CloneGameState(state);

                var rawHealingStatWeights = _statWeightGenerationService.Generate(rhSWState, 100,
                    StatWeightGenerator.StatWeightType.RawHealing);

                //------------------------------

                var finalResults = new ModellingResultsViewModel()
                {

                    ModelResults = results,
                    //EffectiveStatWeightResult = effectiveHealingStatWeights,
                    //RawStatWeightResult = rawHealingStatWeights,
                    //GameState = state,
                    JournalEntries = _gameStateService.GetJournal(state)

                };

                return new JsonResult(finalResults);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Unable to process model");
                return new BadRequestResult();
            }
        }
    }
}
