using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class ProfileEndpoints
{
    public static void MapProfileEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/profiles", async (AppDbContext db) =>
            await db.Profiles.ToListAsync());

        app.MapGet("/profiles/{id}", async (int id, AppDbContext db) =>
            await db.Profiles.FindAsync(id) is Profile profile ? Results.Ok(profile) : Results.NotFound("Profile not found"));

        app.MapPost("/profiles", async (Profile profile, AppDbContext db) =>
        {
            db.Profiles.Add(profile);
            await db.SaveChangesAsync();
            return Results.Created($"/profiles/{profile.Id}", profile);
        });
        
        app.MapPut("/profiles/{id}", async (int id, Profile inputProfile, AppDbContext db) =>
        {
            var profile = await db.Profiles.FindAsync(id);
            if (profile is null) return Results.NotFound("Perfil não encontrado.");

            profile.Perfil = inputProfile.Perfil;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });
        
        app.MapDelete("/profiles/{id}", async (int id, AppDbContext db) =>
        {
            var profile = await db.Profiles.FindAsync(id);
            if (profile is null) return Results.NotFound("Perfil não encontrado.");

            db.Profiles.Remove(profile);
            await db.SaveChangesAsync();
            return Results.Ok(profile);
        });
        
    }
}