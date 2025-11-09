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
        app.MapPost("/tickets/open", async (CreateTicketSimpleDto request, AppDbContext db, IWebHostEnvironment env,
            ITicketService ticketService, ITicketTransactionService transactionService, IGptService gpt,
            ILogger<Program> logger) =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Body))
                    return Results.BadRequest("A mensagem é obrigatória.");

                var user = await db.Users.FindAsync(request.UserSourceId);
                if (user == null)
                    return Results.BadRequest("Usuário não encontrado.");

                int deptIdToUse = user.DeptId;
                var deptExists = await db.Departments.AnyAsync(d => d.Id == deptIdToUse);
                if (!deptExists)
                {
                    var acceptDept = await db.Departments.FirstOrDefaultAsync(d => d.AcceptTicket);
                    if (acceptDept != null) deptIdToUse = acceptDept.Id;
                    else
                    {
                        var firstDept = await db.Departments.FirstOrDefaultAsync();
                        if (firstDept != null) deptIdToUse = firstDept.Id;
                        else return Results.BadRequest("Nenhum departamento disponível para associar o chamado.");
                    }
                }

                var allowed = new HashSet<string>(new[]
                {
                    "hardware", "software", "rede", "acesso", "impressora", "e-mail", "sistema corporativo",
                    "desempenho", "segurança", "outro"
                }, StringComparer.OrdinalIgnoreCase);

                string? categoryName = null;
                string? priorityText = null;
                try
                {
                    categoryName = await gpt.CategorizeAsync(request.Body);
                    priorityText = await gpt.ClassifyPriorityAsync(request.Body);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Falha ao chamar IGptService; aplicando fallback.");
                }

                categoryName = string.IsNullOrWhiteSpace(categoryName) ? "outro" : categoryName.Trim();
                if (!allowed.Contains(categoryName))
                {
                    logger.LogInformation("Categoria do GPT inválida ('{Cat}'), usando 'outro'.", categoryName);
                    categoryName = "outro";
                }

                var nameLower = categoryName.ToLowerInvariant();
                var categoryEntity = await db.Categories.FirstOrDefaultAsync(c => c.Description.ToLower() == nameLower);
                if (categoryEntity == null)
                {
                    categoryEntity = new Category
                    {
                        Description = categoryName,
                        DeptId = deptIdToUse
                    };
                    db.Categories.Add(categoryEntity);
                    await db.SaveChangesAsync();
                    logger.LogInformation("Categoria criada no DB: Id={Id} Description={Desc}", categoryEntity.Id,
                        categoryEntity.Description);
                }

                priorityText = string.IsNullOrWhiteSpace(priorityText) ? null : priorityText.Trim().ToLowerInvariant();
                int priorityLevel = priorityText switch
                {
                    "alta" => 3,
                    "média" => 2,
                    "baixa" => 1,
                };

                var ticket =
                    await ticketService.CreateTicketWithDefaultsAsync(request.UserSourceId, request.Body, env, db,
                        deptIdToUse);
                if (ticket == null)
                {
                    logger.LogError("ticketService.CreateTicketWithDefaultsAsync retornou null.");
                    return Results.BadRequest("Erro ao criar chamado (serviço de criação retornou nulo).");
                }

                ticket.DeptTargetId = deptIdToUse;
                ticket.CategoryId = categoryEntity.Id;
                ticket.Category = categoryEntity;
                ticket.PriorityLevel = priorityLevel;

                db.Tickets.Update(ticket);
                await db.SaveChangesAsync();

                var txRequest = new CreateTicketTransactionDto
                {
                    UserSourceId = request.UserSourceId,
                    Body = request.Body,
                    TicketId = ticket.Id,
                    CreatedAt = DateTime.UtcNow,
                    AttachBase64 = request.AttachBase64
                };

                try
                {
                    await transactionService.CreateTransactionAsync(txRequest, db, env);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Falha ao criar transação do ticket.");
                }

                logger.LogInformation("Ticket criado id={TicketId} categoria={Category} prioridade={Priority}",
                    ticket.Id, categoryEntity.Description, priorityLevel);
                return Results.Created($"/tickets/{ticket.Id}", ticket);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "ArgumentException ao criar ticket");
                return Results.BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "InvalidOperationException ao criar ticket");
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado ao criar chamado");
                return Results.BadRequest($"Erro ao criar chamado: {ex.Message}");
            }
        });

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
                        UserTargetId = g.OrderByDescending(x => x.CreatedAt).Select(x => x.UserTargetId)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                var assignedTicketIds = lastAssignments
                    .Where(a => a.UserTargetId == userId)
                    .Select(a => a.TicketId)
                    .ToHashSet();

                // Prefer explicit 'Em andamento' status; accept legacy names as fallback
                var inProgressStatusIds = await db.StatusTickets
                    .Where(s => s.Name == "Em andamento" || s.Name == "Em Progresso" || s.Name == "Atendendo")
                    .Select(s => s.Id)
                    .ToListAsync();

                // If we have defined 'in progress' statuses, return only tickets with those statuses.
                // Otherwise, fall back to the previous behavior (exclude 'Fechado').

                var q = db.Tickets
                    .Include(t => t.User)
                    .Include(t => t.Department)
                    .Include(t => t.Category)
                    .Include(t => t.StatusTicket)
                    .AsQueryable();

                q = q.Where(t => assignedTicketIds.Contains(t.Id));
                if (inProgressStatusIds.Count > 0)
                {
                    q = q.Where(t => inProgressStatusIds.Contains(t.StatusId));
                }
                else
                {
                    q = q.Where(t => !closedId.HasValue || t.StatusId != closedId.Value);
                }

                var tickets = await q.ToListAsync();

                // Build mapping of last assignment per ticket (we already computed lastAssignments above)
                var assignMap = lastAssignments.ToDictionary(a => a.TicketId, a => a.UserTargetId);
                var assignedUserIds = assignMap.Values.Where(id => id.HasValue).Select(id => id!.Value).Distinct()
                    .ToList();
                var users = await db.Users.Where(u => assignedUserIds.Contains(u.Id)).ToListAsync();
                var userMap = users.ToDictionary(u => u.Id);

                var list = tickets.Select(t =>
                {
                    assignMap.TryGetValue(t.Id, out var assignedId);
                    int? assignedUserId = assignedId;
                    object? assignedUser = null;
                    if (assignedUserId.HasValue && userMap.TryGetValue(assignedUserId.Value, out var uu))
                    {
                        assignedUser = new { id = uu.Id, name = uu.Name, email = uu.Email };
                    }

                    // normalize status name for UI
                    var statusName = t.StatusTicket?.Name;
                    if (!string.IsNullOrWhiteSpace(statusName) && (
                            string.Equals(statusName, "Em andamento", StringComparison.OrdinalIgnoreCase)))
                    {
                        statusName = "Em andamento";
                    }

                    return new
                    {
                        id = t.Id,
                        userId = t.UserId,
                        user = t.User == null
                            ? null
                            : new
                            {
                                id = t.User.Id,
                                name = t.User.Name,
                                email = t.User.Email,
                                password = t.User.Password,
                                deptId = t.User.DeptId,
                                department = t.User.Department == null
                                    ? null
                                    : new
                                    {
                                        id = t.User.Department.Id, name = t.User.Department.Name,
                                        acceptTicket = t.User.Department.AcceptTicket
                                    },
                                userStatusId = t.User.UserStatusId,
                                userStatus = t.User.UserStatus == null
                                    ? null
                                    : new { id = t.User.UserStatus.Id, name = t.User.UserStatus.Name },
                                profileId = t.User.ProfileId,
                                userProfile = t.User.UserProfile == null
                                    ? null
                                    : new { id = t.User.UserProfile.Id, name = t.User.UserProfile.Name }
                            },
                        deptTargetId = t.DeptTargetId,
                        department = t.Department == null
                            ? null
                            : new
                            {
                                id = t.Department.Id, name = t.Department.Name, acceptTicket = t.Department.AcceptTicket
                            },
                        description = t.Description,
                        categoryId = t.CategoryId,
                        category = t.Category == null
                            ? null
                            : new
                            {
                                id = t.Category.Id, name = t.Category.Description, deptId = t.Category.DeptId,
                                department = t.Category.Department == null
                                    ? null
                                    : new { id = t.Category.Department.Id, name = t.Category.Department.Name }
                            },
                        openDateTime = t.OpenDateTime,
                        statusId = t.StatusId,
                        statusTicket =
                            t.StatusTicket == null ? null : new { id = t.StatusTicket.Id, name = statusName },
                        priorityLevel = t.PriorityLevel,
                        assignedUserId = assignedUserId,
                        assignedUser = assignedUser
                    };
                }).ToList();

                return Results.Ok(list);
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
                        UserTargetId = g.OrderByDescending(x => x.CreatedAt).Select(x => x.UserTargetId)
                            .FirstOrDefault()
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
                if (item is null) return Results.NotFound();

                // recupera o último usuário atribuído
                var lastAssignUserId = await db.TicketTransactions
                    .Where(tt => tt.TicketId == id && tt.UserTargetId != null)
                    .OrderByDescending(tt => tt.CreatedAt)
                    .Select(tt => tt.UserTargetId)
                    .FirstOrDefaultAsync();

                object? assignedUser = null;
                if (lastAssignUserId.HasValue)
                {
                    var u = await db.Users.FindAsync(lastAssignUserId.Value);
                    if (u != null)
                    {
                        assignedUser = new { id = u.Id, name = u.Name, email = u.Email };
                    }
                }

                var dto = new
                {
                    id = item.Id,
                    userId = item.UserId,
                    user = item.User == null
                        ? null
                        : new
                        {
                            id = item.User.Id, name = item.User.Name, email = item.User.Email,
                            password = item.User.Password, deptId = item.User.DeptId
                        },
                    deptTargetId = item.DeptTargetId,
                    department = item.Department == null
                        ? null
                        : new
                        {
                            id = item.Department.Id, name = item.Department.Name,
                            acceptTicket = item.Department.AcceptTicket
                        },
                    description = item.Description,
                    categoryId = item.CategoryId,
                    category = item.Category == null
                        ? null
                        : new
                        {
                            id = item.Category.Id, name = item.Category.Description, deptId = item.Category.DeptId
                        },
                    openDateTime = item.OpenDateTime,
                    statusId = item.StatusId,
                    statusTicket = item.StatusTicket == null
                        ? null
                        : new { id = item.StatusTicket.Id, name = item.StatusTicket.Name },
                    priorityLevel = item.PriorityLevel,
                    assignedUserId = lastAssignUserId,
                    assignedUser = assignedUser
                };

                return Results.Ok(dto);
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
                                var webRoot = env.WebRootPath ??
                                              Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                                var rel = tx.AttachUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
                                var path = Path.Combine(webRoot, rel);
                                if (File.Exists(path)) File.Delete(path);
                            }
                            catch
                            {
                                /* nda */
                            }
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

        app.MapGet("/tickets", async (int? userSourceId, int? deptTargetId, int? statusId, AppDbContext db) =>
            {
                var q = db.Tickets
                    .Include(t => t.User).ThenInclude(u => u.Department)
                    .Include(t => t.User).ThenInclude(u => u.UserStatus)
                    .Include(t => t.User).ThenInclude(u => u.UserProfile)
                    .Include(t => t.Department)
                    .Include(t => t.Category).ThenInclude(c => c.Department)
                    .Include(t => t.StatusTicket)
                    .AsQueryable();

                if (userSourceId.HasValue) q = q.Where(t => t.UserId == userSourceId.Value);
                if (deptTargetId.HasValue) q = q.Where(t => t.DeptTargetId == deptTargetId.Value);
                if (statusId.HasValue) q = q.Where(t => t.StatusId == statusId.Value);

                var tickets = await q.OrderByDescending(t => t.OpenDateTime).ToListAsync();

                // get last assignment per ticket
                var ticketIds = tickets.Select(t => t.Id).ToList();
                var lastAssignments = await db.TicketTransactions
                    .Where(tt => ticketIds.Contains(tt.TicketId) && tt.UserTargetId != null)
                    .GroupBy(tt => tt.TicketId)
                    .Select(g => new
                    {
                        TicketId = g.Key,
                        UserTargetId = g.OrderByDescending(x => x.CreatedAt).Select(x => x.UserTargetId)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                var userIds = lastAssignments.Where(a => a.UserTargetId.HasValue).Select(a => a.UserTargetId!.Value)
                    .Distinct().ToList();
                var users = await db.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
                var userMap = users.ToDictionary(u => u.Id);

                var list = tickets.Select(t =>
                {
                    var assign = lastAssignments.FirstOrDefault(a => a.TicketId == t.Id);
                    int? assignedUserId = assign?.UserTargetId;
                    object? assignedUser = null;
                    if (assignedUserId.HasValue && userMap.TryGetValue(assignedUserId.Value, out var uu))
                    {
                        assignedUser = new { id = uu.Id, name = uu.Name, email = uu.Email };
                    }

                    return new
                    {
                        id = t.Id,
                        userId = t.UserId,
                        user = t.User == null
                            ? null
                            : new
                            {
                                id = t.User.Id,
                                name = t.User.Name,
                                email = t.User.Email,
                                password = t.User.Password,
                                deptId = t.User.DeptId,
                                department = t.User.Department == null
                                    ? null
                                    : new
                                    {
                                        id = t.User.Department.Id, name = t.User.Department.Name,
                                        acceptTicket = t.User.Department.AcceptTicket
                                    },
                                userStatusId = t.User.UserStatusId,
                                userStatus = t.User.UserStatus == null
                                    ? null
                                    : new { id = t.User.UserStatus.Id, name = t.User.UserStatus.Name },
                                profileId = t.User.ProfileId,
                                userProfile = t.User.UserProfile == null
                                    ? null
                                    : new { id = t.User.UserProfile.Id, name = t.User.UserProfile.Name }
                            },
                        deptTargetId = t.DeptTargetId,
                        department = t.Department == null
                            ? null
                            : new
                            {
                                id = t.Department.Id, name = t.Department.Name, acceptTicket = t.Department.AcceptTicket
                            },
                        description = t.Description,
                        categoryId = t.CategoryId,
                        category = t.Category == null
                            ? null
                            : new
                            {
                                id = t.Category.Id, name = t.Category.Description, deptId = t.Category.DeptId,
                                department = t.Category.Department == null
                                    ? null
                                    : new { id = t.Category.Department.Id, name = t.Category.Department.Name }
                            },
                        openDateTime = t.OpenDateTime,
                        statusId = t.StatusId,
                        statusTicket = t.StatusTicket == null
                            ? null
                            : new { id = t.StatusTicket.Id, name = t.StatusTicket.Name },
                        priorityLevel = t.PriorityLevel,
                        assignedUserId = assignedUserId,
                        assignedUser = assignedUser
                    };
                }).ToList();

                return Results.Ok(list);
            })
            .WithName("GetTickets")
            .Produces(200)
            .Produces(401);

        app.MapPost("/tickets/{id}/gpt-solution",
                async (int id, GptRequestDto? req, AppDbContext db, IGptService gpt, ILogger<Program> logger) =>
                {
                    try
                    {
                        var ticket = await db.Tickets.FindAsync(id);
                        if (ticket == null) return Results.NotFound();

                        var text = string.IsNullOrWhiteSpace(req?.Prompt)
                            ? (ticket.Description ?? $"Chamado #{id}")
                            : req.Prompt;
                        var solution = await gpt.GetSolutionAsync(text);

                        var dto = new GptResponseDto { Result = solution };
                        return Results.Ok(dto);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Erro ao gerar solução GPT para ticket {TicketId}", id);
                        return Results.Problem("Erro ao processar solicitação GPT.");
                    }
                })
            .WithName("GetTicketGptSolution")
            .Produces<GptResponseDto>(200)
            .Produces(404)
            .Produces(500);

    }
}
