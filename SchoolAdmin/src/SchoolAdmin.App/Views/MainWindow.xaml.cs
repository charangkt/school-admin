using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SchoolAdmin.App.ViewModels;

namespace SchoolAdmin.App.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        viewModel.LogoutRequested += () =>
        {
            App.ShowLogin();
            Close();
        };
    }

    private void OnChangePasswordClick(object sender, RoutedEventArgs e)
    {
        var dialog = new ChangePasswordWindow(App.Services.GetRequiredService<ChangePasswordViewModel>())
        {
            Owner = this,
        };
        dialog.ShowDialog();
    }
}
