using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Salvation.Core.Profile.Model;
using Salvation.Core.ViewModel;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace Salvation.Client.Shared.Components
{
    public partial class ModelProcessor : ComponentBase
    {
        [Inject]
        protected IHttpClientFactory? _httpClientFactory { get; set; }
        [Inject]
        protected IConfiguration? _configuration { get; set; }

        private static int holyPriestSpecId = 257;
        private static string defaultProfileEndpoint = "DefaultProfile";
        private static string wowheadItemLinkPrefix = "//wowhead.com/item=";
        private static string wowheadItemSpellPrefix = "//wowhead.com/spell=";

        private PlayerProfileViewModel? data;
        private string errorMessage = string.Empty;
        private bool loadingData = true;

        private string searchString = "";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await GetDefaultProfile();
        }

        private async Task GetDefaultProfile()
        {
            loadingData = true;
            errorMessage = "";

            var rootUri = _configuration["ModelProcessorSettings:RootUri"];

            var request = new HttpRequestMessage(HttpMethod.Get,
                rootUri + $"{defaultProfileEndpoint}?specid={holyPriestSpecId}");
            request.Headers.Add("Accept", "application/json");

            var client = _httpClientFactory.CreateClient();

            try
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();

                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };

                    data = await JsonSerializer.DeserializeAsync<PlayerProfileViewModel>(responseStream, jsonOptions);

                    loadingData = false;
                }
                else
                {
                    errorMessage = "Unable to generate default profile.";
                    loadingData = false;
                }
            }
            catch (HttpRequestException ex)
            {
                errorMessage = "Unable to generate default profile.";
                loadingData = false;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("updateWowheadTooltips");
        }

        public string GenerateWowheadItemLink(Item item)
        {
            // Link is in the format item={id}?bonus={bonus}:{bonus}&ilvl={ilvl}&spec={specid}
            var link = wowheadItemLinkPrefix;

            link += $"{item.ItemId}";

            return link;
        }

        public string GenerateWowheadItemLink(CastProfileViewModel cast)
        {
            // Link is in the format item={id}?bonus={bonus}:{bonus}&ilvl={ilvl}&spec={specid}
            var link = wowheadItemSpellPrefix;

            link += $"{cast.SpellId}";

            return link;
        }

        /// <summary>
        /// Filters the playstyle list based on the search text
        /// </summary>
        private Func<CastProfileViewModel, bool> playstyleFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;

            if (x.SpellId.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };
    }
}
