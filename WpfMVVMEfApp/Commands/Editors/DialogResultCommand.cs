using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfMVVMEfApp.Commands.Base;

namespace WpfMVVMEfApp.Commands.Editors
{
    internal class DialogResultCommand : Command
    {
        public override bool CanExecute(object? parameter) => true;

        public override void Execute(object? parameter)
        {
            if(App.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsMouseOver) is Window window)
            {
                if(parameter == null) throw new ArgumentNullException(nameof(parameter));

                bool? dialogResult = Convert.ToBoolean(parameter);
                window.DialogResult = dialogResult;
                window.Close();
            }
        }
    }
}
