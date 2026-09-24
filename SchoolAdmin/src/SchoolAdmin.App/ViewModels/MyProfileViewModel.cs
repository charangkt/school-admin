using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using SchoolAdmin.App.Services;
using SchoolAdmin.Data;
using SchoolAdmin.Data.Entities;

namespace SchoolAdmin.App.ViewModels;

/// <summary>The only page a Student login sees: their own record.</summary>
public partial class MyProfileViewModel : PageViewModel
{
    private readonly IDbContextFactory<SchoolDbContext> _dbFactory;
    private readonly int? _studentId;

    public MyProfileViewModel(IDbContextFactory<SchoolDbContext> dbFactory, SessionService session)
    {
        _dbFactory = dbFactory;
        _studentId = session.CurrentUser?.StudentId;
        _ = LoadAsync();
    }

    [ObservableProperty] private Student? _student;

    private async Task LoadAsync()
    {
        if (_studentId is null)
        {
            ShowError("This login is not linked to a student record. Please contact the school office.");
            return;
        }

        try
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            Student = await db.Students.AsNoTracking()
                .Include(s => s.SchoolClass)
                .SingleOrDefaultAsync(s => s.Id == _studentId);

            if (Student is null)
            {
                ShowError("Your student record was not found. Please contact the school office.");
            }
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }
}
