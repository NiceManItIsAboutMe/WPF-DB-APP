using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using System.Xaml;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.Services;
using WpfMVVMEfApp.Services.Interfaces;
using WpfMVVMEfApp.ViewModels;
using WpfMVVMEfApp.ViewModels.AdminViewModels;
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
            .AddSingleton<AuthorizationViewModel>()
            .AddSingleton<MainWindowViewModel>() // singltone объекты данного сервисы создаются в момент обращения к ним все последующие запросы идут к данному объекту
            .AddTransient<DBInitializer>() // нужен всего раз
            .AddTransient<IUserDialogService,WindowUserDialogService>() // диалоги нет смысла все время хранить стоит создавать только в момент обращения к ним
            .AddSingleton<AdminViewModel>()
            .AddSingleton<UsersViewModel>()
            .AddSingleton<BooksViewModel>()
            .AddSingleton<AuthorsViewModel>()
            .AddSingleton<CategoriesViewModel>()

            .AddDbContext<ApplicationContext>(opt => // не уверен как долго живет данный сервис, но раз офф документация советует, скорее всего закрывает как можно быстрее соединение с бд(надеюсь...)
                        {
                            //выбираем секцию из appsettings Database
                            var conf = host.Configuration.GetSection("Database");
                            var type = conf["Type"];
                            switch (type)
                            {
                                case null: throw new InvalidOperationException("Не определен тип БД в appsettings.json");
                                default:throw new InvalidOperationException($"Тип подключения {type} не поддерживается");

                                case "Postgres":
                                    opt.UseNpgsql(conf.GetConnectionString(type));
                                    break;
                                case "Local":
                                    opt.UseInMemoryDatabase(conf.GetConnectionString(type));
                                    break;
                            }
                        })
            ;

        protected override async void OnStartup(StartupEventArgs e)
        {
            //Инициализация бд при запуске
            //await Services.GetRequiredService<DBInitializer>().InitializeAsync();
            base.OnStartup(e);
        }
    }
}
