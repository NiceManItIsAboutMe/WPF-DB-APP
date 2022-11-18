using System.Windows;
using System.Windows.Input;

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
