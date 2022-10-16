using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Services.Interfaces;

namespace WpfMVVMEfApp.Services
{
    internal class WindowUserDialogService : IUserDialogService
    {
        public bool Edit(object item)
        {
            switch (item)
            {
                default: throw new NotSupportedException($"Редактирование объекта типа {item.GetType().Name} не поддерживается");
                case Book book:
                    {
                        return true;
                    }
            }
        }

        public bool Confirm(string Message, string Caption)
        {
            return MessageBox.Show(Message, Caption,
                MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes;
        }

        public void ShowError(string Information, string Caption) => 
            MessageBox.Show(Information, Caption, MessageBoxButton.OK, MessageBoxImage.Error);

        public void ShowInformation(string Information, string Caption) =>
            MessageBox.Show(Information, Caption, MessageBoxButton.OK, MessageBoxImage.Information);

        public void ShowWarning(string Information, string Caption) =>
            MessageBox.Show(Information, Caption, MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
