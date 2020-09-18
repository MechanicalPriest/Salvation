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
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace Salvation.Api
{
    public static class DefaultProfile
    {
        [FunctionName("DefaultProfile")]
        public static async Task<IActionResult> Run(
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
            public BaseProfile Profile;
            public Dictionary<string, int> Covenants;
        }

        private static ProfileResponse BuildProfileResponse(int specId)
        {
            ProfileResponse response = new ProfileResponse();

            response.Profile = DefaultProfiles.GetDefaultProfile(specId);
            response.Covenants = GetCovenants();

            return response;
        }

        private static Dictionary<string, int> GetCovenants()
        {
            var covenantList = Enum.GetValues(typeof(Covenant)).Cast<Covenant>();

            var values = new Dictionary<string, int>();

            foreach (var covenant in covenantList)
            {
                values.Add(covenant.GetDescription(), (int)covenant);
            }

            return values;
        }

        public static string GetDescription<T>(this T value)
        where T : Enum
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                             typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }



    
}
