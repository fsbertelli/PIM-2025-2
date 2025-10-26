using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/categories", async (AppDbContext db) =>
            await db.Categories.Include(c => c.Department).ToListAsync())
        .WithName("GetCategories")
        .Produces(200)
        .Produces(401);

        app.MapGet("/categories/{id}", async (int id, AppDbContext db) =>
        {
            var item = await db.Categories.Include(c => c.Department).FirstOrDefaultAsync(c => c.Id == id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        })
        .WithName("GetCategoryById")
        .Produces(200)
        .Produces(404)
        .Produces(401);

        app.MapPost("/categories", async (Category category, AppDbContext db) =>
        {
            var dept = await db.Departments.FindAsync(category.DeptId);
            if (dept is null) return Results.BadRequest("Departamento inválido.");

            var entity = new Category { Description = category.Description, DeptId = category.DeptId, Department = dept };
            db.Categories.Add(entity);
            await db.SaveChangesAsync();
            return Results.Created($"/categories/{entity.Id}", entity);
        })
        .WithName("CreateCategory")
        .Produces(201)
        .Produces(400)
        .Produces(401);

        app.MapPut("/categories/{id}", async (int id, Category input, AppDbContext db) =>
        {
            var item = await db.Categories.FindAsync(id);
            if (item is null) return Results.NotFound();

            var dept = await db.Departments.FindAsync(input.DeptId);
            if (dept is null) return Results.BadRequest("Departamento inválido.");

            item.Description = input.Description;
            item.DeptId = input.DeptId;
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("UpdateCategory")
        .Produces(204)
        .Produces(404)
        .Produces(401);

        app.MapDelete("/categories/{id}", async (int id, AppDbContext db) =>
        {
            var item = await db.Categories.FindAsync(id);
            if (item is null) return Results.NotFound();
            db.Categories.Remove(item);
            await db.SaveChangesAsync();
            return Results.Ok(item);
        })
        .WithName("DeleteCategory")
        .Produces(200)
        .Produces(404)
        .Produces(401);
    }
}

