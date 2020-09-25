using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Salvation.Core.Profile;
using Salvation.Core;
using Salvation.Core.Constants;
using System.Runtime.CompilerServices;
using Salvation.Core.Modelling;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Core.State;
using Salvation.Core.Interfaces;

namespace Salvation.Api
{
    public class ProcessModel
    {
        private readonly IConstantsService constantsService;
        private readonly IModellingService modellingService;
        private readonly IModellingJournal journal;
        private readonly IStatWeightGenerationService statWeightGenerationService;

        public ProcessModel(IConstantsService constantService, 
            IModellingService modellingService,
            IModellingJournal journal,
            IStatWeightGenerationService statWeightGenerationService)
        {
            this.constantsService = constantService;
            this.modellingService = modellingService;
            this.journal = journal;
            this.statWeightGenerationService = statWeightGenerationService;
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
                log.LogError("Unable to process request body, wrong format?", ex);
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
                constantsService.SetDefaultDirectory(context.FunctionAppDirectory);

                //------------------------------
                GameState state = new GameState();
                state.Constants = constantsService.LoadConstantsFromFile();
                state.Profile = profile;

                var results = modellingService.GetResults(state);

                var effectiveHealingStatWeights = statWeightGenerationService.Generate(state, 100,
                    StatWeightGenerator.StatWeightType.EffectiveHealing);

                var rawHealingStatWeights = statWeightGenerationService.Generate(state, 100,
                    StatWeightGenerator.StatWeightType.RawHealing);

                //------------------------------

                return new JsonResult(new
                {
                    ModelResults = results,
                    StatWeightsEffective = effectiveHealingStatWeights,
                    StatWeightsRaw = rawHealingStatWeights,
                    State = state,
                    Journal = journal.GetJournal(true)
                });
            }
            catch(Exception ex)
            {
                log.LogError(ex, $"Unable to process model");
                return new BadRequestResult();
            }
        }
    }
}
