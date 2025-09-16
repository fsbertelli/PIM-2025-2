using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class DepartmentEndpoints
{
    public static void MapDeptsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/departments", async (AppDbContext db) =>
            await db.Departments.ToListAsync());
        
        app.MapGet("/departments/{id}", async (int id, AppDbContext db) =>
            await db.Departments.FindAsync(id) is Department dept ? Results.Ok(dept) : Results.NotFound());

        app.MapPost("/departments", async (Department department, AppDbContext db) =>
        {
            db.Departments.Add(department);
            await db.SaveChangesAsync();
            return Results.Created($"/departments/{department.Id}", department);
        });
        
        app.MapPut("/departments/{id}", async (int id, Department inputDepartment, AppDbContext db) =>
        {
            var dept = await db.Departments.FindAsync(id);
            if (dept is null) return Results.NotFound();

            dept.Name = inputDepartment.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });
        
        app.MapDelete("/departments/{id}", async (int id, AppDbContext db) =>
        {
            var dept = await db.Departments.FindAsync(id);
            if (dept is null) return Results.NotFound();

            db.Departments.Remove(dept);
            await db.SaveChangesAsync();
            return Results.Ok(dept);
        });
        
    }

}