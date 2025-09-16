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
        app.MapPost("/users", async (User user, AppDbContext db) =>
        {
            var department = await db.Departments.FindAsync(user.DeptId);
            var userStatus = await db.UserStatus.FindAsync(user.UserStatusId);
            var userProfile = await db.UserProfiles.FindAsync(user.ProfileId);
            
            if (department is null || userStatus is null  || userProfile is null)
            {
                return Results.BadRequest("Departamento, Status ou Perfil inválido.");
            }
            
            user.Department = department;
            user.UserStatus = userStatus;
            user.UserProfile = userProfile;
            db.Users.Add(user);
            
            await db.SaveChangesAsync();
            return Results.Created($"/users/{user.Id}", user);
        });

        // Rota para atualizar um usuário existente
        app.MapPut("/users/{id}", async (int id, User inputUser, AppDbContext db) =>
        {
            var user = await db.Users.FindAsync(id);

            if (user is null) return Results.NotFound("Usuário não encontrado.");
            
            var department = await db.Departments.FindAsync(inputUser.DeptId);
            var userStatus = await db.UserStatus.FindAsync(inputUser.UserStatusId);
            var userProfile = await db.UserProfiles.FindAsync(inputUser.ProfileId);

            if (department is null || userStatus is null || userProfile is null){
                return Results.BadRequest("Departamento, Status ou Perfil inválido.");
            }
            
            user.Name = inputUser.Name;
            user.Email = inputUser.Email;
            user.Pwd = inputUser.Pwd;
            user.UserProfile = inputUser.UserProfile;
            user.UserStatus = inputUser.UserStatus;
            user.Department = inputUser.Department;
            
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
    }
}
