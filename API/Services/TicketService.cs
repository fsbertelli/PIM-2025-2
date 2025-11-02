using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

namespace API.Services;

public interface ITicketService
{
    // Now accepts an optional deptTargetId so callers can request a specific department.
    Task<Ticket> CreateTicketWithDefaultsAsync(int userSourceId, string description, IWebHostEnvironment env, AppDbContext db, int? deptTargetId = null);
}

public class TicketService : ITicketService
{
    public async Task<Ticket> CreateTicketWithDefaultsAsync(int userSourceId, string description, IWebHostEnvironment env, AppDbContext db, int? deptTargetId = null)
    {
        // Validate user
        var user = await db.Users.FindAsync(userSourceId);
        if (user is null) throw new ArgumentException($"Usuário inválido: {userSourceId}");

        // If caller provided a department id, validate and use it. Otherwise choose a department that accepts tickets (Suporte). Fallback to any department.
        Department? deptTarget;
        if (deptTargetId.HasValue)
        {
            deptTarget = await db.Departments.FindAsync(deptTargetId.Value);
            if (deptTarget is null) throw new ArgumentException($"Departamento inválido: {deptTargetId.Value}");
            if (!deptTarget.AcceptTicket) throw new InvalidOperationException("Departamento informado não aceita chamados.");
        }
        else
        {
            deptTarget = await db.Departments.FirstOrDefaultAsync(d => d.AcceptTicket) ?? await db.Departments.FirstOrDefaultAsync();
            if (deptTarget is null) throw new InvalidOperationException("Não há departamento configurado para receber chamados.");
        }

        // Choose a category for that department or fallback
        var category = await db.Categories.FirstOrDefaultAsync(c => c.DeptId == deptTarget.Id) ?? await db.Categories.FirstOrDefaultAsync();
        if (category is null) throw new InvalidOperationException("Nenhuma categoria configurada.");

        // Choose status 'Aberto' or first status
        var status = await db.StatusTickets.FirstOrDefaultAsync(s => s.Name == "Aberto") ?? await db.StatusTickets.FirstOrDefaultAsync();
        if (status is null) throw new InvalidOperationException("Nenhum status de ticket configurado.");

        var ticket = new Ticket
        {
            UserId = userSourceId,
            User = user,
            DeptTargetId = deptTarget.Id,
            Department = deptTarget,
            Description = description ?? string.Empty,
            CategoryId = category.Id,
            Category = category,
            OpenDateTime = DateTime.UtcNow,
            StatusId = status.Id,
            StatusTicket = status,
            PriorityLevel = 1
        };

        db.Tickets.Add(ticket);
        await db.SaveChangesAsync();

        return ticket;
    }
}

