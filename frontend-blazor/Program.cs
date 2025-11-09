using Frontend;
using Frontend.Services.SensorServiceApi;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Mount root components
builder.RootComponents.Add<App>("#app");

// Backend base address (fallback to localhost)
var backendBase = builder.Configuration["BackendBase"] ?? "";
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(backendBase) });
builder.Services.AddScoped<ISensorApiClient, SensorApiClient>();

await builder.Build().RunAsync();
