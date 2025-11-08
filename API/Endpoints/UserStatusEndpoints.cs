using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class UserStatusEndpoints
{
    public static void MapUserStatusEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/userstatuses", async (AppDbContext db) =>
            await db.UserStatus.ToListAsync())
        .WithName("GetUserStatuses")
        .Produces(200)
        .Produces(400)
        .Produces(401);
        
        app.MapGet("/userstatuses/{id}", async (int id, AppDbContext db) =>
        {
            var statusUser = await db.UserStatus.FindAsync(id);
            return statusUser is not null ? Results.Ok(statusUser) : Results.NotFound("StatusUser not found");
        })
        .WithName("GetUserStatusById")
        .Produces(200)
        .Produces(404)
        .Produces(401);
        
        app.MapPost("/userstatuses", async (UserStatus userStatus, AppDbContext db) =>  
        {
            // Validate name and check duplicates (normalized)
            var normalized = userStatus.Name?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(normalized))
            {
                return Results.BadRequest("Nome do status é obrigatório.");
            }
            var exists = await db.UserStatus.AnyAsync(s => s.Name != null && s.Name.ToLower() == normalized);
            if (exists)
            {
                return Results.Conflict(new { Message = "Status já cadastrado." });
            }
            // Create a new entity and copy allowed fields only; ignore any Id provided by client
            var entity = new UserStatus { Name = userStatus.Name };
            db.UserStatus.Add(entity);
            await db.SaveChangesAsync();
            return Results.Created($"/userstatuses/{entity.Id}", entity);
        })
        .WithName("CreateUserStatus")
        .Produces(201)
        .Produces(400)
        .Produces(401);
        
        app.MapPut("/userstatuses/{id}", async (int id, UserStatus inputUserStatus, AppDbContext db) =>
        {
            var statusUser = await db.UserStatus.FindAsync(id);
            if (statusUser is null) return Results.NotFound("StatusUser não encontrado.");

            statusUser.Name = inputUserStatus.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("UpdateUserStatus")
        .Produces(204)
        .Produces(404)
        .Produces(401);

        app.MapDelete("/userstatuses/{id}", async (int id, AppDbContext db) =>
        {
            var statusUser = await db.UserStatus.FindAsync(id);
            if (statusUser is null) return Results.NotFound("StatusUser não encontrado.");

            db.UserStatus.Remove(statusUser);
            try
            {
                await db.SaveChangesAsync();
                return Results.Ok(statusUser);
            }
            catch (DbUpdateException ex)
            {
                var error = ex.GetBaseException() as  Microsoft.Data.SqlClient.SqlException;
                if (error != null && (error.Number == 547 || error.Number == 1451))
                {
                    return Results.Conflict(new { Message = "Não é possível excluir o status do usuário porque ele está associado a outros registros." });
                }
                return Results.Problem(detail: ex.GetBaseException()?.Message ?? ex.Message, statusCode: 500);
            }
        })
        .WithName("DeleteUserStatus")
        .Produces(200)
        .Produces(404)
        .Produces(401)
        .Produces(409)
        .Produces(500);
    }
}