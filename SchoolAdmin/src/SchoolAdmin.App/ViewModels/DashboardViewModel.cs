using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using SchoolAdmin.App.Services;
using SchoolAdmin.Data;

namespace SchoolAdmin.App.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IDbContextFactory<SchoolDbContext> _dbFactory;

    public DashboardViewModel(IDbContextFactory<SchoolDbContext> dbFactory, SessionService session)
    {
        _dbFactory = dbFactory;
        Welcome = $"Welcome, {session.CurrentUser?.FullName}";
        _ = LoadAsync();
    }

    public string Welcome { get; }

    [ObservableProperty] private int _studentCount;
    [ObservableProperty] private int _staffCount;
    [ObservableProperty] private int _classCount;
    [ObservableProperty] private int _userCount;
    [ObservableProperty] private string _errorMessage = "";

    private async Task LoadAsync()
    {
        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            StudentCount = await db.Students.CountAsync(s => s.IsActive);
            StaffCount = await db.Staff.CountAsync(s => s.IsActive);
            ClassCount = await db.Classes.CountAsync();
            UserCount = await db.Users.CountAsync(u => u.IsActive);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not load dashboard: {ex.Message}";
        }
    }
}
