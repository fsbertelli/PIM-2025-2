using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        // Rota para obter todos os usuários
        app.MapGet("/users", async (AppDbContext db) => 
            await db.Users.ToListAsync());

        // Rota para obter um usuário por ID
        app.MapGet("/users/{id}", async (int id, AppDbContext db) => 
            await db.Users.FindAsync(id) is Users cliente ? Results.Ok(cliente) : Results.NotFound("Cliente não encontrado."));

        // Rota para criar um novo usuário
        app.MapPost("/users", async (Users users, AppDbContext db) =>
        {
            db.Users.Add(users);
            await db.SaveChangesAsync();
            return Results.Created($"/users/{users.Id}", users);
        });

        // Rota para atualizar um usuário existente
        app.MapPut("/users/{id}", async (int id, Users inputUsers, AppDbContext db) =>
        {
            var cliente = await db.Users.FindAsync(id);

            if (cliente is null) return Results.NotFound("Usuário não encontrado.");

            cliente.Name = inputUsers.Name;


            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        // Rota para deletar um usuário
        app.MapDelete("/users/{id}", async (int id, AppDbContext db) =>
        {
            var cliente = await db.Users.FindAsync(id);
            if (cliente is null) return Results.NotFound("Usuário não encontrado.");

            db.Users.Remove(cliente);
            await db.SaveChangesAsync();

            return Results.Ok(cliente);
        });
    }
}
