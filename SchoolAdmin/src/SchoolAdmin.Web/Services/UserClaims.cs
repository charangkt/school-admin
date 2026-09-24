using System.Security.Claims;
using SchoolAdmin.Data.Entities;

namespace SchoolAdmin.Web.Services;

/// <summary>Reads the logged-in user's details from the login cookie.</summary>
public static class UserClaims
{
    public static int UserId(this ClaimsPrincipal user) =>
        int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    public static string FullName(this ClaimsPrincipal user) =>
        user.FindFirstValue("FullName") ?? user.Identity?.Name ?? "";

    public static string RoleLabel(this ClaimsPrincipal user) =>
        Enum.TryParse<Role>(user.FindFirstValue(ClaimTypes.Role), out var role) ? role.Label() : "";

    public static int? StudentId(this ClaimsPrincipal user) =>
        int.TryParse(user.FindFirstValue("StudentId"), out var id) ? id : null;
}
