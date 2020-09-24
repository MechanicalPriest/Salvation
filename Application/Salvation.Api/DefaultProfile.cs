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
using System.Linq;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Constants.Data;

namespace Salvation.Api
{
    public class DefaultProfile
    {
        private readonly IProfileGenerationService profileGenerationService;

        public DefaultProfile(IProfileGenerationService profileGenerationService)
        {
            this.profileGenerationService = profileGenerationService;
        }

        [FunctionName("DefaultProfile")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            int specId;
            var validSpec = int.TryParse(req.Query["specid"], out specId);

            if(!validSpec)
            {
                // Log only the first 3 characters of the parameter
                log.LogError("Invalid spec provided");
                return new BadRequestResult();
            }

            var response = BuildProfileResponse(specId);

            // Remove the .Profile to return the full response - requires client support
            return new OkObjectResult(response.Profile);
        }

        internal class ProfileResponse
        {
            public PlayerProfile Profile;
            public Dictionary<string, int> Covenants;
        }

        private ProfileResponse BuildProfileResponse(int specId)
        {
            ProfileResponse response = new ProfileResponse();

            response.Profile = profileGenerationService.GetDefaultProfile((Spec)specId);
            response.Covenants = GetCovenants();

            return response;
        }

        private Dictionary<string, int> GetCovenants()
        {
            var covenantList = Enum.GetValues(typeof(Covenant)).Cast<Covenant>();

            var values = new Dictionary<string, int>();

            foreach (var covenant in covenantList)
            {
                values.Add(covenant.GetDescription(), (int)covenant);
            }

            return values;
        }
    }



    
}
