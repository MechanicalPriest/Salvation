using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Salvation.Core.ViewModel;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace Salvation.Client.Shared.Components
{
    public partial class TalentViewer : ComponentBase
    {
        private Spec? holyPriest;
        private static int ClassPoints = 31;
        private static int SpecPoints = 30;

        protected override async Task OnInitializedAsync()
        {
            holyPriest = await Http.GetFromJsonAsync<Spec>("static-data/talent-tree.json");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("updateWowheadTooltips");
        }
    }
}
