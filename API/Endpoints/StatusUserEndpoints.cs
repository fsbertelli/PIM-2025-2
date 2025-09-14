using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class StatusUserEndpoints
{
    public static void MapStatusUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/statususer", async (AppDbContext db) =>
            await db.StatusUsers.ToListAsync());
        
        app.MapGet("/statususer/{id}", async (int id, AppDbContext db) =>
            await db.StatusUsers.FindAsync(id) is StatusUser statusUser ? Results.Ok(statusUser) : Results.NotFound("StatusUser not found"));
        
        app.MapPost("statususer", async (StatusUser statusUser, AppDbContext db) =>  
        {
            db.StatusUsers.Add(statusUser);
            await db.SaveChangesAsync();
            return Results.Created($"/statususer/{statusUser.Id}", statusUser);
        });
        
        app.MapPut("/statususer/{id}", async (int id, StatusUser inputStatusUser, AppDbContext db) =>
        {
            var statusUser = await db.StatusUsers.FindAsync(id);
            if (statusUser is null) return Results.NotFound("StatusUser não encontrado.");

            statusUser.Status = inputStatusUser.Status;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        app.MapDelete("statususers/{id}", async (int id, AppDbContext db) =>
        {
            var statusUser = await db.StatusUsers.FindAsync(id);
            if (statusUser is null) return Results.NotFound("StatusUser não encontrado.");

            db.StatusUsers.Remove(statusUser);
            await db.SaveChangesAsync();
            return Results.Ok(statusUser);
        });
    }
}