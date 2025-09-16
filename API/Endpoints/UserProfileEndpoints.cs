using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class UserProfileEndpoints
{
    public static void MapUserProfileEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles", async (AppDbContext db) =>
            await db.UserProfiles.ToListAsync());

        app.MapGet("/profiles/{id}", async (int id, AppDbContext db) =>
            await db.UserProfiles.FindAsync(id) is UserProfile userProfile ? Results.Ok(userProfile) : Results.NotFound("Profile not found"));

        app.MapPost("/profiles", async (UserProfile userProfile, AppDbContext db) =>
        {
            db.UserProfiles.Add(userProfile);
            await db.SaveChangesAsync();
            return Results.Created($"/profiles/{userProfile.Id}", userProfile);
        });
        
        app.MapPut("/profiles/{id}", async (int id, UserProfile inputUserProfile, AppDbContext db) =>
        {
            var userProfile = await db.UserProfiles.FindAsync(id);
            if (userProfile is null) return Results.NotFound("Perfil não encontrado.");

            userProfile.Name = inputUserProfile.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });
        
        app.MapDelete("/profiles/{id}", async (int id, AppDbContext db) =>
        {
            var userProfile = await db.UserProfiles.FindAsync(id);
            if (userProfile is null) return Results.NotFound("Perfil não encontrado.");

            db.UserProfiles.Remove(userProfile);
            await db.SaveChangesAsync();
            return Results.Ok(userProfile);
        });
    }
}