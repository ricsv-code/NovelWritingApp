using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NovelWritingApp;
using NovelWritingApp.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7292/"),
    DefaultRequestHeaders =
    {
        Accept = { new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json") }
    }
});
//Services
builder.Services.AddScoped<NovelService>();
builder.Services.AddScoped<ChapterService>();
builder.Services.AddScoped<CharacterService>();

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.ReferenceHandler = ReferenceHandler.Preserve;
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

await builder.Build().RunAsync();
