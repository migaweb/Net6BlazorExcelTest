using BlazorApp.Client;
using BlazorApp.Services.Services;
using BlazorApp.GeneralUI;
using BlazorApp.GeneralUI.Components.BusyOverlay;
using BlazorApp.WASM.Shared.Contracts;
using BlazorApp.WASM.Shared.Entities;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IPersistenceService, IndexedDB>();
builder.Services.AddScoped<IExcelReader<Article>, ArticleExcelReader>();
builder.Services.AddScoped<PrimeService>();
builder.Services.AddScoped<BusyOverlayService>();

await builder.Build().RunAsync();
