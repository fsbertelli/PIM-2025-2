using API.Data;
using API.Models;
using API.Models.DTO;
using Microsoft.EntityFrameworkCore;

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
            
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Pwd = dto.Pwd,
                DeptId = dto.DeptId,
                Department = department,
                UserStatusId = dto.UserStatusId,
                UserStatus = userStatus,
                ProfileId = dto.ProfileId,
                UserProfile = userProfile
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Created($"/users/{user.Id}", user);
        });

        // Rota para atualizar um usuário existente
        app.MapPut("/users/{id}", async (int id, CreateUserDto dto, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound("Usuário não encontrado.");

            var department = await db.Departments.FindAsync(dto.DeptId);
            var userStatus = await db.UserStatus.FindAsync(dto.UserStatusId);
            var userProfile = await db.UserProfiles.FindAsync(dto.ProfileId);

            // Log para diagnóstico
            Console.WriteLine($"PUT /users/{{id}} => id: {id}, name: {dto.Name}, deptId: {dto.DeptId}, userStatusId: {dto.UserStatusId}, profileId: {dto.ProfileId}");
            Console.WriteLine($"Departamentos encontrados: {(department != null)}, Status encontrados: {(userStatus != null)}, Perfil encontrado: {(userProfile != null)}");

            if (department is null || userStatus is null || userProfile is null)
                return Results.BadRequest("Departamento, Status ou Perfil inválido.");

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.Pwd = dto.Pwd;
            user.DeptId = dto.DeptId;
            user.UserStatusId = dto.UserStatusId;
            user.ProfileId = dto.ProfileId;

            db.Entry(user).Property(u => u.Name).IsModified = true;
            db.Entry(user).Property(u => u.Email).IsModified = true;
            db.Entry(user).Property(u => u.Pwd).IsModified = true;
            db.Entry(user).Property(u => u.DeptId).IsModified = true;
            db.Entry(user).Property(u => u.UserStatusId).IsModified = true;
            db.Entry(user).Property(u => u.ProfileId).IsModified = true;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // Rota para deletar um usuário
        app.MapDelete("/users/{id}", async (int id, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);
            if (user is null) return Results.NotFound("Usuário não encontrado.");

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Results.Ok(user);
        });

        // Rota para listar todos os usuários
        // REMOVIDO: duplicidade com UserDtoEndpoints.cs
    }
}
