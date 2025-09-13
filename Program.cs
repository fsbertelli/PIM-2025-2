using ClientesAPI.Data;
using ClientesAPI.Endpoints;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/info", () => new {
    Solucao = "CadClientes",
    Projeto = "ClientesAPI",
    VersaoDotNet = "9.0.304",
    Autor = "Alunos",
    Data = DateTime.Now.ToString("yyyy-MM-dd")
});

// Mapeia os endpoints de Clientes
app.MapClienteEndpoints();

app.Run();
