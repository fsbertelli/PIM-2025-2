using API.Data;
using API.Models;
using API.Models.DTO;
using API.Services;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class TicketTransactionEndpoints
{
    public static void MapTicketTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/tickettransactions", async (AppDbContext db) =>
        {
            // Load entities with navigations first, then map in-memory to avoid EF translation issues
            var entities = await db.TicketTransactions
                .Include(tt => tt.UserSource).ThenInclude(u => u.Department)
                .Include(tt => tt.UserSource).ThenInclude(u => u.UserStatus)
                .Include(tt => tt.UserSource).ThenInclude(u => u.UserProfile)
                .Include(tt => tt.UserTarget).ThenInclude(u => u.Department)
                .Include(tt => tt.UserTarget).ThenInclude(u => u.UserStatus)
                .Include(tt => tt.UserTarget).ThenInclude(u => u.UserProfile)
                .Include(tt => tt.Ticket)
                .ToListAsync();

            var list = entities.Select(tt => new TicketTransactionDto
            {
                Id = tt.Id,
                UserSourceId = tt.UserSourceId,
                UserSource = tt.UserSource == null ? null : new UserDto
                {
                    Id = tt.UserSource.Id,
                    Name = tt.UserSource.Name,
                    Email = tt.UserSource.Email,
                    Department = tt.UserSource.Department == null ? null : new DepartmentDto { Id = tt.UserSource.DeptId, Name = tt.UserSource.Department.Name },
                    UserStatus = tt.UserSource.UserStatus == null ? null : new UserStatusDto { Id = tt.UserSource.UserStatusId, Name = tt.UserSource.UserStatus.Name },
                    UserProfile = tt.UserSource.UserProfile == null ? null : new UserProfileDto { Id = tt.UserSource.ProfileId, Name = tt.UserSource.UserProfile.Name }
                },
                UserTargetId = tt.UserTargetId,
                UserTarget = tt.UserTarget == null ? null : new UserDto
                {
                    Id = tt.UserTarget.Id,
                    Name = tt.UserTarget.Name,
                    Email = tt.UserTarget.Email,
                    Department = tt.UserTarget.Department == null ? null : new DepartmentDto { Id = tt.UserTarget.DeptId, Name = tt.UserTarget.Department.Name },
                    UserStatus = tt.UserTarget.UserStatus == null ? null : new UserStatusDto { Id = tt.UserTarget.UserStatusId, Name = tt.UserTarget.UserStatus.Name },
                    UserProfile = tt.UserTarget.UserProfile == null ? null : new UserProfileDto { Id = tt.UserTarget.ProfileId, Name = tt.UserTarget.UserProfile.Name }
                },
                Body = tt.Body,
                TicketId = tt.TicketId,
                Ticket = tt.Ticket == null ? null : new TicketDto
                {
                    Id = tt.Ticket.Id,
                    Description = tt.Ticket.Description,
                    CategoryId = tt.Ticket.CategoryId,
                    StatusId = tt.Ticket.StatusId,
                    UserId = tt.Ticket.UserId,
                    DeptTargetId = tt.Ticket.DeptTargetId,
                    OpenDateTime = tt.Ticket.OpenDateTime,
                    PriorityLevel = tt.Ticket.PriorityLevel
                },
                CreatedAt = tt.CreatedAt,
                AttachUrl = tt.AttachUrl
            }).ToList();

            return Results.Ok(list);
        })
        .WithName("GetTicketTransactions")
        .Produces(200)
        .Produces(401);

        app.MapGet("/tickettransactions/ticket/{ticketId}", async (int ticketId, AppDbContext db) =>
        {
            var entities = await db.TicketTransactions
                .Where(tt => tt.TicketId == ticketId)
                .Include(tt => tt.UserSource).ThenInclude(u => u.Department)
                .Include(tt => tt.UserSource).ThenInclude(u => u.UserStatus)
                .Include(tt => tt.UserSource).ThenInclude(u => u.UserProfile)
                .Include(tt => tt.UserTarget).ThenInclude(u => u.Department)
                .Include(tt => tt.UserTarget).ThenInclude(u => u.UserStatus)
                .Include(tt => tt.UserTarget).ThenInclude(u => u.UserProfile)
                .Include(tt => tt.Ticket)
                .OrderBy(tt => tt.CreatedAt)
                .ToListAsync();

            var list = entities.Select(tt => new TicketTransactionDto
            {
                Id = tt.Id,
                UserSourceId = tt.UserSourceId,
                UserSource = tt.UserSource == null ? null : new UserDto
                {
                    Id = tt.UserSource.Id,
                    Name = tt.UserSource.Name,
                    Email = tt.UserSource.Email,
                    Department = tt.UserSource.Department == null ? null : new DepartmentDto { Id = tt.UserSource.DeptId, Name = tt.UserSource.Department.Name },
                    UserStatus = tt.UserSource.UserStatus == null ? null : new UserStatusDto { Id = tt.UserSource.UserStatusId, Name = tt.UserSource.UserStatus.Name },
                    UserProfile = tt.UserSource.UserProfile == null ? null : new UserProfileDto { Id = tt.UserSource.ProfileId, Name = tt.UserSource.UserProfile.Name }
                },
                UserTargetId = tt.UserTargetId,
                UserTarget = tt.UserTarget == null ? null : new UserDto
                {
                    Id = tt.UserTarget.Id,
                    Name = tt.UserTarget.Name,
                    Email = tt.UserTarget.Email,
                    Department = tt.UserTarget.Department == null ? null : new DepartmentDto { Id = tt.UserTarget.DeptId, Name = tt.UserTarget.Department.Name },
                    UserStatus = tt.UserTarget.UserStatus == null ? null : new UserStatusDto { Id = tt.UserTarget.UserStatusId, Name = tt.UserTarget.UserStatus.Name },
                    UserProfile = tt.UserTarget.UserProfile == null ? null : new UserProfileDto { Id = tt.UserTarget.ProfileId, Name = tt.UserTarget.UserProfile.Name }
                },
                Body = tt.Body,
                TicketId = tt.TicketId,
                Ticket = tt.Ticket == null ? null : new TicketDto
                {
                    Id = tt.Ticket.Id,
                    Description = tt.Ticket.Description,
                    CategoryId = tt.Ticket.CategoryId,
                    StatusId = tt.Ticket.StatusId,
                    UserId = tt.Ticket.UserId,
                    DeptTargetId = tt.Ticket.DeptTargetId,
                    OpenDateTime = tt.Ticket.OpenDateTime,
                    PriorityLevel = tt.Ticket.PriorityLevel
                },
                CreatedAt = tt.CreatedAt,
                AttachUrl = tt.AttachUrl
            }).ToList();

            return Results.Ok(list);
        })
        .WithName("GetTicketTransactionsByTicket")
        .Produces(200)
        .Produces(401);

        app.MapGet("/tickets/assignment-status", async (AppDbContext db) =>
        {
            var lastAssignments = await db.TicketTransactions
                .Where(tt => tt.UserTargetId != null)
                .GroupBy(tt => tt.TicketId)
                .Select(g => new
                {
                    TicketId = g.Key,
                    UserTargetId = g.OrderByDescending(x => x.CreatedAt).Select(x => x.UserTargetId).FirstOrDefault()
                })
                .ToListAsync();

            return Results.Ok(lastAssignments);
        })
        .WithName("GetTicketAssignmentStatus")
        .Produces(200);

        app.MapGet("/tickettransactions/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var tt = await db.TicketTransactions
                .Include(x => x.UserSource).ThenInclude(u => u.Department)
                .Include(x => x.UserSource).ThenInclude(u => u.UserStatus)
                .Include(x => x.UserSource).ThenInclude(u => u.UserProfile)
                .Include(x => x.UserTarget).ThenInclude(u => u.Department)
                .Include(x => x.UserTarget).ThenInclude(u => u.UserStatus)
                .Include(x => x.UserTarget).ThenInclude(u => u.UserProfile)
                .Include(x => x.Ticket)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (tt is null) return Results.NotFound();

            var dto = new TicketTransactionDto
            {
                Id = tt.Id,
                UserSourceId = tt.UserSourceId,
                UserSource = tt.UserSource == null ? null : new UserDto
                {
                    Id = tt.UserSource.Id,
                    Name = tt.UserSource.Name,
                    Email = tt.UserSource.Email,
                    Department = new DepartmentDto { Id = tt.UserSource.DeptId, Name = tt.UserSource.Department == null ? null : tt.UserSource.Department.Name },
                    UserStatus = new UserStatusDto { Id = tt.UserSource.UserStatusId, Name = tt.UserSource.UserStatus == null ? null : tt.UserSource.UserStatus.Name },
                    UserProfile = new UserProfileDto { Id = tt.UserSource.ProfileId, Name = tt.UserSource.UserProfile == null ? null : tt.UserSource.UserProfile.Name }
                },
                UserTargetId = tt.UserTargetId,
                UserTarget = tt.UserTarget == null ? null : new UserDto
                {
                    Id = tt.UserTarget.Id,
                    Name = tt.UserTarget.Name,
                    Email = tt.UserTarget.Email,
                    Department = new DepartmentDto { Id = tt.UserTarget.DeptId, Name = tt.UserTarget.Department == null ? null : tt.UserTarget.Department.Name },
                    UserStatus = new UserStatusDto { Id = tt.UserTarget.UserStatusId, Name = tt.UserTarget.UserStatus == null ? null : tt.UserTarget.UserStatus.Name },
                    UserProfile = new UserProfileDto { Id = tt.UserTarget.ProfileId, Name = tt.UserTarget.UserProfile == null ? null : tt.UserTarget.UserProfile.Name }
                },
                Body = tt.Body,
                TicketId = tt.TicketId,
                Ticket = tt.Ticket == null ? null : new TicketDto
                {
                    Id = tt.Ticket.Id,
                    Description = tt.Ticket.Description,
                    CategoryId = tt.Ticket.CategoryId,
                    StatusId = tt.Ticket.StatusId,
                    UserId = tt.Ticket.UserId,
                    DeptTargetId = tt.Ticket.DeptTargetId,
                    OpenDateTime = tt.Ticket.OpenDateTime,
                    PriorityLevel = tt.Ticket.PriorityLevel
                },
                CreatedAt = tt.CreatedAt,
                AttachUrl = tt.AttachUrl
            };

            return Results.Ok(dto);
        })
        .WithName("GetTicketTransactionById")
        .Produces(200)
        .Produces(404)
        .Produces(401);

        app.MapPost("/tickettransactions", async (CreateTicketTransactionDto request, AppDbContext db, IWebHostEnvironment env, ITicketTransactionService transactionService) =>
        {
            try
            {
                var entity = await transactionService.CreateTransactionAsync(request, db, env);

                // map to DTO
                var createdDto = new TicketTransactionDto
                {
                    Id = entity.Id,
                    UserSourceId = entity.UserSourceId,
                    UserSource = entity.UserSource == null ? null : new UserDto
                    {
                        Id = entity.UserSource.Id,
                        Name = entity.UserSource.Name,
                        Email = entity.UserSource.Email,
                        Department = entity.UserSource.Department == null ? null : new DepartmentDto { Id = entity.UserSource.DeptId, Name = entity.UserSource.Department.Name },
                        UserStatus = entity.UserSource.UserStatus == null ? null : new UserStatusDto { Id = entity.UserSource.UserStatusId, Name = entity.UserSource.UserStatus.Name },
                        UserProfile = entity.UserSource.UserProfile == null ? null : new UserProfileDto { Id = entity.UserSource.ProfileId, Name = entity.UserSource.UserProfile.Name }
                    },
                    UserTargetId = entity.UserTargetId,
                    UserTarget = entity.UserTarget == null ? null : new UserDto
                    {
                        Id = entity.UserTarget.Id,
                        Name = entity.UserTarget.Name,
                        Email = entity.UserTarget.Email,
                        Department = entity.UserTarget.Department == null ? null : new DepartmentDto { Id = entity.UserTarget.DeptId, Name = entity.UserTarget.Department.Name },
                        UserStatus = entity.UserTarget.UserStatus == null ? null : new UserStatusDto { Id = entity.UserTarget.UserStatusId, Name = entity.UserTarget.UserStatus.Name },
                        UserProfile = entity.UserTarget.UserProfile == null ? null : new UserProfileDto { Id = entity.UserTarget.ProfileId, Name = entity.UserTarget.UserProfile.Name }
                    },
                    Body = entity.Body,
                    TicketId = entity.TicketId,
                    Ticket = entity.Ticket == null ? null : new TicketDto
                    {
                        Id = entity.Ticket.Id,
                        Description = entity.Ticket.Description,
                        CategoryId = entity.Ticket.CategoryId,
                        StatusId = entity.Ticket.StatusId,
                        UserId = entity.Ticket.UserId,
                        DeptTargetId = entity.Ticket.DeptTargetId,
                        OpenDateTime = entity.Ticket.OpenDateTime,
                        PriorityLevel = entity.Ticket.PriorityLevel
                    },
                    CreatedAt = entity.CreatedAt,
                    AttachUrl = entity.AttachUrl
                };

                return Results.Created("/tickettransactions/" + entity.Id, createdDto);
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
                return Results.BadRequest($"Erro ao criar transação: {ex.Message}");
            }
        })
        .WithName("CreateTicketTransaction")
        .Produces(201)
        .Produces(400)
        .Produces(401);

        // Multipart upload endpoint (for mobile clients)
        app.MapPost("/tickettransactions/upload", async (HttpRequest request, IFormFile? file, [FromForm] int userSourceId, [FromForm] int? userTargetId, [FromForm] int ticketId, [FromForm] string? body, AppDbContext db, IWebHostEnvironment env, IFileStorageService fileStorage, ITicketTransactionService transactionService) =>
        {
            try
            {
                // save file if provided
                string? attachUrl = null;
                if (file is not null && file.Length > 0)
                {
                    // validate size
                    const long maxBytes = 5 * 1024 * 1024;
                    if (file.Length > maxBytes) return Results.BadRequest($"Arquivo muito grande. Máximo permitido: {maxBytes} bytes.");

                    // basic content type validation
                    var allowed = new[] { "image/png", "image/jpeg", "image/jpg", "image/gif", "image/webp" };
                    if (!allowed.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
                        return Results.BadRequest("Tipo de arquivo não permitido. Apenas imagens (png, jpeg, gif, webp).");

                    attachUrl = await fileStorage.SaveFormFileAsync(file, env);
                    if (attachUrl is null) return Results.BadRequest("Falha ao salvar o arquivo.");
                }

                var entity = await transactionService.CreateTransactionFromFormAsync(userSourceId, userTargetId, ticketId, body, attachUrl, db);

                var createdDto2 = new TicketTransactionDto
                {
                    Id = entity.Id,
                    UserSourceId = entity.UserSourceId,
                    UserSource = entity.UserSource == null ? null : new UserDto
                    {
                        Id = entity.UserSource.Id,
                        Name = entity.UserSource.Name,
                        Email = entity.UserSource.Email,
                        Department = entity.UserSource.Department == null ? null : new DepartmentDto { Id = entity.UserSource.DeptId, Name = entity.UserSource.Department.Name },
                        UserStatus = entity.UserSource.UserStatus == null ? null : new UserStatusDto { Id = entity.UserSource.UserStatusId, Name = entity.UserSource.UserStatus.Name },
                        UserProfile = entity.UserSource.UserProfile == null ? null : new UserProfileDto { Id = entity.UserSource.ProfileId, Name = entity.UserSource.UserProfile.Name }
                    },
                    UserTargetId = entity.UserTargetId,
                    UserTarget = entity.UserTarget == null ? null : new UserDto
                    {
                        Id = entity.UserTarget.Id,
                        Name = entity.UserTarget.Name,
                        Email = entity.UserTarget.Email,
                        Department = entity.UserTarget.Department == null ? null : new DepartmentDto { Id = entity.UserTarget.DeptId, Name = entity.UserTarget.Department.Name },
                        UserStatus = entity.UserTarget.UserStatus == null ? null : new UserStatusDto { Id = entity.UserTarget.UserStatusId, Name = entity.UserTarget.UserStatus.Name },
                        UserProfile = entity.UserTarget.UserProfile == null ? null : new UserProfileDto { Id = entity.UserTarget.ProfileId, Name = entity.UserTarget.UserProfile.Name }
                    },
                    Body = entity.Body,
                    TicketId = entity.TicketId,
                    Ticket = entity.Ticket == null ? null : new TicketDto
                    {
                        Id = entity.Ticket.Id,
                        Description = entity.Ticket.Description,
                        CategoryId = entity.Ticket.CategoryId,
                        StatusId = entity.Ticket.StatusId,
                        UserId = entity.Ticket.UserId,
                        DeptTargetId = entity.Ticket.DeptTargetId,
                        OpenDateTime = entity.Ticket.OpenDateTime,
                        PriorityLevel = entity.Ticket.PriorityLevel
                    },
                    CreatedAt = entity.CreatedAt,
                    AttachUrl = entity.AttachUrl
                };

                return Results.Created("/tickettransactions/" + entity.Id, createdDto2);
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
                return Results.BadRequest($"Erro ao criar transação: {ex.Message}");
            }
        })
        .WithName("UploadTicketTransaction")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces(201)
        .Produces(400)
        .DisableAntiforgery();

        // Require GUID constraint here as well to avoid accidental matching of literal
        // route names like "upload".
        app.MapDelete("/tickettransactions/{id:guid}", async (Guid id, AppDbContext db) =>
        {
            var item = await db.TicketTransactions.FindAsync(id);
            if (item is null) return Results.NotFound();
            db.TicketTransactions.Remove(item);
            await db.SaveChangesAsync();

            // return DTO (no password exposure)
            var removedDto = new TicketTransactionDto
            {
                Id = item.Id,
                UserSourceId = item.UserSourceId,
                UserTargetId = item.UserTargetId,
                Body = item.Body,
                TicketId = item.TicketId,
                CreatedAt = item.CreatedAt,
                AttachUrl = item.AttachUrl
            };

            return Results.Ok(removedDto);
        })
        .WithName("DeleteTicketTransaction")
        .Produces(200)
        .Produces(404)
        .Produces(401);

        app.MapGet("/tickets/{ticketId}/attachments", async (int ticketId, AppDbContext db) =>
        {
            var urls = await db.TicketTransactions
                .Where(tt => tt.TicketId == ticketId && tt.AttachUrl != null && tt.AttachUrl != "")
                .OrderBy(tt => tt.CreatedAt)
                .Select(tt => tt.AttachUrl!)
                .ToListAsync();
            return Results.Ok(urls);
        })
        .WithName("GetTicketAttachments")
        .Produces(200)
        .Produces(401);
    }
}
