using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SchoolAdmin.App.Services;
using SchoolAdmin.Data.Services;

namespace SchoolAdmin.App.ViewModels;

public partial class LoginViewModel(AuthService auth, SessionService session) : ObservableObject
{
    [ObservableProperty]
    private string _username = "";

    /// <summary>Set from the view's PasswordBox (WPF does not allow binding passwords).</summary>
    public string Password { get; set; } = "";

    [ObservableProperty]
    private string _errorMessage = "";

    public event Action? LoginSucceeded;

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = "";
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrEmpty(Password))
        {
            ErrorMessage = "Enter your username and password.";
            return;
        }

        try
        {
            var user = await auth.LoginAsync(Username, Password);
            if (user is null)
            {
                ErrorMessage = "Invalid username or password.";
                return;
            }

            session.CurrentUser = user;
            LoginSucceeded?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not reach the database: {ex.Message}";
        }
    }
}
