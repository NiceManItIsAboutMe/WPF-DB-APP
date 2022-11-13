using Microsoft.Win32;
using System;
using System.Windows;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Services.Interfaces;
using WpfMVVMEfApp.ViewModels;
using WpfMVVMEfApp.ViewModels.AdminViewModels.Editors;
using WpfMVVMEfApp.Views.Windows.Dialogs;
using WpfMVVMEfApp.Views.Windows.Dialogs.Editors;

namespace WpfMVVMEfApp.Services
{
    internal class WindowUserDialogService : IUserDialogService
    {


        public WindowUserDialogService()
        {
        }

        public bool OpenDialogWindow(object item)
        {
            switch (item)
            {
                default: throw new NotSupportedException($"Редактирование объекта типа {item.GetType().Name} не поддерживается");
                case DownloadBookFileViewModel vm:
                    {
                        return DownloadBookFile(vm);
                    }
            }
        }

        public bool Edit(object item)
        {
            switch (item)
            {
                default: throw new NotSupportedException($"Редактирование объекта типа {item.GetType().Name} не поддерживается");
                case BookEditorViewModel vm:
                    {
                        return EditBook(vm);
                    }
                case CategoryEditorViewModel vm:
                    {
                        return EditCategory(vm);
                    }
                case Author author:
                    {
                        return EditAuthor(author);
                    }
                case User user:
                    {
                        return EditUser(user);
                    }
                case Book book:
                    {
                        return EditBook(book);
                    }
                case Category category:
                    {
                        return EditCategory(category);
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

        public bool OpenFile(string Title, out string SelectedFile, string Filter = "Все файлы (*.*)|*.*")
        {
            var file_dialog = new OpenFileDialog
            {
                Title = Title,
                Filter = Filter
            };

            if (file_dialog.ShowDialog() != true)
            {
                SelectedFile = null;
                return false;
            }

            SelectedFile = file_dialog.FileName;

            return true;
        }

        public bool SaveFile(string Title, out string SelectedFile, string DefaultFileName = null, string Filter = "Все файлы (*.*)|*.*")
        {
            var file_dialog = new SaveFileDialog
            {
                Title = Title,
                Filter = Filter
            };
            if (!string.IsNullOrWhiteSpace(DefaultFileName))
                file_dialog.FileName = DefaultFileName;

            if (file_dialog.ShowDialog() != true)
            {
                SelectedFile = null;
                return false;
            }

            SelectedFile = file_dialog.FileName;

            return true;
        }

        #region private
        private bool EditBook(Book book)
        {
            BookEditorViewModel vm = new BookEditorViewModel(book, this);

            BookEditorWindow window = new BookEditorWindow()
            {
                DataContext = vm,
                Owner= Application.Current.MainWindow,
                WindowStartupLocation=WindowStartupLocation.CenterOwner,
            };
            var result = window.ShowDialog();
            return result ?? false;
        }

        /// <summary>
        /// Предпочтительный метод. Позволяет менять категории и авторов.
        /// </summary>
        /// <param name="vm"> ViewModel </param>
        /// <returns>save or not</returns>
        private bool EditBook(BookEditorViewModel vm)
        {
            BookEditorWindow window = new BookEditorWindow()
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            var result = window.ShowDialog();
            return result ?? false;
        }

        private bool EditAuthor(Author author)
        {
            AuthorEditorViewModel vm = new AuthorEditorViewModel(author);

            AuthorEditorWindow window = new AuthorEditorWindow()
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            var result = window.ShowDialog();
            return result ?? false;
        }

        private bool EditCategory(Category category)
        {
            CategoryEditorViewModel vm = new CategoryEditorViewModel(category);

            CategoryEditorWindow window = new CategoryEditorWindow()
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            var result = window.ShowDialog();
            return result ?? false;
        }

        /// <summary>
        /// Предпочтительный метод. Позволяет выбирать книги.
        /// </summary>
        /// <param name="vm"> ViewModel </param>
        /// <returns>save or not</returns>
        private bool EditCategory(CategoryEditorViewModel vm)
        {
            CategoryEditorWindow window = new CategoryEditorWindow()
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            var result = window.ShowDialog();
            return result ?? false;
        }
        private bool EditUser(User user)
        {
            UserEditorViewModel vm = new UserEditorViewModel(user);

            UserEditorWindow window = new UserEditorWindow()
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            var result = window.ShowDialog();
            return result ?? false;
        }

        private bool DownloadBookFile(DownloadBookFileViewModel vm)
        {
            DownloadBookFileWindow window = new DownloadBookFileWindow()
            {
                DataContext = vm,
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            window.ShowDialog();
            return true;
        }
        #endregion


    }
}
