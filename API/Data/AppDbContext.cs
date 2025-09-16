using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<UserStatus> UserStatus { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Department> Departments { get; set; }
}
