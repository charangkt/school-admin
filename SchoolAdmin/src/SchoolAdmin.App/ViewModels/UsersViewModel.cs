using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using SchoolAdmin.App.Services;
using SchoolAdmin.Data;
using SchoolAdmin.Data.Entities;
using SchoolAdmin.Data.Services;

namespace SchoolAdmin.App.ViewModels;

public partial class UsersViewModel : PageViewModel
{
    private const int MinPasswordLength = 6;

    private readonly IDbContextFactory<SchoolDbContext> _dbFactory;
    private readonly SessionService _session;

    public UsersViewModel(IDbContextFactory<SchoolDbContext> dbFactory, SessionService session)
    {
        _dbFactory = dbFactory;
        _session = session;
        _ = LoadAsync();
    }

    public ObservableCollection<User> Users { get; } = [];
    public Role[] Roles { get; } = Enum.GetValues<Role>();
    public ObservableCollection<Option<int>> StaffOptions { get; } = [];
    public ObservableCollection<Option<int>> StudentOptions { get; } = [];

    [ObservableProperty] private User? _selectedUser;

    // ---- Form fields ----
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FormTitle), nameof(PasswordHint))]
    private int _editId;
    [ObservableProperty] private string _username = "";
    [ObservableProperty] private string _fullName = "";
    [ObservableProperty, NotifyPropertyChangedFor(nameof(IsTeacherRole), nameof(IsStudentRole))]
    private Role _role = Role.Accountant;
    [ObservableProperty] private bool _isActive = true;
    [ObservableProperty] private int? _linkedStaffId;
    [ObservableProperty] private int? _linkedStudentId;

    public bool IsTeacherRole => Role == Role.Teacher;
    public bool IsStudentRole => Role == Role.Student;

    /// <summary>Set from the view's PasswordBox.</summary>
    public string NewPassword { get; set; } = "";

    /// <summary>Raised after saving so the view can clear the PasswordBox.</summary>
    public event Action? PasswordFieldCleared;

    public string FormTitle => EditId == 0 ? "New user" : "Edit user";
    public string PasswordHint => EditId == 0 ? "Password *" : "New password (leave blank to keep)";

    partial void OnSelectedUserChanged(User? value)
    {
        if (value is null) return;
        EditId = value.Id;
        Username = value.Username;
        FullName = value.FullName;
        Role = value.Role;
        IsActive = value.IsActive;
        LinkedStaffId = value.StaffId;
        LinkedStudentId = value.StudentId;
        ClearPassword();
        ClearMessages();
    }

    private async Task LoadAsync()
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var users = await db.Users.AsNoTracking().OrderBy(u => u.Username).ToListAsync();
            Users.Clear();
            foreach (var u in users) Users.Add(u);

            if (StaffOptions.Count == 0)
            {
                var staff = await db.Staff.AsNoTracking().Where(s => s.IsActive).OrderBy(s => s.FullName)
                    .Select(s => new Option<int>(s.Id, s.FullName + " (" + s.EmployeeCode + ")")).ToListAsync();
                foreach (var s in staff) StaffOptions.Add(s);

                var students = await db.Students.AsNoTracking().Where(s => s.IsActive)
                    .OrderBy(s => s.SchoolClass!.DisplayOrder).ThenBy(s => s.RollNo)
                    .Select(s => new Option<int>(s.Id,
                        s.FirstName + " " + s.LastName + " — " + s.SchoolClass!.Name + " (" + s.AdmissionNo + ")"))
                    .ToListAsync();
                foreach (var s in students) StudentOptions.Add(s);
            }
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    [RelayCommand]
    private void New()
    {
        SelectedUser = null;
        EditId = 0;
        Username = FullName = "";
        Role = Role.Accountant;
        IsActive = true;
        LinkedStaffId = null;
        LinkedStudentId = null;
        ClearPassword();
        ClearMessages();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(FullName))
        {
            ShowError("Username and full name are required.");
            return;
        }

        if (EditId == 0 && string.IsNullOrEmpty(NewPassword))
        {
            ShowError("Enter a password for the new user.");
            return;
        }

        if (Role == Role.Teacher && LinkedStaffId is null)
        {
            ShowError("Choose which staff member this teacher login belongs to.");
            return;
        }

        if (Role == Role.Student && LinkedStudentId is null)
        {
            ShowError("Choose which student this login belongs to.");
            return;
        }

        if (!string.IsNullOrEmpty(NewPassword) && NewPassword.Length < MinPasswordLength)
        {
            ShowError($"Password must be at least {MinPasswordLength} characters.");
            return;
        }

        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            User user;
            if (EditId == 0)
            {
                user = new User { Username = "", PasswordHash = "", FullName = "" };
                db.Users.Add(user);
            }
            else
            {
                user = await db.Users.FindAsync(EditId)
                    ?? throw new InvalidOperationException("This user no longer exists.");
            }

            // Never leave the system without an active admin.
            var removesAdmin = user.Role == Role.Admin && user.IsActive && (Role != Role.Admin || !IsActive);
            if (EditId != 0 && removesAdmin
                && !await db.Users.AnyAsync(u => u.Id != EditId && u.Role == Role.Admin && u.IsActive))
            {
                ShowError("This is the only active admin. Create another admin first.");
                return;
            }

            if (EditId == _session.CurrentUser?.Id && !IsActive)
            {
                ShowError("You cannot deactivate your own account.");
                return;
            }

            user.Username = Username.Trim();
            user.FullName = FullName.Trim();
            user.Role = Role;
            user.IsActive = IsActive;
            user.StaffId = Role == Role.Teacher ? LinkedStaffId : null;
            user.StudentId = Role == Role.Student ? LinkedStudentId : null;
            if (!string.IsNullOrEmpty(NewPassword))
            {
                user.PasswordHash = PasswordHasher.Hash(NewPassword);
            }

            await db.SaveChangesAsync();
            EditId = user.Id;
            ClearPassword();
            await LoadAsync();
            ShowInfo($"Saved user {user.Username}.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void ClearPassword()
    {
        NewPassword = "";
        PasswordFieldCleared?.Invoke();
    }
}
