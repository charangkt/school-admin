namespace SchoolAdmin.Data.Entities;

/// <summary>What a logged-in user is allowed to see and do.</summary>
public enum Role
{
    Admin = 1,
    Accountant = 2,
    ExamStaff = 3,

    /// <summary>Linked to a <see cref="Staff"/> record.</summary>
    Teacher = 4,

    /// <summary>Linked to a <see cref="Student"/> record; sees only their own details.</summary>
    Student = 5,
}
