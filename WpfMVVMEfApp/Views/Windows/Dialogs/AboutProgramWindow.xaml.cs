using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfMVVMEfApp.Views.Windows.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для AboutProgramWindow.xaml
    /// </summary>
    public partial class AboutProgramWindow : Window
    {

        public AboutProgramWindow()
        {
            CommandBinding Ok = new CommandBinding(ApplicationCommands.Close);
            Ok.Executed += (s, e) => Close();
            Ok.CanExecute += (s, e) => e.CanExecute = true;
            this.CommandBindings.Add(Ok);
            InitializeComponent();
        }


    }
}
