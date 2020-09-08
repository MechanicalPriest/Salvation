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
using System.Collections.Generic;

namespace Salvation.Api
{
    public static class DefaultProfile
    {
        [FunctionName("DefaultProfile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogTrace("Default profile requested.");

            int specId;
            var validSpec = int.TryParse(req.Query["specid"], out specId);

            if(!validSpec)
            {
                // Log only the first 3 characters of the parameter
                log.LogError("Invalid spec provided");
                return new BadRequestResult();
            }

            var basicProfile = DefaultProfiles.GetDefaultProfile(specId);

            return new OkObjectResult(basicProfile);
        }
    }
}
