using System.Windows;
using SchoolAdmin.App.ViewModels;

namespace SchoolAdmin.App.Views;

public partial class ChangePasswordWindow : Window
{
    public ChangePasswordWindow(ChangePasswordViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        CurrentInput.PasswordChanged += (_, _) => viewModel.CurrentPassword = CurrentInput.Password;
        NewInput.PasswordChanged += (_, _) => viewModel.NewPassword = NewInput.Password;
        ConfirmInput.PasswordChanged += (_, _) => viewModel.ConfirmPassword = ConfirmInput.Password;

        viewModel.Completed += () =>
        {
            MessageBox.Show(this, "Your password has been changed.", "Change password",
                MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
        };
        Loaded += (_, _) => CurrentInput.Focus();
    }
}
