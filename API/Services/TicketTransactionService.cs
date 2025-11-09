// ...existing code...
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;
using API.Models.DTO;

namespace API.Services;

public interface ITicketTransactionService
{
    Task<TicketTransaction> CreateTransactionAsync(CreateTicketTransactionDto request, AppDbContext db, IWebHostEnvironment env);
    Task<TicketTransaction> CreateTransactionFromFormAsync(int userSourceId, int? userTargetId, int ticketId, string? body, string? attachUrl, AppDbContext db);
}

public class TicketTransactionService : ITicketTransactionService
{
    private readonly IFileStorageService _fileStorage;

    public TicketTransactionService(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<TicketTransaction> CreateTransactionAsync(CreateTicketTransactionDto request, AppDbContext db, IWebHostEnvironment env)
    {
        var userSource = await db.Users
            .Include(u => u.Department)
            .Include(u => u.UserStatus)
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == request.UserSourceId);

        User? userTarget = null;
        if (request.UserTargetId.HasValue)
        {
            userTarget = await db.Users
                .Include(u => u.Department)
                .Include(u => u.UserStatus)
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == request.UserTargetId.Value);
        }

        var ticket = await db.Tickets.FindAsync(request.TicketId);

        if (userSource is null)
            throw new ArgumentException("Usuário origem inválido: " + request.UserSourceId);
        if (request.UserTargetId.HasValue && userTarget is null)
            throw new ArgumentException("Usuário destino inválido: " + request.UserTargetId);
        if (request.UserTargetId.HasValue && userTarget is not null)
        {
            if (userTarget.Department is null || !userTarget.Department.AcceptTicket)
                throw new ArgumentException("Usuário destino não pertence ao departamento de suporte ou não aceita chamados: " + request.UserTargetId);
        }
        if (ticket is null)
            throw new ArgumentException("Ticket inválido: " + request.TicketId);

        string? attachUrl = null;
        if (!string.IsNullOrWhiteSpace(request.AttachBase64))
        {
            attachUrl = await _fileStorage.SaveBase64Async(request.AttachBase64, env);
            if (attachUrl is null)
                throw new InvalidOperationException("Failed to save attachment.");
        }

        // When assigning a ticket (UserTargetId present), set status to 'Em andamento' (preferred)
        if (request.UserTargetId.HasValue)
        {
            // Ensure a canonical 'Em andamento' status exists, create if missing
            var inProgress = await db.StatusTickets.FirstOrDefaultAsync(s => s.Name == "Em andamento");
            if (inProgress == null)
            {
                inProgress = new Models.StatusTicket { Name = "Em andamento" };
                db.StatusTickets.Add(inProgress);
                await db.SaveChangesAsync();
                inProgress = await db.StatusTickets.FirstAsync(s => s.Name == "Em andamento");
            }
            if (inProgress != null && ticket.StatusId != inProgress.Id)
            {
                ticket.StatusId = inProgress.Id;
            }
        }

        var entity = new TicketTransaction
        {
            Id = Guid.NewGuid(),
            UserSourceId = request.UserSourceId,
            UserSource = userSource,
            UserTargetId = request.UserTargetId,
            UserTarget = userTarget,
            Body = request.Body,
            TicketId = request.TicketId,
            Ticket = ticket,
            CreatedAt = request.CreatedAt ?? DateTime.UtcNow,
            AttachUrl = attachUrl
        };

        db.TicketTransactions.Add(entity);
        await db.SaveChangesAsync();

        return entity;
    }

    public async Task<TicketTransaction> CreateTransactionFromFormAsync(int userSourceId, int? userTargetId, int ticketId, string? body, string? attachUrl, AppDbContext db)
    {
        var userSource = await db.Users
            .Include(u => u.Department)
            .Include(u => u.UserStatus)
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.Id == userSourceId);

        User? userTarget = null;
        if (userTargetId.HasValue)
        {
            userTarget = await db.Users
                .Include(u => u.Department)
                .Include(u => u.UserStatus)
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == userTargetId.Value);
        }

        var ticket = await db.Tickets.FindAsync(ticketId);

        if (userSource is null)
            throw new ArgumentException("Usuário origem inválido: " + userSourceId);
        if (userTargetId.HasValue && userTarget is null)
            throw new ArgumentException("Usuário destino inválido: " + userTargetId);
        if (userTargetId.HasValue && userTarget is not null)
        {
            if (userTarget.Department is null || !userTarget.Department.AcceptTicket)
                throw new ArgumentException("Usuário destino não pertence ao departamento de suporte ou não aceita chamados: " + userTargetId);
        }
        if (ticket is null)
            throw new ArgumentException("Ticket inválido: " + ticketId);

        if (userTargetId.HasValue)
        {
            // Ensure canonical 'Em andamento' status exists, create if missing
            var inProgress = await db.StatusTickets.FirstOrDefaultAsync(s => s.Name == "Em andamento");
            if (inProgress == null)
            {
                inProgress = new Models.StatusTicket { Name = "Em andamento" };
                db.StatusTickets.Add(inProgress);
                await db.SaveChangesAsync();
                inProgress = await db.StatusTickets.FirstAsync(s => s.Name == "Em andamento");
            }
            if (inProgress != null && ticket.StatusId != inProgress.Id)
            {
                ticket.StatusId = inProgress.Id;
            }
        }

        var entity = new TicketTransaction
        {
            Id = Guid.NewGuid(),
            UserSourceId = userSourceId,
            UserSource = userSource,
            UserTargetId = userTargetId,
            UserTarget = userTarget,
            Body = body,
            TicketId = ticketId,
            Ticket = ticket,
            CreatedAt = DateTime.UtcNow,
            AttachUrl = attachUrl
        };

        db.TicketTransactions.Add(entity);
        await db.SaveChangesAsync();

        return entity;
    }
}
