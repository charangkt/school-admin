using System.Windows;
using System.Windows.Controls;
using SchoolAdmin.App.ViewModels;

namespace SchoolAdmin.App.Views;

public partial class UsersView : UserControl
{
    public UsersView()
    {
        InitializeComponent();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not UsersViewModel vm) return;

        // WPF cannot bind PasswordBox.Password, so pass it to the view model by hand.
        PasswordInput.PasswordChanged += (_, _) => vm.NewPassword = PasswordInput.Password;
        vm.PasswordFieldCleared += () => PasswordInput.Clear();
    }
}
