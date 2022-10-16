using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using Microsoft.EntityFrameworkCore;

namespace WpfMVVMEfApp.ViewModels.AdminViewModels
{
    internal class AdminViewModel:ViewModel
    {
        #region Поля
        private ApplicationContext _db;

        #region Выбранная категория

        /// <summary> /// Выбранная категория /// </summary>
        private Category _SelectedCategory;

        /// <summary> /// Выбранная категория /// </summary>
        public Category SelectedCategory
        {
            get => _SelectedCategory;
            set
            {
                /* if (AddBooksInCategoriesCommand.CanExecute(value))
                     AddBooksInCategoriesCommand.Execute(value);*/

                Set(ref _SelectedCategory, value);
            }
        }

        #endregion

        #region Выбранный автор

        /// <summary> /// Выбранный автор /// </summary>
        private Author _SelectedAuthor;

        /// <summary> /// Выбранный автор /// </summary>
        public Author SelectedAuthor
        {
            get => _SelectedAuthor;
            set
            {
                /* if (AddBooksInAuthorsCommand.CanExecute(value))
                     AddBooksInAuthorsCommand.Execute(value);*/
                Set(ref _SelectedAuthor, value);
            }
        }

        #endregion

        #region Выбранный пользователь

        /// <summary> /// Выбранный пользователь /// </summary>
        private User _SelectedUser;

        /// <summary> /// Выбранный пользователь /// </summary>
        public User SelectedUser { get => _SelectedUser; set => Set(ref _SelectedUser, value); }

        #endregion


        #region Категории

        /// <summary> /// Книги /// </summary>
        private ObservableCollection<Category> _Categories;

        /// <summary> /// Книги /// </summary>
        public ObservableCollection<Category> Categories { get => _Categories; set => Set(ref _Categories, value); }

        #endregion

        #region Авторы

        /// <summary> /// Авторы /// </summary>
        private ObservableCollection<Author> _Authors;

        /// <summary> /// Авторы /// </summary>
        public ObservableCollection<Author> Authors { get => _Authors; set => Set(ref _Authors, value); }

        #endregion

        #region Пользователи

        /// <summary> /// Пользователи /// </summary>
        private ObservableCollection<User> _Users;

        /// <summary> /// Пользователи /// </summary>
        public ObservableCollection<User> Users { get => _Users; set => Set(ref _Users, value); }

        #endregion


        #region Книги

        /// <summary> /// Книги /// </summary>
        private ObservableCollection<Book> _Books;

        /// <summary> /// Книги /// </summary>
        public ObservableCollection<Book> Books { get => _Books; set => Set(ref _Books, value); }

        #endregion


#endregion

        #region Команды

        #region команда Сбросить пароль
        /// <summary> /// Сбросить пароль /// </summary>
        private ICommand _PasswordResetCommand;

        /// <summary> /// Сбросить пароль /// </summary>
        public ICommand PasswordResetCommand => _PasswordResetCommand ??= new RelayCommand(OnPasswordResetCommandExecuted, CanPasswordResetCommandExecute);

        /// <summary> /// Сбросить пароль /// </summary>
        public bool CanPasswordResetCommandExecute(object? p) => true;

        /// <summary> /// Сбросить пароль /// </summary>
        public void OnPasswordResetCommandExecuted(object? p)
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show($"Вы уверены, что хотите сбросить пароль " +
                $"{SelectedUser.Surname + ' ' + SelectedUser.Name + ' ' + SelectedUser.Patronymic}?",
                "Сброс пароля", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                SelectedUser.Password = User.HashPassword("PasswordResetedPleaseEnterNewPassword");
                _db.Users.Update(SelectedUser);
                _db.SaveChanges();
            }
        }

        #endregion


        #region команда Обновление данных в бд

        /// <summary> /// Обновление данных в бд /// </summary>
        private ICommand _UpdateDbDataCommand;

        /// <summary> /// Обновление данных в бд /// </summary>
        public ICommand UpdateDbDataCommand => _UpdateDbDataCommand
               ??= new RelayCommand(OnUpdateDbDataCommandExecuted, CanUpdateDbDataCommandExecute);

        /// <summary> /// Обновление данных в бд /// </summary>
        public bool CanUpdateDbDataCommandExecute(object? p) => true;

