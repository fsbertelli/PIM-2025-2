// ...existing code...
using API.Models;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public static class DbSeeder
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Apply any pending migrations (if present) so schema matches the model
        await db.Database.MigrateAsync();

        // Create uploads folder and put a small placeholder image
        var env = scope.ServiceProvider.GetService<IWebHostEnvironment>();
        var webRoot = env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploads = Path.Combine(webRoot, "uploads");
        if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

        // 1x1 PNG base64
        var pngBase64 = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8Xw8AAn8B9pRzE0AAAAASUVORK5CYII=";
        var sampleFile = Path.Combine(uploads, "placeholder.png");
        if (!File.Exists(sampleFile))
        {
            try
            {
                var clean = pngBase64?.Trim() ?? string.Empty;
                // remove data URI prefix if present
                if (clean.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    var comma = clean.IndexOf(',');
                    if (comma >= 0) clean = clean.Substring(comma + 1);
                }

                // remove whitespace/newlines
                clean = clean.Replace("\r", string.Empty).Replace("\n", string.Empty).Replace(" ", string.Empty);

                byte[] bytes;
                try
                {
                    bytes = Convert.FromBase64String(clean);
                }
                catch (FormatException)
                {
                    // fallback to a minimal 1x1 PNG binary (transparent)
                    bytes = new byte[] {
                        137,80,78,71,13,10,26,10,0,0,0,13,73,72,68,82,0,0,0,1,0,0,0,1,8,6,0,0,0,31,21,196,137,0,0,0,12,73,68,65,84,8,215,99,0,1,0,0,5,0,1,13,10,39,57,0,0,0,0,73,69,78,68,174,66,96,130
                    };
                }

                await File.WriteAllBytesAsync(sampleFile, bytes);
            }
            catch (Exception ex)
            {
                // swallow seeder image creation errors so seeding can continue
                Console.WriteLine($"Warning: failed to create placeholder image: {ex.Message}");
            }
        }

        // Seed Departments
        if (!await db.Departments.AnyAsync())
        {
            var depts = new List<Department>
            {
                new Department{ Name = "Suporte", AcceptTicket = true },
                new Department{ Name = "Financeiro", AcceptTicket = false }
            };
            db.Departments.AddRange(depts);
            await db.SaveChangesAsync();
        }

        // Seed UserProfiles
        if (!await db.UserProfiles.AnyAsync())
        {
            var profiles = new List<UserProfile>
            {
                new UserProfile{ Name = "Operador" },
                new UserProfile{ Name = "Gerente" }
            };
            db.UserProfiles.AddRange(profiles);
            await db.SaveChangesAsync();
        }

        // Seed UserStatus
        if (!await db.UserStatus.AnyAsync())
        {
            var statuses = new List<UserStatus>
            {
                new UserStatus{ Name = "Ativo" },
                new UserStatus{ Name = "Inativo" }
            };
            db.UserStatus.AddRange(statuses);
            await db.SaveChangesAsync();
        }

        // Seed StatusTickets (statuses for tickets)
        if (!await db.StatusTickets.AnyAsync())
        {
            var statusTickets = new List<StatusTicket>
            {
                new StatusTicket{ Name = "Aberto" },
                new StatusTicket{ Name = "Em Progresso" },
                new StatusTicket{ Name = "Fechado" }
            };
            db.StatusTickets.AddRange(statusTickets);
            await db.SaveChangesAsync();
        }

        // Seed Categories
        if (!await db.Categories.AnyAsync())
        {
            var deptForCategory = await db.Departments.FirstAsync();
            var categories = new List<Category>
            {
                new Category{ Description = "Bug", DeptId = deptForCategory.Id },
                new Category{ Description = "Melhoria", DeptId = deptForCategory.Id }
            };
            db.Categories.AddRange(categories);
            await db.SaveChangesAsync();
        }

        // Seed Users
        if (!await db.Users.AnyAsync())
        {
            var dept = await db.Departments.FirstAsync();
            var profile = await db.UserProfiles.FirstAsync();
            var status = await db.UserStatus.FirstAsync();

            var users = new List<User>
            {
                new User{ Name = "Alice", Email = "alice@example.com", Password = PasswordService.HashPassword("password123"), DeptId = dept.Id, ProfileId = profile.Id, UserStatusId = status.Id },
                new User{ Name = "Bob", Email = "bob@example.com", Password = PasswordService.HashPassword("password123"), DeptId = dept.Id, ProfileId = profile.Id, UserStatusId = status.Id }
            };
            db.Users.AddRange(users);
            await db.SaveChangesAsync();
        }

        // Seed Tickets
        if (!await db.Tickets.AnyAsync())
        {
            var user = await db.Users.FirstAsync();
            var deptTarget = await db.Departments.FirstAsync();
            var category = await db.Categories.FirstAsync();
            var statusTicket = await db.StatusTickets.FirstAsync();

            var tickets = new List<Ticket>
            {
                new Ticket{ UserId = user.Id, DeptTargetId = deptTarget.Id, Description = "Erro ao acessar sistema", CategoryId = category.Id, StatusId = statusTicket.Id, PriorityLevel = 1 },
            };
            db.Tickets.AddRange(tickets);
            await db.SaveChangesAsync();
        }

        // Seed TicketTransactions
        if (!await db.TicketTransactions.AnyAsync())
        {
            var ticket = await db.Tickets.FirstAsync();
            var users = await db.Users.Take(2).ToListAsync();

            var tt = new TicketTransaction
            {
                Id = Guid.NewGuid(),
                UserSourceId = users[0].Id,
                UserSource = users[0],
                UserTargetId = users[1].Id,
                UserTarget = users[1],
                Body = "Mensagem inicial com anexo",
                TicketId = ticket.Id,
                Ticket = ticket,
                CreatedAt = DateTime.UtcNow,
                AttachUrl = "/uploads/placeholder.png"
            };
            db.TicketTransactions.Add(tt);
            await db.SaveChangesAsync();
        }
    }
}
