using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class UserProfileEndpoints
{
    public static void MapUserProfileEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles", async (AppDbContext db) =>
            await db.UserProfiles.ToListAsync())
        .WithName("GetProfiles")
        .Produces(200)
        .Produces(400)
        .Produces(401);

        app.MapGet("/profiles/{id}", async (int id, AppDbContext db) =>
        {
            var userProfile = await db.UserProfiles.FindAsync(id);
            return userProfile is not null ? Results.Ok(userProfile) : Results.NotFound("Profile not found");
        })
        .WithName("GetProfileById")
        .Produces(200)
        .Produces(404)
        .Produces(401);

        app.MapPost("/profiles", async (UserProfile userProfile, AppDbContext db) =>
        {
            // Validate name and check duplicates (normalized)
            var normalized = userProfile.Name?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(normalized))
            {
                return Results.BadRequest("Nome do perfil é obrigatório.");
            }
            var exists = await db.UserProfiles.AnyAsync(p => p.Name != null && p.Name.ToLower() == normalized);
            if (exists)
            {
                return Results.Conflict(new { Message = $"Perfil {userProfile.Name} cadastrado." });
            }
            // Create a new UserProfile and copy allowed fields; ignore any Id provided by client
            var entity = new UserProfile { Name = userProfile.Name };
            db.UserProfiles.Add(entity);
            await db.SaveChangesAsync();
            return Results.Created($"/profiles/{entity.Id}", entity);
        })
        .WithName("CreateProfile")
        .Produces(201)
        .Produces(400)
        .Produces(401);
        
        app.MapPut("/profiles/{id}", async (int id, UserProfile inputUserProfile, AppDbContext db) =>
        {
            var userProfile = await db.UserProfiles.FindAsync(id);
            if (userProfile is null) return Results.NotFound("Perfil não encontrado.");

            userProfile.Name = inputUserProfile.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("UpdateProfile")
        .Produces(204)
        .Produces(404)
        .Produces(401);
        
        app.MapDelete("/profiles/{id}", async (int id, AppDbContext db) =>
        {
            var userProfile = await db.UserProfiles.FindAsync(id);
            if (userProfile is null) return Results.NotFound("Perfil não encontrado.");

            db.UserProfiles.Remove(userProfile);
            await db.SaveChangesAsync();
            return Results.Ok(userProfile);
        })
        .WithName("DeleteProfile")
        .Produces(200)
        .Produces(404)
        .Produces(401);
    }
}