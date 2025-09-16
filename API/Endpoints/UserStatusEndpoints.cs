using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class UserStatusEndpoints
{
    public static void MapUserStatusEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/userstatuses", async (AppDbContext db) =>
            await db.UserStatus.ToListAsync());
        
        app.MapGet("/userstatuses/{id}", async (int id, AppDbContext db) =>
            await db.UserStatus.FindAsync(id) is UserStatus statusUser ? Results.Ok(statusUser) : Results.NotFound("StatusUser not found"));
        
        app.MapPost("userstatuses", async (UserStatus userStatus, AppDbContext db) =>  
        {
            db.UserStatus.Add(userStatus);
            await db.SaveChangesAsync();
            return Results.Created($"/userstatuses/{userStatus.Id}", userStatus);
        });
        
        app.MapPut("/userstatuses/{id}", async (int id, UserStatus inputUserStatus, AppDbContext db) =>
        {
            var statusUser = await db.UserStatus.FindAsync(id);
            if (statusUser is null) return Results.NotFound("StatusUser não encontrado.");

            statusUser.Name = inputUserStatus.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        app.MapDelete("userstatuses/{id}", async (int id, AppDbContext db) =>
        {
            var statusUser = await db.UserStatus.FindAsync(id);
            if (statusUser is null) return Results.NotFound("StatusUser não encontrado.");

            db.UserStatus.Remove(statusUser);
            await db.SaveChangesAsync();
            return Results.Ok(statusUser);
        });
    }
}