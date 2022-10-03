using BlazorApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Salvation.Core.Profile.Model;
using Salvation.Core.ViewModel;
using System.Net.Http.Json;

namespace Salvation.Client.Shared.Components
{
    public partial class ModelProcessor : ComponentBase
    {
        [Inject]
        protected IHttpClientFactory? _httpClientFactory { get; set; }
        [Inject]
        protected IConfiguration? _configuration { get; set; }

        [Inject] 
        protected IApplicationInsights? _appInsights { get; set; }

        private static readonly int holyPriestSpecId = 257;
        private static readonly string wowheadItemLinkPrefix = "//wowhead.com/item=";
        private static readonly string wowheadSpellPrefix = "//wowhead.com/spell=";

        // Loading of default profile
        private static readonly string defaultProfileEndpoint = "DefaultProfile";
        private PlayerProfileViewModel? data = default;
        private string errorMessage = string.Empty;
        private bool loadingData = true;

        private string searchString = string.Empty;
        private string advancedSearchString = string.Empty;

        // Talent viewer
        private Dictionary<int, int> selectedTalents = new Dictionary<int, int>();

        // Loading of results
        private static readonly string processModelEndpoint = "ProcessModel";
        private ModellingResultsViewModel? modellingResults = null;
        private bool loadingResults = false;

        // Charts
        private ChartDataItem[] HpmChartData { get; set; } = Array.Empty<ChartDataItem>();
        private ChartDataItem[] HealCpmChartData { get; set; } = Array.Empty<ChartDataItem>();
        private ChartDataItem[] HealMpsChartData { get; set; } = Array.Empty<ChartDataItem>();

        // Applying simc import string
        private static readonly string simcImportStringEndpoint = "ApplySimcProfile";
        private string simcImportString = string.Empty;

        // Talent Bar display
        private TalentSpec? TalentData;
        private List<TalentBarItem> ClassTalents { get; set; } = new List<TalentBarItem>();
        private List<TalentBarItem> SpecTalents { get; set; } = new List<TalentBarItem>();
        private static string UnknownIconName = "inv_misc_questionmark.jpg";

        protected override async Task OnInitializedAsync()
        {
            await GetDefaultProfile();
        }

