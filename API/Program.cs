using API.Data;
using API.Endpoints;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
    
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

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

app.Run();
