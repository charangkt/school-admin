using Microsoft.EntityFrameworkCore;
using SchoolAdmin.Data.Entities;

namespace SchoolAdmin.Data.Services;

public class AuthService(IDbContextFactory<SchoolDbContext> dbFactory)
{
    /// <summary>Returns the user if the credentials are valid and the account is active; otherwise null.</summary>
    public async Task<User?> LoginAsync(string username, string password)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var user = await db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Username == username.Trim() && u.IsActive);

        return user is not null && PasswordHasher.Verify(password, user.PasswordHash) ? user : null;
    }

    /// <summary>Changes a user's own password. Returns false if the current password is wrong.</summary>
    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var user = await db.Users.FindAsync(userId);
        if (user is null || !PasswordHasher.Verify(currentPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = PasswordHasher.Hash(newPassword);
        await db.SaveChangesAsync();
        return true;
    }
}
