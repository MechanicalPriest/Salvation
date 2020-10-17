using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimcProfileParser.Model.Generated;
using SimcProfileParser.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using SimcProfileParser.Model.RawData;

namespace Salvation.Api
{
    public class GetSpell
    {
        private readonly ISimcGenerationService _simcGenerationService;

        public GetSpell(ISimcGenerationService simcGenerationService)
        {
            _simcGenerationService = simcGenerationService;
        }

        [FunctionName("GetSpell")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "spell/{id:int?}")]
            HttpRequest req, int? id, ILogger log)
        {
            var spellOptions = new SimcSpellOptions();

            if (id.HasValue)
                spellOptions.SpellId = Convert.ToUInt32(id.Value);
            else
                return new BadRequestErrorMessageResult("ID must be a valid SpellId");
                
            if(req.Query.ContainsKey("itemLevel"))
            {
                if(!req.Query.ContainsKey("itemQuality"))
                    return new BadRequestErrorMessageResult("itemQuality must be specified");

                if (!req.Query.ContainsKey("itemInventoryType"))
                    return new BadRequestErrorMessageResult("itemInventoryType must be specified");

                if(!int.TryParse(req.Query["itemLevel"], out int itemLevel))
                    return new BadRequestErrorMessageResult("itemLevel provided was not recognised");

                if (!Enum.TryParse(req.Query["itemQuality"], out ItemQuality itemQuality))
                    return new BadRequestErrorMessageResult("itemQuality provided was not recognised");

                if (!Enum.TryParse(req.Query["itemInventoryType"], out InventoryType itemInventoryType))
                    return new BadRequestErrorMessageResult("itemInventoryType provided was not recognised");

                spellOptions.ItemLevel = itemLevel;
                spellOptions.ItemQuality = itemQuality;
                spellOptions.ItemInventoryType = itemInventoryType;
            }
            else if (req.Query.ContainsKey("itemInventoryType"))
            {
                if (!uint.TryParse(req.Query["playerLevel"], out uint playerLevel))
                    return new BadRequestErrorMessageResult("itemLevel provided was not recognised");

                if (!Enum.TryParse(req.Query["itemInventoryType"], out InventoryType itemInventoryType))
                    return new BadRequestErrorMessageResult("itemInventoryType provided was not recognised");

                spellOptions.PlayerLevel = playerLevel;
                spellOptions.ItemInventoryType = itemInventoryType;
            }
            else
            {
                return new BadRequestErrorMessageResult("itemLevel or playerLevel must be supplied.");
            }

            try
            {
                var spell = await _simcGenerationService.GenerateSpellAsync(spellOptions);
                return new OkObjectResult(spell);
            }
            catch(Exception ex)
            {
                log.LogError(ex, ex.Message);
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
