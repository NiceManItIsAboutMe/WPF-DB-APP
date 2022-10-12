using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using System.Xaml;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.ViewModels;
using WpfMVVMEfApp.Views;

namespace WpfMVVMEfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost _Host;

        public static IHost Host => _Host
            ??= Program.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

        public static IServiceProvider Services => Host.Services;

        internal static void ConfigureServices(HostBuilderContext host, IServiceCollection services) => services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<AuthorizationViewModel>()
            .AddDbContext<ApplicationContext>(opt => opt.UseNpgsql(host.Configuration.GetConnectionString("Bookinist")))
            .AddTransient<DBInitializer>()
            ;
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //Инициализация бд при запуске
            await Services.GetRequiredService<DBInitializer>().InitializeAsync();
        }
    }
}
