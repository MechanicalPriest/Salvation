using BlazorApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.JSInterop;
using Salvation.Core.Constants;
using Salvation.Core.Profile.Model;
using Salvation.Core.ViewModel;
using System.Text.Json;

namespace Salvation.Client.Shared.Components
{
    public partial class ModelProcessor : ComponentBase
    {
        [Inject]
        protected IHttpClientFactory? _httpClientFactory { get; set; }
        [Inject]
        protected IConfiguration? _configuration { get; set; }

        [Inject] 
        protected IApplicationInsights _appInsights { get; set; }

        private static int holyPriestSpecId = 257;
        private static string defaultProfileEndpoint = "DefaultProfile";
        private static string wowheadItemLinkPrefix = "//wowhead.com/item=";
        private static string wowheadSpellPrefix = "//wowhead.com/spell=";

        // Loading of default profile
        private PlayerProfileViewModel? data;
        private string errorMessage = string.Empty;
        private bool loadingData = true;

        private string searchString = "";
        private string advancedSearchString = "";

        // Loading of results
        private ModellingResults modellingResults = new ModellingResults();

        protected override async Task OnInitializedAsync()
        {
            await GetDefaultProfile();
        }

        private async Task GetDefaultProfile()
        {
            loadingData = true;
            errorMessage = "";

            var client = _httpClientFactory.CreateClient("Api");

            try
            {
                var response = await client.GetAsync($"{defaultProfileEndpoint}?specid={holyPriestSpecId}");

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
                Error error = new()
                {
                    Message = ex.Message,
                    Stack = ex.StackTrace
                };
                await _appInsights.TrackException(error);

                errorMessage = "Unable to generate default profile.";
                loadingData = false;
            }
            catch(InvalidOperationException ex)
            {
                Error error = new()
                {
                    Message = ex.Message,
                    Stack = ex.StackTrace
                };
                await _appInsights.TrackException(error);

                errorMessage = $"Unable to generate default profile.";
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

        public string GenerateWowheadSpellLink(int spellId)
        {
            var link = wowheadSpellPrefix;

            link += $"{spellId}";

            return link;
        }

        public string GenerateWowheadSpellLink(uint spellId)
        {
            return GenerateWowheadSpellLink((int)spellId);
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

        /// <summary>
        /// Filters the advanced list based on the search text
        /// </summary>
        private Func<AdvancedSettingsViewModel, bool> advancedFilter => x =>
        {
            if (string.IsNullOrWhiteSpace(advancedSearchString))
                return true;

            if (x.SpellId.ToString().Contains(advancedSearchString, StringComparison.OrdinalIgnoreCase))
                return true;

            if (x.Name.Contains(advancedSearchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };
    }
}
