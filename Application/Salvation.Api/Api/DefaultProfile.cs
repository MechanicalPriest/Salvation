using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Salvation.Core.Constants.Data;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Profile.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Salvation.Api.Api
{
    public class DefaultProfile
    {
        private readonly IProfileService _profileGenerationService;

        public DefaultProfile(IProfileService profileGenerationService)
        {
            _profileGenerationService = profileGenerationService;
        }

        [FunctionName("DefaultProfile")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var validSpec = int.TryParse(req.Query["specid"], out int specId);

            if (!validSpec)
            {
                // Log only the first 3 characters of the parameter
                log.LogError("Invalid spec provided");
                return new BadRequestResult();
            }

            var response = BuildProfileResponse(specId);

            // Remove the .Profile to return the full response - requires client support
            return new OkObjectResult(new { Data = response.Profile });
        }

        internal class ProfileResponse
        {
            public PlayerProfile Profile;
            public Dictionary<string, int> Covenants;
        }

        private ProfileResponse BuildProfileResponse(int specId)
        {
            ProfileResponse response = new ProfileResponse
            {
                Profile = _profileGenerationService.GetDefaultProfile((Spec)specId),
                Covenants = GetCovenants()
            };

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
