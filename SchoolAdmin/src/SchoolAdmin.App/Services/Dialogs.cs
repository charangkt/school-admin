using System.Windows;

namespace SchoolAdmin.App.Services;

public static class Dialogs
{
    public static bool Confirm(string message, string title = "Please confirm") =>
        MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
}
