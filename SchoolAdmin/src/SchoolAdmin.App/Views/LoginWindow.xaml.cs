using System.Windows;
using SchoolAdmin.App.ViewModels;

namespace SchoolAdmin.App.Views;

public partial class LoginWindow : Window
{
    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        PasswordInput.PasswordChanged += (_, _) => viewModel.Password = PasswordInput.Password;
        viewModel.LoginSucceeded += () =>
        {
            App.ShowMain();
            Close();
        };
        Loaded += (_, _) => UsernameInput.Focus();
    }
}
