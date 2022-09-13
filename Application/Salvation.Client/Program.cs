using BlazorApplicationInsights;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Salvation.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddHttpClient("Api", httpClient =>
{
    httpClient.BaseAddress = new Uri((builder.Configuration["ModelProcessorSettings:ApiRootUri"] ?? builder.HostEnvironment.BaseAddress) + builder.Configuration["ModelProcessorSettings:ApiPath"]);
    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddBlazorApplicationInsights(async applicationInsights =>
{
    var telemetryItem = new TelemetryItem()
    {
        Tags = new Dictionary<string, object>()
            {
                { "ai.cloud.role", "Salvation" },
                { "ai.cloud.roleInstance", "Salvation Client" },
            }
    };

    await applicationInsights.AddTelemetryInitializer(telemetryItem);
});

await builder.Build().RunAsync();
