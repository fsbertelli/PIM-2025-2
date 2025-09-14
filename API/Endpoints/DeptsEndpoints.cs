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

        app.MapPost("/depts", async (Dept dept, AppDbContext db) =>
        {
            db.Depts.Add(dept);
            await db.SaveChangesAsync();
            return Results.Created($"/depts/{dept.Id}", dept);
        });
        
        app.MapPut("/depts/{id}", async (int id, Dept inputDept, AppDbContext db) =>
        {
            var dept = await db.Depts.FindAsync(id);
            if (dept is null) return Results.NotFound();

            dept.Name = inputDept.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });
        
        app.MapDelete("/depts/{id}", async (int id, AppDbContext db) =>
        {
            var dept = await db.Depts.FindAsync(id);
            if (dept is null) return Results.NotFound();

            db.Depts.Remove(dept);
            await db.SaveChangesAsync();
            return Results.Ok(dept);
        });
        
    }

}