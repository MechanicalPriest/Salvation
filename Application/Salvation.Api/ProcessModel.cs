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
using Salvation.Core.Models;

namespace Salvation.Api
{
    public static class ProcessModel
    {
        [FunctionName("ProcessModel")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request,
            ILogger log, ExecutionContext context)
        {

            BaseProfile profile;
            try
            {
                string requestBody = await new StreamReader(request.Body).ReadToEndAsync();
                profile = JsonConvert.DeserializeObject<BaseProfile>(requestBody);
            }
            catch (Exception ex)
            {
                log.LogError("Unable to process request body, wrong format?", ex);
                return new BadRequestResult();
            }

            if (profile == null)
            {
                log.LogError("Profile needs tobe provided");
                return new BadRequestResult();
            }

            log.LogInformation("Processing a new profile: {0}", JsonConvert.SerializeObject(profile));

            string filePath = Path.Combine(context.FunctionAppDirectory, @"constants.json");
            string data;

            try
            {
                data = File.ReadAllText(filePath);
            }
            catch(Exception ex)
            {
                log.LogError(ex, $"Unable to load constants file: {filePath}");
                return new BadRequestResult();
            }

            var constants = ConstantsManager.ParseConstants(data);

            var model = ModelManager.LoadModel(profile, constants);

            var results = model.GetResults();

            var sw = new StatWeightGenerator();

            var effectiveHealingStatWeights = sw.Generate(results.Profile, 100,
                StatWeightGenerator.StatWeightType.EffectiveHealing);

            var rawHealingStatWeights = sw.Generate(results.Profile, 100,
                StatWeightGenerator.StatWeightType.RawHealing);

            return new JsonResult(new { 
                ModelResults = results,
                StatWeightsEffective = effectiveHealingStatWeights,
                StatWeightsRaw = rawHealingStatWeights
            });
        }
    }
}
