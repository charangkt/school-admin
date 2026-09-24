using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolAdmin.App.Services;
using SchoolAdmin.App.ViewModels;
using SchoolAdmin.App.Views;
using SchoolAdmin.Data;
using SchoolAdmin.Data.Services;

namespace SchoolAdmin.App;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.AddDbContextFactory<SchoolDbContext>(o => o.UseSqlServer(config.GetConnectionString("SchoolDb")));
        services.AddSingleton<AuthService>();
        services.AddSingleton<SessionService>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<StudentsViewModel>();
        services.AddTransient<StaffViewModel>();
        services.AddTransient<ClassesViewModel>();
        services.AddTransient<UsersViewModel>();
        services.AddTransient<ChangePasswordViewModel>();
        services.AddTransient<MyProfileViewModel>();
        Services = services.BuildServiceProvider();

        try
        {
            var dbFactory = Services.GetRequiredService<IDbContextFactory<SchoolDbContext>>();
            await using var db = await dbFactory.CreateDbContextAsync();
            await DbInitializer.InitializeAsync(db);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"School Admin could not start because the database is not available.\n\n{DbErrors.Describe(ex)}",
                "School Admin", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        ShowLogin();
    }

    public static void ShowLogin()
    {
        var login = new LoginWindow(Services.GetRequiredService<LoginViewModel>());
        Current.MainWindow = login;
        login.Show();
    }

    public static void ShowMain()
    {
        var main = new MainWindow(Services.GetRequiredService<MainViewModel>());
        Current.MainWindow = main;
        main.Show();
    }
}
