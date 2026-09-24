using System.ComponentModel.DataAnnotations;

namespace SchoolAdmin.Data.Entities;

/// <summary>A teaching or non-teaching staff member.</summary>
public class Staff
{
    public int Id { get; set; }

    [MaxLength(20)]
    public required string EmployeeCode { get; set; }

    [MaxLength(100)]
    public required string FullName { get; set; }

    [MaxLength(50)]
    public string Designation { get; set; } = "";

    [MaxLength(15)]
    public string Phone { get; set; } = "";

    [MaxLength(100)]
    public string Email { get; set; } = "";

    public DateOnly JoiningDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public bool IsActive { get; set; } = true;
}
