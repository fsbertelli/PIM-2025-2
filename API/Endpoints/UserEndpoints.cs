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
        // Rota para criar um novo usuário
        app.MapPost("/users", async (CreateUserDto dto, AppDbContext db) =>
        {
            var department = await db.Departments.FindAsync(dto.DeptId);
            var userStatus = await db.UserStatus.FindAsync(dto.UserStatusId);
            var userProfile = await db.UserProfiles.FindAsync(dto.ProfileId);
            
            if (department is null || userStatus is null  || userProfile is null)
            {
                return Results.BadRequest("Departamento, Status ou Perfil inválido.");
            }

            if (string.IsNullOrEmpty(dto.Password))
            {
                return Results.BadRequest("Senha é obrigatória.");
            }
            
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                // Armazena o hash da senha
                Password = PasswordService.HashPassword(dto.Password),
                DeptId = dto.DeptId,
                Department = department,
                UserStatusId = dto.UserStatusId,
                UserStatus = userStatus,
                ProfileId = dto.ProfileId,
                UserProfile = userProfile
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Department = new DepartmentDto { Id = department.Id, Name = department.Name },
                UserStatus = new UserStatusDto { Id = userStatus.Id, Name = userStatus.Name },
                UserProfile = new UserProfileDto { Id = userProfile.Id, Name = userProfile.Name }
            };

            return Results.Created($"/users/{user.Id}", userDto);
        })
        .WithName("CreateUser")
        .Produces(201)
        .Produces(401);

        // Rota para atualizar um usuário existente
        app.MapPut("/users/{id}", async (int id, CreateUserDto dto, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound("Usuário não encontrado.");

            var department = await db.Departments.FindAsync(dto.DeptId);
            var userStatus = await db.UserStatus.FindAsync(dto.UserStatusId);
            var userProfile = await db.UserProfiles.FindAsync(dto.ProfileId);

            // Log
            Console.WriteLine($"PUT /users/{{id}} => id: {id}, name: {dto.Name}, deptId: {dto.DeptId}, userStatusId: {dto.UserStatusId}, profileId: {dto.ProfileId}");
            Console.WriteLine($"Departamentos encontrados: {(department != null)}, Status encontrados: {(userStatus != null)}, Perfil encontrado: {(userProfile != null)}");

            if (department is null || userStatus is null || userProfile is null)
                return Results.BadRequest("Departamento, Status ou Perfil inválido.");

            user.Name = dto.Name;
            user.Email = dto.Email;
            // Atualiza a senha somente se um novo valor foi enviado (presume que dto.Password contém a senha em texto plano)
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.Password = PasswordService.HashPassword(dto.Password);
            }
            user.DeptId = dto.DeptId;
            user.UserStatusId = dto.UserStatusId;
            user.ProfileId = dto.ProfileId;

            db.Entry(user).Property(u => u.Name).IsModified = true;
            db.Entry(user).Property(u => u.Email).IsModified = true;
            if (!string.IsNullOrEmpty(dto.Password)) db.Entry(user).Property(u => u.Password).IsModified = true;
            db.Entry(user).Property(u => u.DeptId).IsModified = true;
            db.Entry(user).Property(u => u.UserStatusId).IsModified = true;
            db.Entry(user).Property(u => u.ProfileId).IsModified = true;

            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("UpdateUser")
        .Produces(204)
        .Produces(404)
        .Produces(401);

        // Rota para deletar um usuário
        app.MapDelete("/users/{id}", async (int id, AppDbContext db) =>
        {
            // load user including navigation properties to be safe
            var user = await db.Users
                .Include(u => u.Department)
                .Include(u => u.UserStatus)
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user is null) return Results.NotFound("Usuário não encontrado.");

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Department = user.Department is null ? null : new DepartmentDto { Id = user.Department.Id, Name = user.Department.Name },
                UserStatus = user.UserStatus is null ? null : new UserStatusDto { Id = user.UserStatus.Id, Name = user.UserStatus.Name },
                UserProfile = user.UserProfile is null ? null : new UserProfileDto { Id = user.UserProfile.Id, Name = user.UserProfile.Name }
            };

            return Results.Ok(userDto);
        })
        .WithName("DeleteUser")
        .Produces(200)
        .Produces(404)
        .Produces(401);

        // Rota para listar todos os usuários
        // REMOVIDO: duplicidade com UserDtoEndpoints.cs

        // Rota de login
        app.MapPost("/login", async (LoginRequest request, AppDbContext db) =>
        {
            // validação básica - o binding garante instância, validamos campos importantes
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return Results.BadRequest(new { Message = "Dados de login inválidos. Forneça 'username' e 'password'." });
            }

            // Assumimos que "Username" corresponde ao campo Email do usuário no banco
            var user = await db.Users
                .Include(u => u.Department)
                .Include(u => u.UserStatus)
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Email == request.Username);

            if (user == null || string.IsNullOrEmpty(user.Password) || !PasswordService.VerifyPassword(request.Password, user.Password))
            {
                return Results.Unauthorized();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Department = user.Department != null ? new DepartmentDto { Id = user.Department.Id, Name = user.Department.Name } : null,
                UserStatus = user.UserStatus != null ? new UserStatusDto { Id = user.UserStatus.Id, Name = user.UserStatus.Name } : null,
                UserProfile = user.UserProfile != null ? new UserProfileDto { Id = user.UserProfile.Id, Name = user.UserProfile.Name } : null
            };
            return Results.Ok(userDto);
        })
        .WithName("Login")
        .Produces(200)
        .Produces(401);
    }
}
