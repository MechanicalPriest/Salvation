using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.Interfaces.State;
using Salvation.Core.Modelling;
using Salvation.Core.Profile.Model;
using Salvation.Core.State;
using System;
using System.IO;
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
            PlayerProfile profile;
            try
            {
                string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
                profile = JsonConvert.DeserializeObject<PlayerProfile>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Unable to process request body, wrong format?");
                return new BadRequestResult();
            }

            if (profile == null)
            {
                log.LogError("Profile needs to be provided");
                return new BadRequestResult();
            }

            log.LogInformation("Processing a new profile: {0}", JsonConvert.SerializeObject(profile));

            // Load the profile into the model and return the results
            try
            {
                _constantsService.SetDefaultDirectory(context.FunctionAppDirectory);

                //------------------------------
                GameState state = _gameStateService.CreateValidatedGameState(profile);

                var results = _modellingService.GetResults(state);

                var effectiveHealingStatWeights = _statWeightGenerationService.Generate(state, 100,
                    StatWeightGenerator.StatWeightType.EffectiveHealing);

                var rawHealingStatWeights = _statWeightGenerationService.Generate(state, 100,
                    StatWeightGenerator.StatWeightType.RawHealing);

                //------------------------------

                return new JsonResult(new
                {
                    Data = new
                    {
                        ModelResults = results,
                        StatWeightsEffective = effectiveHealingStatWeights,
                        StatWeightsRaw = rawHealingStatWeights,
                        State = state,
                        Journal = _gameStateService.GetJournal(state)
                    }
                });
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Unable to process model");
                return new BadRequestResult();
            }
        }
    }
}
