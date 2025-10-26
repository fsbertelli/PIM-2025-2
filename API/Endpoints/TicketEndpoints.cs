using API.Data;
using API.Models;
using API.Models.DTO;
using API.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace API.Endpoints;

public static class TicketEndpoints
{
    public static void MapTicketEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/tickets/open", async (CreateTicketSimpleDto request, AppDbContext db, IWebHostEnvironment env, ITicketService ticketService, ITicketTransactionService transactionService) =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Body))
                    return Results.BadRequest("A mensagem (body) é obrigatória.");

                var ticket = await ticketService.CreateTicketWithDefaultsAsync(request.UserSourceId, request.Body, env, db, request.DeptTargetId);

                var txRequest = new CreateTicketTransactionDto
                {
                    UserSourceId = request.UserSourceId,
                    Body = request.Body,
                    TicketId = ticket.Id,
                    CreatedAt = DateTime.UtcNow,
                    AttachBase64 = request.AttachBase64
                };

                await transactionService.CreateTransactionAsync(txRequest, db, env);

                return Results.Created($"/tickets/{ticket.Id}", ticket);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Erro ao criar chamado: {ex.Message}");
            }
        })
        .WithName("OpenTicket")
        .Produces(201)
        .Produces(400);

        app.MapGet("/tickets", async (AppDbContext db) =>
            await db.Tickets
                .Include(t => t.User)
                .Include(t => t.Department)
                .Include(t => t.Category)
                .Include(t => t.StatusTicket)
                .ToListAsync())
        .WithName("GetTickets")
        .Produces(200)
        .Produces(401);

        app.MapGet("/tickets/assigned/{userId}", async (int userId, AppDbContext db) =>
        {
            var closed = await db.StatusTickets.FirstOrDefaultAsync(s => s.Name == "Fechado");
            int? closedId = closed?.Id;

            var lastAssignments = await db.TicketTransactions
                .Where(tt => tt.UserTargetId != null)
                .GroupBy(tt => tt.TicketId)
                .Select(g => new
                {
                    TicketId = g.Key,
                    UserTargetId = g.OrderByDescending(x => x.CreatedAt).Select(x => x.UserTargetId).FirstOrDefault()
                })
                .ToListAsync();

            var assignedTicketIds = lastAssignments
                .Where(a => a.UserTargetId == userId)
                .Select(a => a.TicketId)
                .ToHashSet();

            var result = await db.Tickets
                .Include(t => t.User)
                .Include(t => t.Department)
                .Include(t => t.Category)
                .Include(t => t.StatusTicket)
                .Where(t => assignedTicketIds.Contains(t.Id) && (!closedId.HasValue || t.StatusId != closedId.Value))
                .ToListAsync();

            return Results.Ok(result);
        })
        .WithName("GetAssignedTickets")
        .Produces(200)
        .Produces(401);

        app.MapGet("/tickets/assigned/{userId}/closed", async (int userId, AppDbContext db) =>
        {
            var closed = await db.StatusTickets.FirstOrDefaultAsync(s => s.Name == "Fechado");
            if (closed == null) return Results.Ok(new List<Ticket>());
            var closedId = closed.Id;

            var lastAssignments = await db.TicketTransactions
                .Where(tt => tt.UserTargetId != null)
                .GroupBy(tt => tt.TicketId)
                .Select(g => new
                {
                    TicketId = g.Key,
                    UserTargetId = g.OrderByDescending(x => x.CreatedAt).Select(x => x.UserTargetId).FirstOrDefault()
                })
                .ToListAsync();

            var assignedTicketIds = lastAssignments
                .Where(a => a.UserTargetId == userId)
                .Select(a => a.TicketId)
                .ToHashSet();

            var result = await db.Tickets
                .Include(t => t.User)
                .Include(t => t.Department)
                .Include(t => t.Category)
                .Include(t => t.StatusTicket)
                .Where(t => assignedTicketIds.Contains(t.Id) && t.StatusId == closedId)
                .ToListAsync();

            return Results.Ok(result);
        })
        .WithName("GetAssignedClosedTickets")
        .Produces(200)
        .Produces(401);

        app.MapGet("/tickets/{id}", async (int id, AppDbContext db) =>
        {
            var item = await db.Tickets
                .Include(t => t.User)
                .Include(t => t.Department)
                .Include(t => t.Category)
                .Include(t => t.StatusTicket)
                .FirstOrDefaultAsync(t => t.Id == id);
            return item is not null ? Results.Ok(item) : Results.NotFound();
        })
        .WithName("GetTicketById")
        .Produces(200)
        .Produces(404)
        .Produces(401);

        app.MapPost("/tickets", async (Ticket ticket, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(ticket.UserId);
            var dept = await db.Departments.FindAsync(ticket.DeptTargetId);
            var category = await db.Categories.FindAsync(ticket.CategoryId);
            var status = await db.StatusTickets.FindAsync(ticket.StatusId);

            if (user is null || dept is null || category is null || status is null)
            {
                return Results.BadRequest("Usuário, Departamento, Categoria ou Status inválido.");
            }

            var entity = new Ticket
            {
                UserId = ticket.UserId,
                User = user,
                DeptTargetId = ticket.DeptTargetId,
                Department = dept,
                Description = ticket.Description,
                CategoryId = ticket.CategoryId,
                Category = category,
                OpenDateTime = ticket.OpenDateTime,
                StatusId = ticket.StatusId,
                StatusTicket = status,
                PriorityLevel = ticket.PriorityLevel
            };

            db.Tickets.Add(entity);
            await db.SaveChangesAsync();
            return Results.Created($"/tickets/{entity.Id}", entity);
        })
        .WithName("CreateTicket")
        .Produces(201)
        .Produces(400)
        .Produces(401);

        app.MapPut("/tickets/{id}", async (int id, Ticket input, AppDbContext db) =>
        {
            var item = await db.Tickets.FindAsync(id);
            if (item is null) return Results.NotFound();

            var user = await db.Users.FindAsync(input.UserId);
            var dept = await db.Departments.FindAsync(input.DeptTargetId);
            var category = await db.Categories.FindAsync(input.CategoryId);
            var status = await db.StatusTickets.FindAsync(input.StatusId);

            if (user is null || dept is null || category is null || status is null)
            {
                return Results.BadRequest("Usuário, Departamento, Categoria ou Status inválido.");
            }

            item.UserId = input.UserId;
            item.DeptTargetId = input.DeptTargetId;
            item.Description = input.Description;
            item.CategoryId = input.CategoryId;
            item.OpenDateTime = input.OpenDateTime;
            item.StatusId = input.StatusId;
            item.PriorityLevel = input.PriorityLevel;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("UpdateTicket")
        .Produces(204)
        .Produces(404)
        .Produces(401);

        app.MapDelete("/tickets/{id}", async (int id, AppDbContext db, IWebHostEnvironment env) =>
        {
            var item = await db.Tickets.FindAsync(id);
            if (item is null) return Results.NotFound();

            var txs = await db.TicketTransactions
                .Where(tt => tt.TicketId == id)
                .ToListAsync();

            if (txs.Count > 0)
            {

                foreach (var tx in txs)
                {
                    if (!string.IsNullOrWhiteSpace(tx.AttachUrl))
                    {
                        try
                        {
                            var webRoot = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                            var rel = tx.AttachUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                            var path = Path.Combine(webRoot, rel);
                            if (File.Exists(path)) File.Delete(path);
                        }
                        catch { /* ignore file cleanup errors */ }
                    }
                }

                db.TicketTransactions.RemoveRange(txs);
                await db.SaveChangesAsync();
            }

            db.Tickets.Remove(item);
            await db.SaveChangesAsync();
            return Results.Ok(item);
        })
        .WithName("DeleteTicket")
        .Produces(200)
        .Produces(404)
        .Produces(401);
    }
}
