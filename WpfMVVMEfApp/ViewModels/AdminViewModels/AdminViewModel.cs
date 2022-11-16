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

        public AdminViewModel(UsersViewModel usersViewModel,
            AuthorsViewModel authorsViewModel,
            CategoriesViewModel categoriesViewModel,
            BooksViewModel booksViewModel)
        {
            UsersViewModel = usersViewModel;
            AuthorsViewModel = authorsViewModel;
            CategoriesViewModel = categoriesViewModel;
            BooksViewModel = booksViewModel;
        }
    }
}
