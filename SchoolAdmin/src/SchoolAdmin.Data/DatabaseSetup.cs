using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SchoolAdmin.Data;

public static class DatabaseSetup
{
    /// <summary>
    /// Registers the database. Provider "SqlServer" is used at schools;
    /// "Sqlite" is used for the online demo (a single file, no server needed).
    /// </summary>
    public static IServiceCollection AddSchoolDb(this IServiceCollection services, string provider, string connectionString)
    {
        services.AddDbContextFactory<SchoolDbContext>(options =>
        {
            if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
            {
                options.UseSqlite(connectionString);
            }
            else
            {
                options.UseSqlServer(connectionString);
            }
        });
        return services;
    }
}
