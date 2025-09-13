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
    }
}