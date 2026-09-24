using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using SchoolAdmin.App.Services;
using SchoolAdmin.Data;
using SchoolAdmin.Data.Entities;

namespace SchoolAdmin.App.ViewModels;

/// <summary>"Classes &amp; Subjects" page: two small lists managed side by side.</summary>
public partial class ClassesViewModel : PageViewModel
{
    private readonly IDbContextFactory<SchoolDbContext> _dbFactory;

    public ClassesViewModel(IDbContextFactory<SchoolDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
        _ = LoadAsync();
    }

    public ObservableCollection<SchoolClass> Classes { get; } = [];
    public ObservableCollection<Subject> Subjects { get; } = [];

    // ---- Class form ----
    [ObservableProperty] private SchoolClass? _selectedClass;
    [ObservableProperty] private int _classEditId;
    [ObservableProperty] private string _className = "";
    [ObservableProperty] private string _classSection = "A";
    [ObservableProperty] private string _classOrder = "";

    // ---- Subject form ----
    [ObservableProperty] private Subject? _selectedSubject;
    [ObservableProperty] private int _subjectEditId;
    [ObservableProperty] private string _subjectName = "";
    [ObservableProperty] private string _subjectCode = "";
    [ObservableProperty] private string _subjectOrder = "";

    partial void OnSelectedClassChanged(SchoolClass? value)
    {
        if (value is null) return;
        ClassEditId = value.Id;
        ClassName = value.Name;
        ClassSection = value.Section;
        ClassOrder = value.DisplayOrder.ToString();
        ClearMessages();
    }

    partial void OnSelectedSubjectChanged(Subject? value)
    {
        if (value is null) return;
        SubjectEditId = value.Id;
        SubjectName = value.Name;
        SubjectCode = value.Code;
        SubjectOrder = value.DisplayOrder.ToString();
        ClearMessages();
    }

    private async Task LoadAsync()
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var classes = await db.Classes.AsNoTracking().OrderBy(c => c.DisplayOrder).ThenBy(c => c.Section).ToListAsync();
            var subjects = await db.Subjects.AsNoTracking().OrderBy(s => s.DisplayOrder).ThenBy(s => s.Name).ToListAsync();

            Classes.Clear();
            foreach (var c in classes) Classes.Add(c);
            Subjects.Clear();
            foreach (var s in subjects) Subjects.Add(s);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    // ---------- Classes ----------

    [RelayCommand]
    private void NewClass()
    {
        SelectedClass = null;
        ClassEditId = 0;
        ClassName = "";
        ClassSection = "A";
        ClassOrder = "";
        ClearMessages();
    }

    [RelayCommand]
    private async Task SaveClassAsync()
    {
        if (string.IsNullOrWhiteSpace(ClassName) || string.IsNullOrWhiteSpace(ClassSection))
        {
            ShowError("Class name and section are required.");
            return;
        }

        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            SchoolClass schoolClass;
            if (ClassEditId == 0)
            {
                schoolClass = new SchoolClass { Name = "" };
                db.Classes.Add(schoolClass);
            }
            else
            {
                schoolClass = await db.Classes.FindAsync(ClassEditId)
                    ?? throw new InvalidOperationException("This class no longer exists.");
            }

            schoolClass.Name = ClassName.Trim();
            schoolClass.Section = ClassSection.Trim().ToUpperInvariant();
            schoolClass.DisplayOrder = int.TryParse(ClassOrder, out var order)
                ? order
                : (await db.Classes.MaxAsync(c => (int?)c.DisplayOrder) ?? 0) + 1;

            await db.SaveChangesAsync();
            ShowInfo($"Saved {schoolClass.DisplayName}.");
            NewClassForm();
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    [RelayCommand]
    private async Task DeleteClassAsync()
    {
        if (ClassEditId == 0) return;
        if (!Dialogs.Confirm($"Delete class {ClassName} - {ClassSection}?")) return;

        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            if (await db.Students.AnyAsync(s => s.SchoolClassId == ClassEditId))
            {
                ShowError("This class has students. Move them to another class first.");
                return;
            }

            await db.Classes.Where(c => c.Id == ClassEditId).ExecuteDeleteAsync();
            NewClassForm();
            await LoadAsync();
            ShowInfo("Class deleted.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    /// <summary>Adds LKG, UKG and Class 1–12 (section A) if they do not exist yet.</summary>
    [RelayCommand]
    private async Task AddStandardClassesAsync()
    {
        string[] names = ["LKG", "UKG", .. Enumerable.Range(1, 12).Select(n => $"Class {n}")];
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var existing = await db.Classes.Where(c => c.Section == "A").Select(c => c.Name).ToListAsync();
            var added = 0;
            for (var i = 0; i < names.Length; i++)
            {
                if (existing.Contains(names[i], StringComparer.OrdinalIgnoreCase)) continue;
                db.Classes.Add(new SchoolClass { Name = names[i], Section = "A", DisplayOrder = i + 1 });
                added++;
            }

            await db.SaveChangesAsync();
            await LoadAsync();
            ShowInfo(added == 0 ? "Standard classes already exist." : $"Added {added} classes.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void NewClassForm()
    {
        SelectedClass = null;
        ClassEditId = 0;
        ClassName = "";
        ClassSection = "A";
        ClassOrder = "";
    }

    // ---------- Subjects ----------

    [RelayCommand]
    private void NewSubject()
    {
        NewSubjectForm();
        ClearMessages();
    }

    [RelayCommand]
    private async Task SaveSubjectAsync()
    {
        if (string.IsNullOrWhiteSpace(SubjectName))
        {
            ShowError("Subject name is required.");
            return;
        }

        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            Subject subject;
            if (SubjectEditId == 0)
            {
                subject = new Subject { Name = "" };
                db.Subjects.Add(subject);
            }
            else
            {
                subject = await db.Subjects.FindAsync(SubjectEditId)
                    ?? throw new InvalidOperationException("This subject no longer exists.");
            }

            subject.Name = SubjectName.Trim();
            subject.Code = SubjectCode.Trim().ToUpperInvariant();
            subject.DisplayOrder = int.TryParse(SubjectOrder, out var order)
                ? order
                : (await db.Subjects.MaxAsync(s => (int?)s.DisplayOrder) ?? 0) + 1;

            await db.SaveChangesAsync();
            ShowInfo($"Saved {subject.Name}.");
            NewSubjectForm();
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    [RelayCommand]
    private async Task DeleteSubjectAsync()
    {
        if (SubjectEditId == 0) return;
        if (!Dialogs.Confirm($"Delete subject {SubjectName}?")) return;

        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            await db.Subjects.Where(s => s.Id == SubjectEditId).ExecuteDeleteAsync();
            NewSubjectForm();
            await LoadAsync();
            ShowInfo("Subject deleted.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void NewSubjectForm()
    {
        SelectedSubject = null;
        SubjectEditId = 0;
        SubjectName = "";
        SubjectCode = "";
        SubjectOrder = "";
    }
}
