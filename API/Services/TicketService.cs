// csharp
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

namespace API.Services;

public interface ITicketService
{
    // Mantido o parâmetro opcional para compatibilidade, mas será ignorado pela implementação.
    Task<Ticket> CreateTicketWithDefaultsAsync(int userSourceId, string description, IWebHostEnvironment env, AppDbContext db, int? deptTargetId = null);
}

public class TicketService : ITicketService
{
    public async Task<Ticket> CreateTicketWithDefaultsAsync(int userSourceId, string description, IWebHostEnvironment env, AppDbContext db, int? deptTargetId = null)
    {
        // Validar usuário
        var user = await db.Users.FindAsync(userSourceId);
        if (user is null) throw new ArgumentException($"Usuário inválido: {userSourceId}");

        // SEMPRE usar o departamento do usuário
        var userDeptId = user.DeptId;
        var deptTarget = await db.Departments.FindAsync(userDeptId);
        if (deptTarget is null) throw new InvalidOperationException("Departamento do usuário inválido.");

        // Escolher categoria do departamento do usuário ou fallback para qualquer categoria
        var category = await db.Categories.FirstOrDefaultAsync(c => c.DeptId == deptTarget.Id)
                       ?? await db.Categories.FirstOrDefaultAsync();
        if (category is null) throw new InvalidOperationException("Nenhuma categoria configurada.");

        // Garantir status "Aberto" (cria se ausente)
        var status = await db.StatusTickets.FirstOrDefaultAsync(s => s.Name == "Aberto");
        if (status is null)
        {
            status = new StatusTicket { Name = "Aberto" };
            db.StatusTickets.Add(status);
            await db.SaveChangesAsync();
            // recarregar para garantir o Id
            status = await db.StatusTickets.FirstAsync(s => s.Name == "Aberto");
        }

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