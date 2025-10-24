using Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register HttpClient for calling the backend API (adjust base address if your API runs on a different port)
builder.Services.AddHttpClient("API", client =>
{
    // API launchSettings shows HTTPS on port 7084 and HTTP on 5185.
    // Use the HTTPS address by default so TLS works when both projects run with HTTPS.
    client.BaseAddress = new Uri("http://172.22.64.1:5185");
});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
