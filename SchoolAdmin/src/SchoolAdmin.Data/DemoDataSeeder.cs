using Microsoft.EntityFrameworkCore;
using SchoolAdmin.Data.Entities;
using SchoolAdmin.Data.Services;

namespace SchoolAdmin.Data;

/// <summary>
/// Fills an empty database with sample data for demonstrations
/// (same data as scripts/seed-demo.sql). Never enable on a real school database.
/// </summary>
public static class DemoDataSeeder
{
    /// <summary>Demo logins, shown on the login page when demo mode is on.</summary>
    public static readonly (string Username, string Password, Role Role)[] DemoLogins =
    [
        (DbInitializer.DefaultAdminUsername, DbInitializer.DefaultAdminPassword, Role.Admin),
        ("accountant", "Accountant@123", Role.Accountant),
        ("examstaff", "Exam@123", Role.ExamStaff),
        ("teacher", "Teacher@123", Role.Teacher),
        ("student", "Student@123", Role.Student),
    ];

    private static readonly string[] Boys =
    [
        "Arjun", "Karthik", "Vignesh", "Harish", "Surya", "Pranav", "Rahul", "Dinesh", "Gokul", "Naveen",
        "Aakash", "Bharath", "Sanjay", "Varun", "Hari", "Ashwin", "Manoj", "Vishal", "Kishore", "Adithya",
        "Rohit", "Siddharth", "Tarun", "Yuvan", "Nithin", "Mukesh", "Ajay", "Santhosh", "Lokesh", "Praveen",
    ];

    private static readonly string[] Girls =
    [
        "Divya", "Keerthana", "Swathi", "Harini", "Nandhini", "Aishwarya", "Priyanka", "Sneha", "Janani", "Pooja",
        "Anjali", "Kavya", "Monisha", "Varsha", "Deepika", "Shalini", "Ramya", "Sangeetha", "Bhavya", "Lavanya",
        "Madhu", "Nivetha", "Oviya", "Pavithra", "Revathi", "Sowmya", "Thenmozhi", "Vaishnavi", "Yamini", "Abinaya",
    ];

    private static readonly string[] Fathers =
    [
        "Ramesh", "Kumar", "Selvam", "Murugan", "Srinivasan", "Venkatesan", "Rajendran", "Balasubramanian",
        "Chandrasekar", "Ganesan", "Palani", "Saravanan", "Sivakumar", "Mohan", "Anand", "Elango",
        "Jayaraman", "Natarajan", "Prabhu", "Thirumalai",
    ];

    private static readonly string[] Areas =
        ["Anna Nagar", "T. Nagar", "Velachery", "Tambaram", "Adyar", "Mylapore", "Porur", "Chromepet", "Guindy", "Perambur"];

    private static readonly string[] Streets =
        ["2nd Cross Street", "Gandhi Street", "Kamarajar Salai", "Main Road", "Nehru Street"];

