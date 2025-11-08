using API.Data;
using API.Endpoints;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketTransactionService, TicketTransactionService>();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
    
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();

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

app.MapUserEndpoints();
app.MapUserDtoEndpoints();
app.MapUserStatusEndpoints();
app.MapUserProfileEndpoints();
app.MapDeptsEndpoints();
app.MapStatusTicketEndpoints();
app.MapCategoryEndpoints();
app.MapTicketEndpoints();
app.MapTicketTransactionEndpoints();

app.Run();
