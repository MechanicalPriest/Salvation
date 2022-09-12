using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.ViewModel;
using Spec = Salvation.Core.Constants.Data.Spec;

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

            var profile = _profileGenerationService.GetDefaultProfile((Spec)specId);

            var profileVM = profile.ToViewModel();

            return new OkObjectResult(profileVM);
        }
    }




}