        private async Task GenerateResults()
        {
            loadingResults = true;

            if (_httpClientFactory == null)
                throw new NullReferenceException("Web client was not initialised");

            if (_appInsights == null)
                throw new NullReferenceException("App insights logging was not initialised");

            if(data == null)
                throw new NullReferenceException("There is no profile to generate results from");

            var client = _httpClientFactory.CreateClient("Api");

            try
            {
                // Populate the selected talents.
                foreach(var talent in selectedTalents)
                {
                    var dataTalent = data.Talents.Where(t => t.SpellId == talent.Key).FirstOrDefault();

                    if (dataTalent != null)
                        dataTalent.Rank = talent.Value;
                }

                var response = await client.PostAsJsonAsync(processModelEndpoint, data);

                if (response.IsSuccessStatusCode)
                {
                    modellingResults = await response.Content.ReadFromJsonAsync<ModellingResultsViewModel>();

                    loadingResults = false;
                }
                else
                {
                    loadingResults = false;
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

                loadingResults = false;
            }
            catch (InvalidOperationException ex)
            {
                Error error = new()
                {
                    Message = ex.Message,
                    Stack = ex.StackTrace
                };
                await _appInsights.TrackException(error);

                loadingResults = false;
            }

            DataPostProcessing();
        }

        private async Task ApplySimcImportString()
        {
            if (_httpClientFactory == null)
                throw new NullReferenceException("Web client was not initialised");

            if (_appInsights == null)
                throw new NullReferenceException("App insights logging was not initialised");

            var client = _httpClientFactory.CreateClient("Api");

            try
            {
                var dataToSend = new ApplySimcProfileRequest()
                {
                    Profile = data,
                    SimcProfileString = simcImportString
                };

                var response = await client.PostAsJsonAsync(simcImportStringEndpoint, dataToSend);

                if (response.IsSuccessStatusCode)
                {
                    data = await response.Content.ReadFromJsonAsync<PlayerProfileViewModel>();
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
            }
            catch (InvalidOperationException ex)
            {
                Error error = new()
                {
                    Message = ex.Message,
                    Stack = ex.StackTrace
                };
                await _appInsights.TrackException(error);
            }
        }

        private async Task GetDefaultProfile()
        {
            loadingData = true;
            errorMessage = "";

            if (_httpClientFactory == null)
                throw new NullReferenceException("Web client was not initialised");

            if (_appInsights == null)
                throw new NullReferenceException("App insights logging was not initialised");

            var client = _httpClientFactory.CreateClient("Api");

            try
            {
                var response = await client.GetAsync($"{defaultProfileEndpoint}?specid={holyPriestSpecId}");

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();

                    var jsonOptions = new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };

                    data = await System.Text.Json.JsonSerializer.DeserializeAsync<PlayerProfileViewModel>(responseStream, jsonOptions);

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

        private void DataPostProcessing()
        {
            if (data == null)
                throw new NullReferenceException("There are no results to post-process");

            HealCpmChartData = Array.Empty<ChartDataItem>();
            HealMpsChartData = Array.Empty<ChartDataItem>();
            HpmChartData = Array.Empty<ChartDataItem>();

            if (modellingResults == null)
                return;

            // Generate CPM chart data
            HealCpmChartData = modellingResults.ModelResults.SpellCastResults
                .Where(s => s.CastsPerMinute > 0 && s.RawHealing > 0)
                .Select(s => new ChartDataItem() { Name = s.SpellName, Value = s.CastsPerMinute })
                .ToArray();

            // Generate MPS chart data
            HealMpsChartData = modellingResults.ModelResults.SpellCastResults
                .Where(s => s.ManaCost > 0 && s.CastsPerMinute > 0 && s.RawHealing > 0)
                .Select(s => new ChartDataItem() { Name = s.SpellName, Value = s.MPS })
                .ToArray();

            // Generate HPM chart data
            HpmChartData = modellingResults.ModelResults.SpellCastResults
                .Where(s => s.ManaCost > 0 && s.CastsPerMinute > 0 && s.RawHealing > 0)
                .OrderByDescending(s => s.HPM)
                .Select(s => new ChartDataItem() { Name = s.SpellName, Value = s.HPM })
                .ToArray();

            // Populate the list of class talents
            // Pulling this all together without components is very ugly but it'll do for now.
            if (TalentData == null)
                throw new NullReferenceException("There are no list of talents to post-process with");

            ClassTalents = new List<TalentBarItem>();
            foreach(var talent in data.Talents)
            {
                if(talent.Rank > 0)
                {
                    // Look for it in the talents list
                    var talentInfo = TalentData.ClassNodes
                        .Where(t => t.TalentEntries.Where(te => te.SpellId == talent.SpellId).Any())
                        .Select(s => s.TalentEntries.Where(te => te.SpellId == talent.SpellId).FirstOrDefault())
                        .FirstOrDefault();

                    if (talentInfo == null)
                        continue;

                    ClassTalents.Add(new TalentBarItem()
                    {
                        SpellId = talent.SpellId,
                        IconName = talentInfo.Icon,
                        Rank = talent.Rank,
                    });
                }
            }

            SpecTalents = new List<TalentBarItem>();
            foreach (var talent in data.Talents)
            {
                if (talent.Rank > 0)
                {
                    // Look for it in the talents list
                    var talentInfo = TalentData.SpecNodes
                        .Where(t => t.TalentEntries.Where(te => te.SpellId == talent.SpellId).Any())
                        .Select(s => s.TalentEntries.Where(te => te.SpellId == talent.SpellId).FirstOrDefault())
                        .FirstOrDefault();

                    if (talentInfo == null)
                        continue;

                    SpecTalents.Add(new TalentBarItem()
                    {
                        SpellId = talent.SpellId,
                        IconName = talentInfo.Icon,
                        Rank = talent.Rank,
                    });
                }
            }
        }

        private void OnTalentDefinitionChanged(TalentSpec talentSpec)
        {
            TalentData = talentSpec;
        }


        public string GetImageStyle(string talentIcon)
        {
            return $"background-image: url('static-data/icons/{talentIcon}.jpg'), url('static-data/icons/{UnknownIconName}');";
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

        class ChartDataItem
        {
            public string Name { get; set; } = string.Empty;
            public double Value { get; set; }
        }

        class TalentBarItem
        {
            public int SpellId { get; set; } = 0;
            public string IconName { get; set; } = string.Empty;
            public int Rank { get; set; } = 0;
        }
    }
}
