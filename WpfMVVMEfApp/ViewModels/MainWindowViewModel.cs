using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.Services.Interfaces;

namespace WpfMVVMEfApp.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Поля
        private ApplicationContext _db;
        public BooksViewModel BooksViewModel { get; set; }
        #region Заголовок
        private string _Title = "Библиотека";

        public string Title { get => _Title; set => Set(ref _Title, value); }
        #endregion


        #region Текущая ViewModel

        /// <summary> /// Текущая ViewModel /// </summary>
        private ViewModel _CurrrentViewModel;

        /// <summary> /// Текущая ViewModel /// </summary>
        public ViewModel CurrrentViewModel { get => _CurrrentViewModel; set => Set(ref _CurrrentViewModel, value); }

        #endregion
        #endregion



        public MainWindowViewModel(ApplicationContext db, IUserDialogService DialogService)
        {
            _db = db;
            this.BooksViewModel = new BooksViewModel(_db);
            CurrrentViewModel = new AuthorizationViewModel(db,this, DialogService);
        }

    }
}
