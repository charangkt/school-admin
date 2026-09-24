using CommunityToolkit.Mvvm.ComponentModel;
using SchoolAdmin.App.Services;

namespace SchoolAdmin.App.ViewModels;

/// <summary>Base for every page: shows a success or error message under the form.</summary>
public abstract partial class PageViewModel : ObservableObject
{
    [ObservableProperty] private string _infoText = "";
    [ObservableProperty] private string _errorText = "";

    protected void ShowInfo(string message)
    {
        ErrorText = "";
        InfoText = message;
    }

    protected void ShowError(string message)
    {
        InfoText = "";
        ErrorText = message;
    }

    protected void ShowError(Exception ex) => ShowError(DbErrors.Describe(ex));

    protected void ClearMessages()
    {
        InfoText = "";
        ErrorText = "";
    }
}
