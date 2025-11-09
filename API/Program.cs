using API.Data;
using API.Endpoints;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using API.Hubs;
using API.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketTransactionService, TicketTransactionService>();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("gpt", c =>
{
    var apiKey = "g4a-wcFI3axvYYes67tLpMw2lYE3AfcvL74nTZJ"; 
    var baseUrl = "https://api.gpt4-all.xyz/v1/chat/completions";

    c.BaseAddress = new Uri(baseUrl);
    c.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});
builder.Services.AddScoped<IGptService, GptService>();

builder.Services.AddSignalR();
builder.Services.AddSingleton<ILoggerProvider>(sp => new SignalRLoggerProvider(sp));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.MapHub<LogHub>("/loghub");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/info", () => new {
    Solucao = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name,
    Projeto = "PIM 2025.2",
    VersaoDotNet = System.Environment.Version.ToString(),
    Autor = "Felipe Bertelli",
    Data = DateTime.Now.ToString("dd-MM-yyyy")
});

app.MapGet("/sniffer", () => Results.Redirect("/sniffer.html"));
app.MapGet("/logtest", (ILogger<Program> logger) =>
{
    logger.LogInformation("Sniffer test log enviado em {Now}", DateTime.UtcNow);
    return Results.Ok("log enviado");
});

app.MapUserEndpoints();
app.MapUserDtoEndpoints();
app.MapUserStatusEndpoints();
app.MapUserProfileEndpoints();
app.MapDeptsEndpoints();
app.MapStatusTicketEndpoints();
app.MapCategoryEndpoints();
app.MapTicketEndpoints();
app.MapTicketTransactionEndpoints();

var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
startupLogger.LogInformation("Aplicação inicializada em {Now}", DateTime.Now);

app.Run();
