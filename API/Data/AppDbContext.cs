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
    public DbSet<StatusTicket> StatusTickets { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketTransaction> TicketTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Prevent cascade delete cycles / multiple cascade paths on SQL Server
        // Make deletes restrict when referencing Department to avoid multiple cascade paths.
        modelBuilder.Entity<User>()
            .HasOne(u => u.Department)
            .WithMany()
            .HasForeignKey(u => u.DeptId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>()
            .HasOne(c => c.Department)
            .WithMany()
            .HasForeignKey(c => c.DeptId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Department)
            .WithMany()
            .HasForeignKey(t => t.DeptTargetId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Ticket relationships explicitly (avoid mistaken self-reference)
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Category)
            .WithMany()
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.StatusTicket)
            .WithMany()
            .HasForeignKey(t => t.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure TicketTransaction relationships to avoid cascading delete paths
        modelBuilder.Entity<TicketTransaction>()
            .HasOne(tt => tt.Ticket)
            .WithMany()
            .HasForeignKey(tt => tt.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TicketTransaction>()
            .HasOne(tt => tt.UserSource)
            .WithMany()
            .HasForeignKey(tt => tt.UserSourceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TicketTransaction>()
            .HasOne(tt => tt.UserTarget)
            .WithMany()
            .HasForeignKey(tt => tt.UserTargetId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure other relationships to restrict delete to be safe
        modelBuilder.Entity<User>()
            .HasOne(u => u.UserProfile)
            .WithMany()
            .HasForeignKey(u => u.ProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.UserStatus)
            .WithMany()
            .HasForeignKey(u => u.UserStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
