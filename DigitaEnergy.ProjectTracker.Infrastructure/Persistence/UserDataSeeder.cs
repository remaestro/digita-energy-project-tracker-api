using DigitaEnergy.ProjectTracker.Domain.Entities;
using DigitaEnergy.ProjectTracker.Domain.Enums;
using DigitaEnergy.ProjectTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DigitaEnergy.ProjectTracker.Infrastructure.Persistence;

public static class UserDataSeeder
{
    public static async System.Threading.Tasks.Task SeedUsersAsync(ProjectTrackerDbContext context)
    {
        // Check if users already exist
        if (await context.Users.AnyAsync())
        {
            return; // Users already seeded
        }

        var now = DateTime.UtcNow;

        var users = new List<User>
        {
            // Project Manager
            new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@digita-energy.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FirstName = "Jean",
                LastName = "Dupont",
                Role = UserRole.PROJECT_MANAGER,
                AssignedWorkstreams = null,
                CreatedAt = now,
                UpdatedAt = now
            },
            // Stream Lead 1 - Génie civil & Liaisons HTA
            new User
            {
                Id = Guid.NewGuid(),
                Email = "manager@digita-energy.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                FirstName = "Marie",
                LastName = "Martin",
                Role = UserRole.STREAM_LEAD,
                AssignedWorkstreams = "GenieCivil,LiaisonsHTA",
                CreatedAt = now,
                UpdatedAt = now
            },
            // Stream Lead 2 - Postes Sources & Postes de Livraison
            new User
            {
                Id = Guid.NewGuid(),
                Email = "manager2@digita-energy.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                FirstName = "Sophie",
                LastName = "Lefebvre",
                Role = UserRole.STREAM_LEAD,
                AssignedWorkstreams = "PostesSources,PostesDeLivraison",
                CreatedAt = now,
                UpdatedAt = now
            },
            // Team Member
            new User
            {
                Id = Guid.NewGuid(),
                Email = "user@digita-energy.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                FirstName = "Pierre",
                LastName = "Durand",
                Role = UserRole.TEAM_MEMBER,
                AssignedWorkstreams = null,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Users seeded successfully:");
        Console.WriteLine("   - admin@digita-energy.com (PROJECT_MANAGER) - Password: admin123");
        Console.WriteLine("   - manager@digita-energy.com (STREAM_LEAD - Génie civil, Liaisons HTA) - Password: manager123");
        Console.WriteLine("   - manager2@digita-energy.com (STREAM_LEAD - Postes Sources, Postes de Livraison) - Password: manager123");
        Console.WriteLine("   - user@digita-energy.com (TEAM_MEMBER) - Password: user123");
    }
}
