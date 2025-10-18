using API.Data;
using API.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class UserDtoEndpoints
{
    public static void MapUserDtoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (AppDbContext db) => 
            await db.Users
                .Include(u => u.Department)
                .Include(u => u.UserStatus)
                .Include(u => u.UserProfile)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    UserProfile = u.UserProfile == null ? null : new UserProfileDto { Id = u.UserProfile.Id, Name = u.UserProfile.Name },
                    UserStatus = u.UserStatus == null ? null : new UserStatusDto { Id = u.UserStatus.Id, Name = u.UserStatus.Name },
                    Department = u.Department == null ? null : new DepartmentDto { Id = u.Department.Id, Name = u.Department.Name }
                })
                .ToListAsync())
        .WithName("GetUsers")
        .Produces(200)
        .Produces(400)
        .Produces(401);
        
        // Rota para obter um usuário por ID
        app.MapGet("/users/{id}", async (int id, AppDbContext db) =>
        {
            var user = await db.Users
                .Include(u => u.Department)
                .Include(u => u.UserStatus)
                .Include(u => u.UserProfile)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    UserProfile = u.UserProfile == null ? null : new UserProfileDto { Id = u.UserProfile.Id, Name = u.UserProfile.Name },
                    UserStatus = u.UserStatus == null ? null : new UserStatusDto { Id = u.UserStatus.Id, Name = u.UserStatus.Name },
                    Department = u.Department == null ? null : new DepartmentDto { Id = u.Department.Id, Name = u.Department.Name }
                })
                .FirstOrDefaultAsync(u => u.Id == id);
            return user is not null ? Results.Ok(user) : Results.NotFound();
        })
        .WithName("GetUserById")
        .Produces(200)
        .Produces(404)
        .Produces(401);
    }
}