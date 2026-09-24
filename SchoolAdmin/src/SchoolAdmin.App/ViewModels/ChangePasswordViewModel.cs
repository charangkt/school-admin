using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolAdmin.App.Services;
using SchoolAdmin.Data.Services;

namespace SchoolAdmin.App.ViewModels;

public partial class ChangePasswordViewModel(AuthService auth, SessionService session) : ObservableObject
{
    // Set from the view's PasswordBoxes.
    public string CurrentPassword { get; set; } = "";
    public string NewPassword { get; set; } = "";
    public string ConfirmPassword { get; set; } = "";

    [ObservableProperty] private string _errorMessage = "";

    public event Action? Completed;

    [RelayCommand]
    private async Task ChangeAsync()
    {
        ErrorMessage = "";
        if (session.CurrentUser is null) return;

        if (NewPassword.Length < 6)
        {
            ErrorMessage = "New password must be at least 6 characters.";
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "The new passwords do not match.";
            return;
        }

        try
        {
            if (!await auth.ChangePasswordAsync(session.CurrentUser.Id, CurrentPassword, NewPassword))
            {
                ErrorMessage = "Current password is incorrect.";
                return;
            }

            Completed?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = DbErrors.Describe(ex);
        }
    }
}
