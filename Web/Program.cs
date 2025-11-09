using Web.Components;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri("http://localhost:5185");
});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// In Development, avoid HTTPS redirection to prevent mixed-content and certificate issues
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

// Proxy uploads from API so the browser loads them from the same origin as the Web app
app.MapGet("/uploads/{*filePath}", async (string filePath, IHttpClientFactory factory) =>
{
    var client = factory.CreateClient("API");
    var resp = await client.GetAsync($"uploads/{filePath}");
    if (!resp.IsSuccessStatusCode) return Results.NotFound();
    var contentType = resp.Content.Headers.ContentType?.ToString() ?? MediaTypeNames.Application.Octet;
    var bytes = await resp.Content.ReadAsByteArrayAsync();
    return Results.File(bytes, contentType);
});

app.MapGet("/files/{*filePath}", async (string filePath, IHttpClientFactory factory) =>
{
    var client = factory.CreateClient("API");
    var resp = await client.GetAsync($"uploads/{filePath}");
    if (!resp.IsSuccessStatusCode) return Results.NotFound();
    var contentType = resp.Content.Headers.ContentType?.ToString() ?? MediaTypeNames.Application.Octet;
    var bytes = await resp.Content.ReadAsByteArrayAsync();
    return Results.File(bytes, contentType);
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
