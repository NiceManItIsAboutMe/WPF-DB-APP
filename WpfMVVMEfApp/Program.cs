using Microsoft.Extensions.Hosting;
using System;
using Microsoft.EntityFrameworkCore;

namespace WpfMVVMEfApp
{
    /// <summary>
    /// точка входа 
    /// </summary>
    internal class Program
    {
        [STAThread]
        public static void Main()
        {
            var app=new App();
            app.InitializeComponent();
            app.Run();
        }
        /// <summary>
        /// нужен для EF
        /// </summary>
        public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .ConfigureServices(App.ConfigureServices);
    }
}
