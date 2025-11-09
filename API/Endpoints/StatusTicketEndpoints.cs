using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class StatusTicketEndpoints
{
    public static void MapStatusTicketEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/statustickets", async (AppDbContext db) =>
            await db.StatusTickets.ToListAsync())
        .WithName("GetStatusTickets")
        .Produces(200)
        .Produces(401);

        app.MapGet("/statustickets/{id}", async (int id, AppDbContext db) =>
        {
            var item = await db.StatusTickets.FindAsync(id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        })
        .WithName("GetStatusTicketById")
        .Produces(200)
        .Produces(404)
        .Produces(401);

        app.MapPost("/statustickets", async (StatusTicket status, AppDbContext db) =>
        {
            var entity = new StatusTicket { Name = status.Name };
            db.StatusTickets.Add(entity);
            await db.SaveChangesAsync();
            return Results.Created($"/statustickets/{entity.Id}", entity);
        })
        .WithName("CreateStatusTicket")
        .Produces(201)
        .Produces(400)
        .Produces(401);

        app.MapPut("/statustickets/{id}", async (int id, StatusTicket input, AppDbContext db) =>
        {
            var item = await db.StatusTickets.FindAsync(id);
            if (item is null) return Results.NotFound();
            item.Name = input.Name;
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("UpdateStatusTicket")
        .Produces(204)
        .Produces(404)
        .Produces(401);

        app.MapDelete("/statustickets/{id}", async (int id, AppDbContext db) =>
        {
            var item = await db.StatusTickets.FindAsync(id);
            if (item is null) return Results.NotFound();
            db.StatusTickets.Remove(item);
            await db.SaveChangesAsync();
            return Results.Ok(item);
        })
        .WithName("DeleteStatusTicket")
        .Produces(200)
        .Produces(404)
        .Produces(401);
    }
}

