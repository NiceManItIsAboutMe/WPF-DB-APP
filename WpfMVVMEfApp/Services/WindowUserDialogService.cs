using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.Services.Interfaces;
using WpfMVVMEfApp.ViewModels.Editors;
using WpfMVVMEfApp.Views.Windows.Dialogs;

namespace WpfMVVMEfApp.Services
{
    internal class WindowUserDialogService : IUserDialogService
    {
        private ApplicationContext _db;

        public bool Edit(object item)
        {
            switch (item)
            {
                default: throw new NotSupportedException($"Редактирование объекта типа {item.GetType().Name} не поддерживается");
                case Book book:
                    {
                        return EditBook(book);
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

        public WindowUserDialogService(ApplicationContext db)
        {
            _db = db;
        }

        private bool EditBook(Book book)
        {

            BookEditorViewModel vm = new BookEditorViewModel(book,
                new ObservableCollection<Category>(_db.Categories),
                new ObservableCollection<Author>(_db.Authors));

            BookEditorWindow window = new BookEditorWindow()
            {
                DataContext = vm,
                Owner=App.Current.MainWindow,
                WindowStartupLocation=WindowStartupLocation.CenterOwner,
            };
            var result = window.ShowDialog();
            return result ?? false;
        }
    }
}
