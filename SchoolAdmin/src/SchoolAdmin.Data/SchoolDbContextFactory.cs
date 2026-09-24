using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SchoolAdmin.Data;

/// <summary>
/// Used only by the `dotnet ef` tool when creating migrations.
/// Override the database with the SCHOOLADMIN_DB environment variable.
/// </summary>
public class SchoolDbContextFactory : IDesignTimeDbContextFactory<SchoolDbContext>
{
    public SchoolDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("SCHOOLADMIN_DB")
            ?? @"Server=(localdb)\MSSQLLocalDB;Database=SchoolAdmin;Trusted_Connection=True;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<SchoolDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new SchoolDbContext(options);
    }
}
