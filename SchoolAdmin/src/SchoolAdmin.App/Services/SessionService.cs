using SchoolAdmin.Data.Entities;

namespace SchoolAdmin.App.Services;

/// <summary>Holds the user who is currently logged in.</summary>
public class SessionService
{
    public User? CurrentUser { get; set; }

    public bool IsInRole(params Role[] roles) =>
        CurrentUser is not null && roles.Contains(CurrentUser.Role);
}
