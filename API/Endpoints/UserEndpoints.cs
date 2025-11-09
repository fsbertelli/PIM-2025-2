using API.Data;
using API.Models;
using API.Models.DTO;
using Microsoft.EntityFrameworkCore;
using API.Services;

namespace API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        // Helpers locais
        static UserDto BuildUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Department = user.Department is null ? null : new DepartmentDto { Id = user.Department.Id, Name = user.Department.Name },
                UserStatus = user.UserStatus is null ? null : new UserStatusDto { Id = user.UserStatus.Id, Name = user.UserStatus.Name },
                UserProfile = user.UserProfile is null ? null : new UserProfileDto { Id = user.UserProfile.Id, Name = user.UserProfile.Name }
            };
        }

        static async Task<(Department? dept, UserStatus? status, UserProfile? profile)> FindRelatedSequentialAsync(AppDbContext db, int deptId, int statusId, int profileId)
        {
            var department = await db.Departments.FindAsync(deptId);
            var status = await db.UserStatus.FindAsync(statusId);
            var profile = await db.UserProfiles.FindAsync(profileId);
            
            return (department, status, profile);

        }

        app.MapPost("/users", async (CreateUserDto dto, AppDbContext db) =>
        {
            var (department, userStatus, userProfile) = await FindRelatedSequentialAsync(db, dto.DeptId, dto.UserStatusId, dto.ProfileId);
            if (department is null || userStatus is null || userProfile is null)
                return Results.BadRequest(new { Message = "Departamento, Status ou Perfil inválido." });

            if (string.IsNullOrEmpty(dto.Password))
                return Results.BadRequest(new { Message = "Senha é obrigatória." });

            var normalizedEmail = dto.Email?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(normalizedEmail))
                return Results.BadRequest(new { Message = "Email é obrigatório." });

            var exists = await db.Users.AnyAsync(u => u.Email != null && u.Email.ToLower() == normalizedEmail);
            if (exists)
                return Results.Conflict(new { Message = "Email já cadastrado." });

            var user = new User
            {
                Name = dto.Name,
                Email = normalizedEmail,
                Password = PasswordService.HashPassword(dto.Password),
                DeptId = dto.DeptId,
                Department = department,
                UserStatusId = dto.UserStatusId,
                UserStatus = userStatus,
                ProfileId = dto.ProfileId,
                UserProfile = userProfile
            };

            db.Users.Add(user);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Results.Conflict(new { Message = "Não foi possível criar o usuário devido a conflito de dados." });
            }
            catch (Exception)
            {
                return Results.Problem(detail: "Erro interno ao criar usuário.", statusCode: 500);
            }

            var userDto = BuildUserDto(user);
            return Results.Created($"/users/{user.Id}", userDto);
        })
        .WithName("CreateUser")
        .Produces(201)
        .Produces(400)
        .Produces(409)
        .Produces(500)
        .Produces(401);

        app.MapPut("/users/{id}", async (int id, UpdateUserDto dto, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound(new { Message = "Usuário não encontrado." });

            var (department, userStatus, userProfile) = await FindRelatedSequentialAsync(db, dto.DeptId, dto.UserStatusId, dto.ProfileId);
            if (department is null || userStatus is null || userProfile is null)
                return Results.BadRequest(new { Message = "Departamento, Status ou Perfil inválido." });

            var normalizedNewEmail = dto.Email?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(normalizedNewEmail))
                return Results.BadRequest(new { Message = "Email é obrigatório." });

            if (!string.Equals(user.Email, normalizedNewEmail, StringComparison.OrdinalIgnoreCase))
            {
                var conflict = await db.Users.AnyAsync(u => u.Id != id && u.Email != null && u.Email.ToLower() == normalizedNewEmail);
                if (conflict)
                    return Results.Conflict(new { Message = "Email já cadastrado por outro usuário." });

                user.Email = normalizedNewEmail;
                db.Entry(user).Property(u => u.Email).IsModified = true;
            }

            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.Password = PasswordService.HashPassword(dto.Password);
                db.Entry(user).Property(u => u.Password).IsModified = true;
            }

            user.DeptId = dto.DeptId;
            user.UserStatusId = dto.UserStatusId;
            user.ProfileId = dto.ProfileId;
            db.Entry(user).Property(u => u.DeptId).IsModified = true;
            db.Entry(user).Property(u => u.UserStatusId).IsModified = true;
            db.Entry(user).Property(u => u.ProfileId).IsModified = true;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Results.Conflict(new { Message = "Não foi possível atualizar o usuário devido a conflito de dados." });
            }
            catch (Exception)
            {
                return Results.Problem(detail: "Erro interno ao atualizar usuário.", statusCode: 500);
            }

            return Results.Ok(new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                deptId = user.DeptId,
                userStatusId = user.UserStatusId,
                profileId = user.ProfileId
            });
        })
        .WithName("UpdateUser")
        .Produces(200)
        .Produces(400)
        .Produces(404)
        .Produces(409)
        .Produces(500)
        .Produces(401);

        app.MapDelete("/users/{id}", async (int id, AppDbContext db) =>
        {
            var user = await db.Users
                .Include(u => u.Department)
                .Include(u => u.UserStatus)
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null) return Results.NotFound(new { Message = "Usuário não encontrado." });

            db.Users.Remove(user);

            try
            {
                await db.SaveChangesAsync();
                var userDto = BuildUserDto(user);
                return Results.Ok(userDto);
            }
            catch (DbUpdateException ex)
            {
                var error = ex.GetBaseException() as Microsoft.Data.SqlClient.SqlException;
                if (error != null && (error.Number == 547 || error.Number == 1451))
                {
                    return Results.Conflict(new { Message = "Não é possível excluir o usuário porque ele está associado a outros registros." });
                }
                return Results.Problem(detail: ex.GetBaseException()?.Message ?? ex.Message, statusCode: 500);
            }
        })
        .WithName("DeleteUser")
        .Produces(200)
        .Produces(404)
        .Produces(409)
        .Produces(500)
        .Produces(401);

        app.MapPost("/login", async (LoginRequest request, AppDbContext db) =>
        {
            var email = request.Email?.Trim();
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(request.Password))
            {
                return Results.BadRequest(new { Message = "Dados de login inválidos. Forneça 'email' e 'password'." });
            }

            var normalized = email.ToLowerInvariant();

            var user = await db.Users
                .Include(u => u.Department)
                .Include(u => u.UserStatus)
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalized);

            if (user == null || string.IsNullOrEmpty(user.Password) || !PasswordService.VerifyPassword(request.Password, user.Password))
            {
                return Results.Unauthorized();
            }

            var userDto = BuildUserDto(user);
            return Results.Ok(userDto);
        })
        .WithName("Login")
        .Produces(200)
        .Produces(400)
        .Produces(401)
        .Produces(500);
    }
}
