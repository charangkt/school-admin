using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using SchoolAdmin.App.Services;
using SchoolAdmin.Data;
using SchoolAdmin.Data.Entities;

namespace SchoolAdmin.App.ViewModels;

public partial class StaffViewModel : PageViewModel
{
    private readonly IDbContextFactory<SchoolDbContext> _dbFactory;

    public StaffViewModel(IDbContextFactory<SchoolDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
        _ = LoadAsync();
    }

    public ObservableCollection<Staff> StaffMembers { get; } = [];

    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private Staff? _selectedStaff;

    // ---- Form fields ----
    [ObservableProperty, NotifyPropertyChangedFor(nameof(FormTitle))]
    private int _editId;
    [ObservableProperty] private string _employeeCode = "";
    [ObservableProperty] private string _fullName = "";
    [ObservableProperty] private string _designation = "";
    [ObservableProperty] private string _phone = "";
    [ObservableProperty] private string _email = "";
    [ObservableProperty] private DateTime? _joiningDate = DateTime.Today;
    [ObservableProperty] private bool _isActive = true;

    public string FormTitle => EditId == 0 ? "New staff member" : "Edit staff member";

    partial void OnSearchTextChanged(string value) => _ = LoadAsync();
    partial void OnSelectedStaffChanged(Staff? value)
    {
        if (value is null) return;
        EditId = value.Id;
        EmployeeCode = value.EmployeeCode;
        FullName = value.FullName;
        Designation = value.Designation;
        Phone = value.Phone;
        Email = value.Email;
        JoiningDate = value.JoiningDate.ToDateTime();
        IsActive = value.IsActive;
        ClearMessages();
    }

    private async Task LoadAsync()
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var query = db.Staff.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var term = SearchText.Trim();
                query = query.Where(s => s.EmployeeCode.Contains(term) || s.FullName.Contains(term)
                    || s.Designation.Contains(term) || s.Phone.Contains(term));
            }

            var staff = await query.OrderBy(s => s.FullName).ToListAsync();
            StaffMembers.Clear();
            foreach (var s in staff) StaffMembers.Add(s);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    [RelayCommand]
    private void New()
    {
        SelectedStaff = null;
        EditId = 0;
        EmployeeCode = FullName = Designation = Phone = Email = "";
        JoiningDate = DateTime.Today;
        IsActive = true;
        ClearMessages();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(EmployeeCode) || string.IsNullOrWhiteSpace(FullName))
        {
            ShowError("Employee code and full name are required.");
            return;
        }

        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            Staff staff;
            if (EditId == 0)
            {
                staff = new Staff { EmployeeCode = "", FullName = "" };
                db.Staff.Add(staff);
            }
            else
            {
                staff = await db.Staff.FindAsync(EditId)
                    ?? throw new InvalidOperationException("This staff member no longer exists.");
            }

            staff.EmployeeCode = EmployeeCode.Trim();
            staff.FullName = FullName.Trim();
            staff.Designation = Designation.Trim();
            staff.Phone = Phone.Trim();
            staff.Email = Email.Trim();
            staff.JoiningDate = JoiningDate.ToDateOnly() ?? DateOnly.FromDateTime(DateTime.Today);
            staff.IsActive = IsActive;

            await db.SaveChangesAsync();
            EditId = staff.Id;
            await LoadAsync();
            ShowInfo($"Saved {staff.FullName}.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (EditId == 0) return;
        if (!Dialogs.Confirm($"Delete staff member {FullName}? This cannot be undone.")) return;

        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            await db.Staff.Where(s => s.Id == EditId).ExecuteDeleteAsync();
            New();
            await LoadAsync();
            ShowInfo("Staff member deleted.");
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }
}
