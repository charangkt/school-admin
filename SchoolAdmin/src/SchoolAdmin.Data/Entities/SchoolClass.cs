using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolAdmin.Data.Entities;

/// <summary>A class and section, e.g. "Class 5" / "A".</summary>
public class SchoolClass
{
    public int Id { get; set; }

    [MaxLength(30)]
    public required string Name { get; set; }

    [MaxLength(10)]
    public string Section { get; set; } = "A";

    /// <summary>Sort position, so "Class 2" comes before "Class 10".</summary>
    public int DisplayOrder { get; set; }

    public ICollection<Student> Students { get; set; } = [];

    [NotMapped]
    public string DisplayName => $"{Name} - {Section}";
}
