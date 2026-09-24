using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace SchoolAdmin.Data.Services;

/// <summary>Turns database exceptions into messages office staff can understand.</summary>
public static class DbErrors
{
    private const string Duplicate = "A record with the same value already exists (e.g. duplicate number or name).";
    private const string InUse = "This record is in use by other records and cannot be deleted. Mark it inactive instead.";

    public static string Describe(Exception ex)
    {
        switch (ex)
        {
            case DbUpdateException { InnerException: SqlException sql }:
                return sql.Number switch
                {
                    2601 or 2627 => Duplicate,
                    547 => InUse,
                    _ => $"Database error: {sql.Message}",
                };

            case DbUpdateException { InnerException: SqliteException lite }:
                return lite.SqliteExtendedErrorCode switch
                {
                    2067 or 1555 => Duplicate, // UNIQUE / PRIMARY KEY constraint
                    787 => InUse,              // FOREIGN KEY constraint
                    _ => $"Database error: {lite.Message}",
                };

            case SqlException:
                return "Cannot connect to the database. Check that the server PC is on and connected to the network.";

            default:
                return ex.Message;
        }
    }
}
