using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();
    public DbSet<Interview> Interviews => Set<Interview>();
    public DbSet<StatusHistory> StatusHistories => Set<StatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            e.Property(u => u.Email).IsRequired().HasMaxLength(200);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<JobApplication>(e =>
        {
            e.HasKey(j => j.Id);
            e.Property(j => j.CompanyName).IsRequired().HasMaxLength(150);
            e.Property(j => j.RoleTitle).IsRequired().HasMaxLength(150);
            e.Property(j => j.JobUrl).HasMaxLength(500);
            e.Property(j => j.Location).HasMaxLength(150);
            e.Property(j => j.Salary).HasPrecision(18, 2);
            e.Property(j => j.Status).HasConversion<string>().HasMaxLength(20);

            e.HasOne(j => j.User)
             .WithMany(u => u.JobApplications)
             .HasForeignKey(j => j.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(j => new { j.UserId, j.Status });
        });

        modelBuilder.Entity<Interview>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.Mode).HasConversion<string>().HasMaxLength(20);
            e.HasOne(i => i.JobApplication)
             .WithMany(j => j.Interviews)
             .HasForeignKey(i => i.JobApplicationId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<StatusHistory>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.OldStatus).HasConversion<string>().HasMaxLength(20);
            e.Property(s => s.NewStatus).HasConversion<string>().HasMaxLength(20);
            e.HasOne(s => s.JobApplication)
             .WithMany(j => j.StatusHistories)
             .HasForeignKey(s => s.JobApplicationId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}