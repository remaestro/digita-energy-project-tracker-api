using DigitaEnergy.ProjectTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitaEnergy.ProjectTracker.Infrastructure.Persistence;

public class ProjectTrackerDbContext : DbContext
{
    public ProjectTrackerDbContext(DbContextOptions<ProjectTrackerDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserInvitation> UserInvitations { get; set; }
    public DbSet<Domain.Entities.Task> Tasks { get; set; }
    public DbSet<Milestone> Milestones { get; set; }
    public DbSet<Risk> Risks { get; set; }
    public DbSet<ProjectSettings> ProjectSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Add any entity configurations here
    }
}