        /// <summary> /// Обновление данных в бд /// </summary>
        public void OnUpdateDbDataCommandExecuted(object? p)
        {
            if (p is null)
            {
                MessageBox.Show("Ошибка. Обратитесь в службу поддержки.");
                return;
            }

            var dRes = MessageBox.Show("Вы уверены, что хотите сохранить изменения? Изменить данные обратно можно будет только вручную.",
                "Сохранение изменений", MessageBoxButton.YesNo);
            if (dRes == MessageBoxResult.No) return;

            if (p is ObservableCollection<Category> categories)
            {
                _db.Categories.UpdateRange(categories);
            }
            else if (p is ObservableCollection<User> users)
            {
                _db.Users.UpdateRange(users);
            }
            else if (p is ObservableCollection<Author> authors)
            {
                _db.Authors.UpdateRange(authors);
            }
            else if (p is ObservableCollection<Book> books)
            {
                _db.Books.UpdateRange(books);
            }
            else
            {
                MessageBox.Show("Ошибка. Обратитесь в службу поддержки.");
                return;
            }
            _db.SaveChanges();
        }

        #endregion

        #region команда Отмена обновления данных, берем данные из бд

        /// <summary> /// Отмена обновления данных, берем данные из бд /// </summary>
        private ICommand _CancelDbUpdateDataCommand;

        /// <summary> /// Отмена обновления данных, берем данные из бд /// </summary>
        public ICommand CancelDbUpdateDataCommand => _CancelDbUpdateDataCommand
               ??= new RelayCommand(OnCancelDbUpdateDataCommandExecuted, CanCancelDbUpdateDataCommandExecute);

        /// <summary> /// Отмена обновления данных, берем данные из бд /// </summary>
        public bool CanCancelDbUpdateDataCommandExecute(object? p) => true;

        /// <summary> /// Отмена обновления данных, берем данные из бд /// </summary>
        public void OnCancelDbUpdateDataCommandExecuted(object? p)
        {
            if (p is null)
            {
                MessageBox.Show("Ошибка. Обратитесь в службу поддержки.");
                return;
            }

            var dRes = MessageBox.Show("Вы уверены, что хотите отменить изменения? Изменить данные обратно можно будет только вручную.",
               "Отмена изменений", MessageBoxButton.YesNo);
            if (dRes == MessageBoxResult.No) return;

            if (p is ObservableCollection<Category> categories)
            {
                Categories = new ObservableCollection<Category>(_db.Categories.Include(c => c.Books));
            }
            else if (p is ObservableCollection<User> users)
            {
                Users = new ObservableCollection<User>(_db.Users.Include(a => a.Books));
            }
            else if (p is ObservableCollection<Author> authors)
            {
                Authors = new ObservableCollection<Author>(_db.Authors.Include(a => a.Books));
            }
            else if (p is ObservableCollection<Book> books)
            {
                Books = new ObservableCollection<Book>(_db.Books);
            }
            else
            {
                MessageBox.Show("Ошибка. Обратитесь в службу поддержки.");
                return;
            }

        }

        #endregion


        #region команда обновление таблиц

        /// <summary> ///  /// </summary>
        private ICommand _NAMECommand;

        /// <summary> ///  /// </summary>
        public ICommand NAMECommand => _NAMECommand
               ??= new RelayCommand(OnNAMECommandExecuted, CanNAMECommandExecute);

        /// <summary> ///  /// </summary>
        public bool CanNAMECommandExecute(object? p) => true;

        /// <summary> ///  /// </summary>
        public void OnNAMECommandExecuted(object? p)
        {
        }

        #endregion


        #region команда Отображения книг

        /// <summary> /// Отображения книг /// </summary>
        private ICommand _SelectBookViewModelCommand;

        /// <summary> /// Отображения книг /// </summary>
        public ICommand SelectBookViewModelCommand => _SelectBookViewModelCommand
               ??= new RelayCommand(OnSelectBookViewModelCommandExecuted, CanSelectBookViewModelCommandExecute);

        /// <summary> /// Отображения книг /// </summary>
        public bool CanSelectBookViewModelCommandExecute(object? p) => true;

        /// <summary> /// Отображения книг /// </summary>
        public void OnSelectBookViewModelCommandExecuted(object? p)
        {
            //CurrrentViewModel = new BooksViewModel(_db);
        }

        #endregion
        #endregion

        public AdminViewModel(ApplicationContext db)
        {
            _db = db;
            Categories = new ObservableCollection<Category>(db.Categories.Include(c => c.Books));
            Authors = new ObservableCollection<Author>(db.Authors.Include(a => a.Books));
            Users = new ObservableCollection<User>(db.Users.Include(a => a.Books));
            Books = new ObservableCollection<Book>(db.Books);
        }
    }
}
