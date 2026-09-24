using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolAdmin.Data.Entities;

public class Student
{
    public int Id { get; set; }

    [MaxLength(20)]
    public required string AdmissionNo { get; set; }

    [MaxLength(50)]
    public required string FirstName { get; set; }

    [MaxLength(50)]
    public string LastName { get; set; } = "";

    public DateOnly? DateOfBirth { get; set; }

    [MaxLength(10)]
    public string Gender { get; set; } = "";

    public int SchoolClassId { get; set; }
    public SchoolClass? SchoolClass { get; set; }

    public int? RollNo { get; set; }

    [MaxLength(100)]
    public string GuardianName { get; set; } = "";

    [MaxLength(15)]
    public string GuardianPhone { get; set; } = "";

    [MaxLength(250)]
    public string Address { get; set; } = "";

    public DateOnly AdmissionDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public bool IsActive { get; set; } = true;

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}".Trim();
}
