using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace SchoolAdmin.App.Services;

/// <summary>Turns database exceptions into messages office staff can understand.</summary>
public static class DbErrors
{
    public static string Describe(Exception ex)
    {
        if (ex is DbUpdateException { InnerException: SqlException sql })
        {
            return sql.Number switch
            {
                2601 or 2627 => "A record with the same value already exists (e.g. duplicate number or name).",
                547 => "This record is in use by other records and cannot be deleted. Mark it inactive instead.",
                _ => $"Database error: {sql.Message}",
            };
        }

        if (ex is SqlException)
        {
            return "Cannot connect to the database. Check that the server PC is on and connected to the network, "
                + "and that the connection string in appsettings.json is correct.";
        }

        return ex.Message;
    }
}
