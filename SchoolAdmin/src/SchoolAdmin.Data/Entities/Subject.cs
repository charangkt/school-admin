using System.ComponentModel.DataAnnotations;

namespace SchoolAdmin.Data.Entities;

public class Subject
{
    public int Id { get; set; }

    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(10)]
    public string Code { get; set; } = "";

    /// <summary>Order in which subjects appear on marks sheets and report cards.</summary>
    public int DisplayOrder { get; set; }
}
