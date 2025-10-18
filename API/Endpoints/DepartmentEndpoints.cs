using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class DepartmentEndpoints
{
    public static void MapDeptsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/departments", async (AppDbContext db) =>
            await db.Departments.ToListAsync())
        .WithName("GetDepartments")
        .Produces(200)
        .Produces(401);
        
        app.MapGet("/departments/{id}", async (int id, AppDbContext db) =>
        {
            var dept = await db.Departments.FindAsync(id);
            return dept is not null ? Results.Ok(dept) : Results.NotFound();
        })
        .WithName("GetDepartmentById")
        .Produces(200)
        .Produces(404)
        .Produces(401);

        app.MapPost("/departments", async (Department department, AppDbContext db) =>
        {
            // Create a new Department and copy allowed fields only; ignore any Id provided by client
            var entity = new Department { Name = department.Name, AcceptTicket = department.AcceptTicket };
            db.Departments.Add(entity);
            await db.SaveChangesAsync();
            return Results.Created($"/departments/{entity.Id}", entity);
        })
        .WithName("CreateDepartment")
        .Produces(201)
        .Produces(400)
        .Produces(401);
        
        app.MapPut("/departments/{id}", async (int id, Department inputDepartment, AppDbContext db) =>
        {
            var dept = await db.Departments.FindAsync(id);
            if (dept is null) return Results.NotFound();

            dept.Name = inputDepartment.Name;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("UpdateDepartment")
        .Produces(204)
        .Produces(404)
        .Produces(401);
        
        app.MapDelete("/departments/{id}", async (int id, AppDbContext db) =>
        {
            var dept = await db.Departments.FindAsync(id);
            if (dept is null) return Results.NotFound();

            db.Departments.Remove(dept);
            await db.SaveChangesAsync();
            return Results.Ok(dept);
        })
        .WithName("DeleteDepartment")
        .Produces(200)
        .Produces(404)
        .Produces(401);
        
    }

}