using ClientesAPI.Data;
using ClientesAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientesAPI.Endpoints;

public static class StatusUserEndpoints
{
    public static void MapStatusUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/statususer", async (AppDbContext db) =>
            await db.StatusUsers.ToListAsync());
        
        app.MapGet("/statususer/{id}", async (int id, AppDbContext db) =>
            await db.StatusUsers.FindAsync(id) is StatusUser statusUser ? Results.Ok(statusUser) : Results.NotFound("StatusUser not found"));
    }
    
}