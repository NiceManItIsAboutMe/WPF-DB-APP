using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMVVMEfApp.ViewModels
{
    //Задать в App.xaml
    /*<Application.Resources>
        <ResourceDictionary>
            <vm:ViewModelLocator x:Key="Locator"/>
        </ResourceDictionary>
    </Application.Resources>*/
    internal class ViewModelLocator
    {
        public MainWindowViewModel MainWindowModel => App.Services.GetRequiredService<MainWindowViewModel>();

        public AuthorizationViewModel AuthorizationModel => App.Services.GetRequiredService<AuthorizationViewModel>();
    }
}
