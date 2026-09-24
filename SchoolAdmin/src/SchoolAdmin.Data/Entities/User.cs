using System.ComponentModel.DataAnnotations;

namespace SchoolAdmin.Data.Entities;

/// <summary>An application login (admin, accountant, exam staff).</summary>
public class User
{
    public int Id { get; set; }

    [MaxLength(50)]
    public required string Username { get; set; }

    /// <summary>BCrypt hash — the plain password is never stored.</summary>
    [MaxLength(100)]
    public required string PasswordHash { get; set; }

    [MaxLength(100)]
    public required string FullName { get; set; }

    public Role Role { get; set; }

    /// <summary>The staff record behind a Teacher login.</summary>
    public int? StaffId { get; set; }
    public Staff? Staff { get; set; }

    /// <summary>The student record behind a Student login.</summary>
    public int? StudentId { get; set; }
    public Student? Student { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