    public static async Task SeedAsync(SchoolDbContext db)
    {
        if (await db.Students.AnyAsync()) return; // already has data

        // Classes: LKG, UKG, Class 1-12
        string[] classNames = ["LKG", "UKG", .. Enumerable.Range(1, 12).Select(n => $"Class {n}")];
        var classes = classNames
            .Select((name, i) => new SchoolClass { Name = name, Section = "A", DisplayOrder = i + 1 })
            .ToList();
        db.Classes.AddRange(classes);

        db.Subjects.AddRange(
            new Subject { Name = "Tamil", Code = "TAM", DisplayOrder = 1 },
            new Subject { Name = "English", Code = "ENG", DisplayOrder = 2 },
            new Subject { Name = "Mathematics", Code = "MAT", DisplayOrder = 3 },
            new Subject { Name = "Science", Code = "SCI", DisplayOrder = 4 },
            new Subject { Name = "Social Science", Code = "SOC", DisplayOrder = 5 },
            new Subject { Name = "Computer Science", Code = "CS", DisplayOrder = 6 });

        var staff = new[]
        {
            NewStaff("EMP001", "Lakshmi Narayanan", "Principal", "2012-06-01"),
            NewStaff("EMP002", "Meena Sundaram", "Tamil Teacher", "2015-06-03"),
            NewStaff("EMP003", "Rajesh Kumar", "English Teacher", "2016-06-01"),
            NewStaff("EMP004", "Priya Venkatesh", "Mathematics Teacher", "2017-06-05"),
            NewStaff("EMP005", "Suresh Babu", "Science Teacher", "2014-06-02"),
            NewStaff("EMP006", "Kavitha Ramesh", "Social Science Teacher", "2018-06-04"),
            NewStaff("EMP007", "Arun Prakash", "Computer Teacher", "2019-06-03"),
            NewStaff("EMP008", "Deepa Krishnan", "Primary Teacher", "2020-06-01"),
            NewStaff("EMP009", "Senthil Murugan", "Physical Education", "2016-06-06"),
            NewStaff("EMP010", "Revathi Balaji", "Primary Teacher", "2021-06-02"),
            NewStaff("EMP011", "Ganesh Raman", "Accountant", "2013-04-01"),
            NewStaff("EMP012", "Saranya Mohan", "Office Clerk", "2022-04-04"),
        };
        db.Staff.AddRange(staff);

        // 60 students: 6 per class across Class 1-10, 3 boys + 3 girls each
        var students = new List<Student>();
        for (var k = 1; k <= 60; k++)
        {
            var classNo = (k - 1) % 10 + 1;
            var round = (k - 1) / 10;
            var isBoy = round % 2 == 0;
            var nameIdx = round / 2 * 10 + (k - 1) % 10;
            var first = isBoy ? Boys[nameIdx] : Girls[nameIdx];
            var father = Fathers[(k * 13 + round) % 20];

            students.Add(new Student
            {
                AdmissionNo = $"ADM2026{k:000}",
                FirstName = first,
                LastName = father,
                DateOfBirth = new DateOnly(2026 - (classNo + 6), 1, 1).AddDays(k * 37 % 365),
                Gender = isBoy ? "Male" : "Female",
                SchoolClass = classes[classNo + 1], // index 2 = "Class 1"
                GuardianName = $"{father} {first[0]}",
                GuardianPhone = $"98{41000000 + k * 1373:00000000}",
                Address = $"{10 + k * 13 % 190}, {Streets[(k + round) % 5]}, {Areas[(k * 7 + round) % 10]}, Chennai",
                AdmissionDate = new DateOnly(2026 - k % 5, 6, 1 + k % 10),
            });
        }

        // Roll numbers: alphabetical within each class
        foreach (var group in students.GroupBy(s => s.SchoolClass))
        {
            var roll = 1;
            foreach (var s in group.OrderBy(s => s.FirstName).ThenBy(s => s.LastName)) s.RollNo = roll++;
        }

        db.Students.AddRange(students);
        await db.SaveChangesAsync();

        // Demo logins (the admin account is created by DbInitializer)
        db.Users.AddRange(
            DemoUser("accountant", "Accountant@123", "Ganesh Raman", Role.Accountant),
            DemoUser("examstaff", "Exam@123", "Priya Venkatesh", Role.ExamStaff),
            DemoUser("teacher", "Teacher@123", "Meena Sundaram", Role.Teacher, staffId: staff[1].Id),
            DemoUser("student", "Student@123", students[4].FullName, Role.Student, studentId: students[4].Id));
        await db.SaveChangesAsync();
    }

    private static Staff NewStaff(string code, string name, string designation, string joined)
    {
        var n = int.Parse(code[3..]);
        return new Staff
        {
            EmployeeCode = code,
            FullName = name,
            Designation = designation,
            Phone = $"98400110{n:00}",
            Email = $"{name.Split(' ')[0].ToLowerInvariant()}@school.local",
            JoiningDate = DateOnly.Parse(joined),
        };
    }

    private static User DemoUser(string username, string password, string fullName, Role role,
        int? staffId = null, int? studentId = null) => new()
    {
        Username = username,
        PasswordHash = PasswordHasher.Hash(password),
        FullName = fullName,
        Role = role,
        StaffId = staffId,
        StudentId = studentId,
    };
}
