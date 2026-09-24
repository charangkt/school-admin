namespace SchoolAdmin.Data.Entities;

public static class RoleNames
{
    /// <summary>Friendly text for a role, e.g. "Exam Staff".</summary>
    public static string Label(this Role role) => role switch
    {
        Role.ExamStaff => "Exam Staff",
        _ => role.ToString(),
    };
}
