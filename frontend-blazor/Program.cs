using Frontend;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Mount root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Backend base address (fallback to localhost)
var backendBase = builder.Configuration["BackendBase"] ?? "https://localhost:5001/";

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(backendBase) });

await builder.Build().RunAsync();
