using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Salvation.Api.Api.Model;
using Salvation.Core.Interfaces.Profile;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Salvation.Api.Api
{
    public class ApplySimcProfile
    {
        private readonly ISimcProfileService _simcProfileService;

        public ApplySimcProfile(ISimcProfileService simcProfileService)
        {
            _simcProfileService = simcProfileService;
        }

        [FunctionName("ApplySimcProfile")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] ApplySimcProfileRequest req,
            ILogger log)
        {
            if (req == null)
                return new BadRequestErrorMessageResult("Unable to process incoming request body.");

            if(String.IsNullOrEmpty(req.SimcProfileString))
                return new BadRequestErrorMessageResult("Unable to process incoming request body.");

            try
            {
                var newProfile = await _simcProfileService.ApplySimcProfileAsync(req.SimcProfileString, req.Profile);
                return new JsonResult(new { Data = newProfile });
            }
            catch (Exception ex)
            {
                log.LogError("Unable to process request body, wrong format?", ex);
                return new BadRequestErrorMessageResult("Unable to process request body, wrong format?");
            }
        }
    }
}
