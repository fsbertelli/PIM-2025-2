using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class DeptsEndpoints
{
    public static void MapDeptsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/depts", async (AppDbContext db) =>
            await db.Depts.ToListAsync());
        
        app.MapGet("/depts/{id}", async (int id, AppDbContext db) =>
            await db.Depts.FindAsync(id) is Dept dept ? Results.Ok(dept) : Results.NotFound());
        
        
    }

}