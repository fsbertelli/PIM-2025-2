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
            // Validate name and check duplicates (normalized)
            var normalized = department.Name?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(normalized))
            {
                return Results.BadRequest("Nome do departamento é obrigatório.");
            }
            var exists = await db.Departments.AnyAsync(d => d.Name != null && d.Name.ToLower() == normalized);
            if (exists)
            {
                return Results.Conflict(new { Message = "Departamento já cadastrado." });
            }
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
            try
            {
                await db.SaveChangesAsync();
                return Results.Ok(dept);
            }
            catch (DbUpdateException ex)
            {
                var error = ex.GetBaseException() as  Microsoft.Data.SqlClient.SqlException;
                if (error != null && (error.Number == 547 || error.Number == 1451))
                {
                    return Results.Conflict(new { Message = "Não é possível excluir o departamento porque ele está associado a outros registros." });
                }
                return Results.Problem(detail: ex.GetBaseException()?.Message ?? ex.Message, statusCode: 500);
            }

        })
        .WithName("DeleteDepartment")
        .Produces(200)
        .Produces(404)
        .Produces(401)
        .Produces(409)
        .Produces(500);
        
    }

}