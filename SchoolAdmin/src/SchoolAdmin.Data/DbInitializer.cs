using Microsoft.EntityFrameworkCore;
using SchoolAdmin.Data.Entities;
using SchoolAdmin.Data.Services;

namespace SchoolAdmin.Data;

public static class DbInitializer
{
    public const string DefaultAdminUsername = "admin";
    public const string DefaultAdminPassword = "Admin@123";

    /// <summary>
    /// Creates or upgrades the database to the latest schema and adds the
    /// first admin account if there are no users yet.
    /// </summary>
    public static async Task InitializeAsync(SchoolDbContext db)
    {
        await db.Database.MigrateAsync();

        if (!await db.Users.AnyAsync())
        {
            db.Users.Add(new User
            {
                Username = DefaultAdminUsername,
                PasswordHash = PasswordHasher.Hash(DefaultAdminPassword),
                FullName = "Administrator",
                Role = Role.Admin,
            });
            await db.SaveChangesAsync();
        }
    }
}
