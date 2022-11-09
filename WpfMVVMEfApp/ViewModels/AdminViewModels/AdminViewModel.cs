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
using WpfMVVMEfApp.Services.Interfaces;

namespace WpfMVVMEfApp.ViewModels.AdminViewModels
{
    internal class AdminViewModel : ViewModel
    {
        #region Поля
        private ApplicationDbContext _db;


        #region UsersViewModel

        /// <summary> /// UsersViewModel /// </summary>
        private UsersViewModel _UsersViewModel;

        /// <summary> /// UsersViewModel /// </summary>
        public UsersViewModel UsersViewModel { get => _UsersViewModel; set => Set(ref _UsersViewModel, value); }

        #endregion

        #region BooksViewModel

        /// <summary> /// BooksViewModel /// </summary>
        private BooksViewModel _BooksViewModel;

        /// <summary> /// BooksViewModel /// </summary>
        public BooksViewModel BooksViewModel { get => _BooksViewModel; set => Set(ref _BooksViewModel, value); }

        #endregion

        #region CategoriesViewModel

        /// <summary> /// CategoriesViewModel /// </summary>
        private CategoriesViewModel _CategoriesViewModel;

        /// <summary> /// CategoriesViewModel /// </summary>
        public CategoriesViewModel CategoriesViewModel { get => _CategoriesViewModel; set => Set(ref _CategoriesViewModel, value); }

        #endregion

        #region AuthorsViewModel

        /// <summary> /// AuthorsViewModel /// </summary>
        private AuthorsViewModel _AuthorsViewModel;

        /// <summary> /// AuthorsViewModel /// </summary>
        public AuthorsViewModel AuthorsViewModel { get => _AuthorsViewModel; set => Set(ref _AuthorsViewModel, value); }

        #endregion





        







        #endregion
        /*
                #region Команды
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
        */
        // разобьем данную модель на каждый tabitem не уверен на сколько это правильно, зато создам множество viewModel и view и потренеруюсь
        public AdminViewModel(ApplicationDbContext db, IUserDialogService dialogService, UsersViewModel usersViewModel,AuthorsViewModel authorsViewModel,CategoriesViewModel categoriesViewModel, BooksViewModel booksViewModel)
        {
            _db = db;
            UsersViewModel = usersViewModel;
            AuthorsViewModel = authorsViewModel;
            CategoriesViewModel = categoriesViewModel;
            BooksViewModel = booksViewModel;
        }
    }
}
