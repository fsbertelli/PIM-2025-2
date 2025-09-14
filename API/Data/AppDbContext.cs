using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }
    public DbSet<StatusUser> StatusUsers { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    
    public DbSet<Dept> Depts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aqui você pode adicionar configurações adicionais para o modelo, se necessário.
        // Ex: modelBuilder.Entity<Cliente>().ToTable("TB_CLIENTES");
        base.OnModelCreating(modelBuilder);
    }
}
