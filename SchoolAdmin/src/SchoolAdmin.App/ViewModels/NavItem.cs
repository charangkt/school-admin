using MaterialDesignThemes.Wpf;

namespace SchoolAdmin.App.ViewModels;

/// <summary>One entry in the left-hand menu. CreatePage builds the page's view model when selected.</summary>
public record NavItem(string Title, PackIconKind Icon, Func<object> CreatePage);
