using API.Data;
using API.Models;
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
                    UserProfile = u.UserProfile.Name,
                    UserStatus = u.UserStatus.Name,
                    Department = u.Department.Name
                })
                .ToListAsync());
        
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
                    UserProfile = u.UserProfile.Name,
                    UserStatus = u.UserStatus.Name,
                    Department = u.Department.Name
                })
                .FirstOrDefaultAsync(u => u.Id == id);
            return user is not null ? Results.Ok(user) : Results.NotFound();
        });
    }
}