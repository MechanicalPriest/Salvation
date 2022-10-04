using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Salvation.Core.Interfaces.Profile;
using Salvation.Core.Profile;
using Salvation.Core.ViewModel;
using System.IO;
using System.Threading.Tasks;
using Spec = Salvation.Core.Constants.Data.Spec;

namespace Salvation.Api.Api
{
    public class DefaultProfile
    {
        private readonly IProfileService _profileGenerationService;
        private readonly ISimcProfileService _simcProfileService;

        public DefaultProfile(IProfileService profileGenerationService, ISimcProfileService simcProfileService)
        {
            _profileGenerationService = profileGenerationService;
            _simcProfileService = simcProfileService;
        }

        [FunctionName("DefaultProfile")]
        public async Task<IActionResult> Run(
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
            var spec = (Spec)specId;
            log.LogTrace("Pulling profile for {spec}", spec);

            var profile = _profileGenerationService.GetDefaultProfile(spec);

            log.LogTrace("Loading default simc profile import");

            var profileData = File.ReadAllText(Path.Combine("Profile", "HolyPriest", "dragonflight_fresh.simc"));

            log.LogTrace("Updating profile with simc profile import");

            profile = await _simcProfileService.ApplySimcProfileAsync(profileData, profile);

            log.LogTrace("Converting profile to viewmodel");

            var profileVM = profile.ToViewModel();

            log.LogTrace("Done building default profile");

            return new OkObjectResult(profileVM);
        }
    }




}
