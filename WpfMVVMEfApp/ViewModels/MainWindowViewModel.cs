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

        private AuthorizationViewModel _AuthorizationViewModel;

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


        #region команда Переводим окно в режим авторизации

        /// <summary> /// Переводим окно в режим авторизации /// </summary>
        private ICommand _SelectAuthorizationViewModelCommand;

        /// <summary> /// Переводим окно в режим авторизации /// </summary>
        public ICommand SelectAuthorizationViewModelCommand => _SelectAuthorizationViewModelCommand
               ??= new RelayCommand(OnSelectAuthorizationViewModelCommandExecuted, CanSelectAuthorizationViewModelCommandExecute);

        /// <summary> /// Переводим окно в режим авторизации /// </summary>
        public bool CanSelectAuthorizationViewModelCommandExecute(object? p) => true;

        /// <summary> /// Переводим окно в режим авторизации /// </summary>
        public void OnSelectAuthorizationViewModelCommandExecuted(object? p)
        {
            CurrrentViewModel = _AuthorizationViewModel;
        }

        #endregion

        public MainWindowViewModel(ApplicationContext db,AuthorizationViewModel authorizationViewModel, IUserDialogService DialogService)
        {
            _db = db;
            _AuthorizationViewModel = authorizationViewModel;
            //чтобы создать авторизацию нам надо данный сервис а чтобы создать данный сервис надо авторизацию разрываем это кольцо
            _AuthorizationViewModel._MainWindowViewModel = this;
        }

    }
}
