using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using SchoolAdmin.App.Services;
using SchoolAdmin.Data;
using SchoolAdmin.Data.Entities;

namespace SchoolAdmin.App.ViewModels;

public partial class StudentsViewModel : PageViewModel
{
    private readonly IDbContextFactory<SchoolDbContext> _dbFactory;

    public StudentsViewModel(IDbContextFactory<SchoolDbContext> dbFactory, SessionService session)
    {
        _dbFactory = dbFactory;
        CanEdit = session.IsInRole(Role.Admin);
        _ = InitAsync();
    }

    /// <summary>Only admins can add or change students; other roles get a read-only list.</summary>
    public bool CanEdit { get; }
    public bool IsReadOnly => !CanEdit;

    public string[] Genders { get; } = ["Male", "Female", "Other"];
    public ObservableCollection<Student> Students { get; } = [];
    public ObservableCollection<SchoolClass> Classes { get; } = [];
    public ObservableCollection<Option<int?>> ClassFilters { get; } = [];

    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private Option<int?>? _classFilter;
    [ObservableProperty] private Student? _selectedStudent;

    // ---- Form fields ----
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FormTitle))]
    private int _editId;
    [ObservableProperty] private string _admissionNo = "";
    [ObservableProperty] private string _firstName = "";
    [ObservableProperty] private string _lastName = "";
    [ObservableProperty] private DateTime? _dateOfBirth;
    [ObservableProperty] private string _gender = "";
    [ObservableProperty] private int? _formClassId;
    [ObservableProperty] private string _rollNo = "";
    [ObservableProperty] private string _guardianName = "";
    [ObservableProperty] private string _guardianPhone = "";
    [ObservableProperty] private string _address = "";
    [ObservableProperty] private DateTime? _admissionDate = DateTime.Today;
    [ObservableProperty] private bool _isActive = true;

    public string FormTitle => EditId == 0 ? "New student" : "Edit student";

    partial void OnSearchTextChanged(string value) => _ = LoadAsync();
    partial void OnClassFilterChanged(Option<int?>? value) => _ = LoadAsync();
    partial void OnSelectedStudentChanged(Student? value)
    {
        if (value is not null) FillForm(value);
    }

    private async Task InitAsync()
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var classes = await db.Classes.AsNoTracking()
                .OrderBy(c => c.DisplayOrder).ThenBy(c => c.Section).ToListAsync();

            ClassFilters.Add(new(null, "All classes"));
            foreach (var c in classes)
            {
                Classes.Add(c);
                ClassFilters.Add(new(c.Id, c.DisplayName));
            }

            if (classes.Count == 0)
            {
                ShowError("No classes yet. Add classes under 'Classes & Subjects' first.");
            }

            ClassFilter = ClassFilters[0]; // triggers LoadAsync
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private async Task LoadAsync()
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var query = db.Students.AsNoTracking().Include(s => s.SchoolClass).AsQueryable();

            if (ClassFilter?.Value is int classId)
            {
                query = query.Where(s => s.SchoolClassId == classId);
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var term = SearchText.Trim();
                query = query.Where(s => s.AdmissionNo.Contains(term) || s.FirstName.Contains(term)
                    || s.LastName.Contains(term) || s.GuardianPhone.Contains(term));
            }

            var students = await query
                .OrderBy(s => s.SchoolClass!.DisplayOrder).ThenBy(s => s.SchoolClass!.Section)
                .ThenBy(s => s.RollNo).ThenBy(s => s.FirstName)
                .ToListAsync();

            Students.Clear();
            foreach (var s in students) Students.Add(s);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void FillForm(Student s)
    {
        EditId = s.Id;
        AdmissionNo = s.AdmissionNo;
        FirstName = s.FirstName;
        LastName = s.LastName;
        DateOfBirth = s.DateOfBirth.ToDateTime();
        Gender = s.Gender;
        FormClassId = s.SchoolClassId;
        RollNo = s.RollNo?.ToString() ?? "";
        GuardianName = s.GuardianName;
        GuardianPhone = s.GuardianPhone;
        Address = s.Address;
        AdmissionDate = s.AdmissionDate.ToDateTime();
        IsActive = s.IsActive;
        ClearMessages();
    }

    [RelayCommand]
    private void New()
    {
        SelectedStudent = null;
        EditId = 0;
        AdmissionNo = FirstName = LastName = Gender = RollNo = GuardianName = GuardianPhone = Address = "";
        DateOfBirth = null;
        FormClassId = ClassFilter?.Value;
        AdmissionDate = DateTime.Today;
        IsActive = true;
        ClearMessages();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!CanEdit) return;

        if (string.IsNullOrWhiteSpace(AdmissionNo) || string.IsNullOrWhiteSpace(FirstName) || FormClassId is null)
        {
            ShowError("Admission no, first name and class are required.");
            return;
        }

        int? rollNo = null;
        if (!string.IsNullOrWhiteSpace(RollNo))
        {
            if (!int.TryParse(RollNo, out var parsed))
            {
                ShowError("Roll no must be a number.");
                return;
            }
            rollNo = parsed;
        }

        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            Student student;
            if (EditId == 0)
            {
                student = new Student { AdmissionNo = "", FirstName = "" };
                db.Students.Add(student);
            }
            else
            {
                student = await db.Students.FindAsync(EditId)
                    ?? throw new InvalidOperationException("This student no longer exists.");
            }

            student.AdmissionNo = AdmissionNo.Trim();
            student.FirstName = FirstName.Trim();
            student.LastName = LastName.Trim();
            student.DateOfBirth = DateOfBirth.ToDateOnly();
            student.Gender = Gender;
            student.SchoolClassId = FormClassId.Value;
            student.RollNo = rollNo;
            student.GuardianName = GuardianName.Trim();
            student.GuardianPhone = GuardianPhone.Trim();
            student.Address = Address.Trim();
            student.AdmissionDate = AdmissionDate.ToDateOnly() ?? DateOnly.FromDateTime(DateTime.Today);
            student.IsActive = IsActive;

            await db.SaveChangesAsync();
            EditId = student.Id;
            await LoadAsync();
            ShowInfo($"Saved {student.FullName}.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (!CanEdit || EditId == 0) return;
        if (!Dialogs.Confirm($"Delete student {FirstName} {LastName}? This cannot be undone.")) return;

        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            await db.Students.Where(s => s.Id == EditId).ExecuteDeleteAsync();
            New();
            await LoadAsync();
            ShowInfo("Student deleted.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }
}
