using Microsoft.EntityFrameworkCore;
using SchoolAdmin.Data.Entities;

namespace SchoolAdmin.Data;

public class SchoolDbContext(DbContextOptions<SchoolDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<SchoolClass> Classes => Set<SchoolClass>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<Subject> Subjects => Set<Subject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
            e.HasOne(u => u.Staff).WithMany().HasForeignKey(u => u.StaffId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(u => u.Student).WithMany().HasForeignKey(u => u.StudentId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SchoolClass>(e =>
        {
            e.ToTable("Classes");
            e.HasIndex(c => new { c.Name, c.Section }).IsUnique();
        });

        modelBuilder.Entity<Student>(e =>
        {
            e.HasIndex(s => s.AdmissionNo).IsUnique();
            e.HasOne(s => s.SchoolClass)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.SchoolClassId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Staff>(e =>
        {
            e.ToTable("Staff");
            e.HasIndex(s => s.EmployeeCode).IsUnique();
        });

        modelBuilder.Entity<Subject>(e =>
        {
            e.HasIndex(s => s.Name).IsUnique();
        });
    }
}
