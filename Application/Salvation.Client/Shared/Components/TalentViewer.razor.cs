using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Salvation.Core.ViewModel;
using System.Net.Http;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace Salvation.Client.Shared.Components
{
    public partial class TalentViewer : ComponentBase
    {
        [Inject]
        protected IHttpClientFactory? _httpClientFactory { get; set; }

        [Parameter]
        public Dictionary<int, int> SelectedTalents { get; set; } = new Dictionary<int, int>();

        private TalentSpec? holyPriest;
        private static int ClassPoints = 31;
        private static int SpecPoints = 30;

        protected override async Task OnInitializedAsync()
        {
            if (_httpClientFactory == null)
                throw new NullReferenceException("Web client was not initialised");

            var client = _httpClientFactory.CreateClient("StaticData");
            holyPriest = await client.GetFromJsonAsync<TalentSpec>("talent-tree.json");

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("updateWowheadTooltips");
        }
    }
}
