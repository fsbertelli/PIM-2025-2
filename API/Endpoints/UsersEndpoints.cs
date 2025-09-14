using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        // Rota para obter todos os clientes
        app.MapGet("/clientes", async (AppDbContext db) => 
            await db.Users.ToListAsync());

        // Rota para obter um cliente por ID
        app.MapGet("/clientes/{id}", async (int id, AppDbContext db) => 
            await db.Users.FindAsync(id) is Users cliente ? Results.Ok(cliente) : Results.NotFound("Cliente não encontrado."));

        // Rota para criar um novo cliente
        app.MapPost("/clientes", async (Users users, AppDbContext db) =>
        {
            db.Users.Add(users);
            await db.SaveChangesAsync();
            return Results.Created($"/clientes/{users.Id}", users);
        });

        // Rota para atualizar um cliente existente
        app.MapPut("/clientes/{id}", async (int id, Users inputUsers, AppDbContext db) =>
        {
            var cliente = await db.Users.FindAsync(id);

            if (cliente is null) return Results.NotFound("Cliente não encontrado.");

            cliente.Name = inputUsers.Name;


            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        // Rota para deletar um cliente
        app.MapDelete("/clientes/{id}", async (int id, AppDbContext db) =>
        {
            var cliente = await db.Users.FindAsync(id);
            if (cliente is null) return Results.NotFound("Cliente não encontrado.");

            db.Users.Remove(cliente);
            await db.SaveChangesAsync();

            return Results.Ok(cliente);
        });
    }
}
