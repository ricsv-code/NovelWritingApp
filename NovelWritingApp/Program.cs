using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NovelWritingApp;
using NovelWritingApp.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure the base address for the HttpClient to use your API URL
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7292/"),
    DefaultRequestHeaders =
    {
        Accept = { new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json") }
    }
});

// Register services
builder.Services.AddScoped<NovelService>();
builder.Services.AddScoped<ChapterService>();
builder.Services.AddScoped<CharacterService>();

// Configure JSON serializer options
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.ReferenceHandler = ReferenceHandler.Preserve;
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

await builder.Build().RunAsync();
