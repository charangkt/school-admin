using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using SchoolAdmin.App.Services;
using SchoolAdmin.Data.Entities;

namespace SchoolAdmin.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly SessionService _session;

    public MainViewModel(SessionService session, IServiceProvider services)
    {
        _session = session;
        var user = session.CurrentUser ?? throw new InvalidOperationException("No user is logged in.");
        UserDisplay = $"{user.FullName} · {user.Role}";

        // Each menu item lists the roles that may open it.
        var menu = new (NavItem Item, Role[] Roles)[]
        {
            (new("My Profile", PackIconKind.AccountCircle, services.GetRequiredService<MyProfileViewModel>),
                [Role.Student]),
            (new("Dashboard", PackIconKind.ViewDashboard, services.GetRequiredService<DashboardViewModel>),
                [Role.Admin, Role.Accountant, Role.ExamStaff, Role.Teacher]),
            (new("Students", PackIconKind.AccountSchool, services.GetRequiredService<StudentsViewModel>),
                [Role.Admin, Role.Accountant, Role.ExamStaff, Role.Teacher]),
            (new("Staff", PackIconKind.AccountTie, services.GetRequiredService<StaffViewModel>),
                [Role.Admin]),
            (new("Classes & Subjects", PackIconKind.GoogleClassroom, services.GetRequiredService<ClassesViewModel>),
                [Role.Admin]),
            (new("Fees", PackIconKind.CashMultiple, () => new PlaceholderViewModel("Fees", "Fee heads, collection and receipts — coming in Days 3–4.")),
                [Role.Admin, Role.Accountant]),
            (new("Exams", PackIconKind.ClipboardTextOutline, () => new PlaceholderViewModel("Exams", "Exam schedule, marks and admit cards — coming in Days 5–6.")),
                [Role.Admin, Role.ExamStaff, Role.Teacher]),
            (new("Users", PackIconKind.AccountKey, services.GetRequiredService<UsersViewModel>),
                [Role.Admin]),
        };

        NavItems = new ObservableCollection<NavItem>(
            menu.Where(m => m.Roles.Contains(user.Role)).Select(m => m.Item));
        SelectedNavItem = NavItems.FirstOrDefault();
    }

    public string UserDisplay { get; }

    public ObservableCollection<NavItem> NavItems { get; }

    [ObservableProperty]
    private NavItem? _selectedNavItem;

    [ObservableProperty]
    private object? _currentPage;

    public event Action? LogoutRequested;

    partial void OnSelectedNavItemChanged(NavItem? value) => CurrentPage = value?.CreatePage();

    [RelayCommand]
    private void Logout()
    {
        _session.CurrentUser = null;
        LogoutRequested?.Invoke();
    }
}
