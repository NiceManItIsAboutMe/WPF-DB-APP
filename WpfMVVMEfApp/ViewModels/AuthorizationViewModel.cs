﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xaml;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.Services.Interfaces;
using WpfMVVMEfApp.ViewModels.AdminViewModels;
using WpfMVVMEfApp.Views;
using WpfMVVMEfApp.Views.Windows;

namespace WpfMVVMEfApp.ViewModels
{
    internal class AuthorizationViewModel:ViewModel
    {
        #region поля
        private IUserDialogService _DialogService;
        private ApplicationContext _db;
        public MainWindowViewModel _MainWindowViewModel { get; set; }
        private AdminViewModel _AdminViewModel;

        #region Заголовок string Title

        /// <summary> /// Заголовок /// </summary>
        private string _Title = "Авторизация";

        /// <summary> /// Заголовок /// </summary>
        public string Title { get => _Title; set => Set(ref _Title, value); }

        #endregion


        #region Пароль string Password

        /// <summary> /// Пароль /// </summary>
        private string _Password;

        /// <summary> /// Пароль /// </summary>
        public string Password { get => _Password; set => Set(ref _Password, value); }

        #endregion


        #region Логин string Login

        /// <summary> /// Логин /// </summary>
        private string _Login;

        /// <summary> /// Логин /// </summary>
        public string Login { get => _Login; set => Set(ref _Login, value); }

        #endregion


        #endregion


        #region команда Команда входа

        /// <summary> /// Команда входа /// </summary>
        private ICommand _SignInCommand;

        /// <summary> /// Команда входа /// </summary>
        public ICommand SignInCommand => _SignInCommand
               ??= new RelayCommand(OnSignInCommandExecuted, CanSignInCommandExecute);

        /// <summary> /// Команда входа /// </summary>
        public bool CanSignInCommandExecute(object? p) => true;

        /// <summary> /// Команда входа /// </summary>
        public async void OnSignInCommandExecuted(object? p)
        {
            if (String.IsNullOrEmpty(Login)) { _DialogService.ShowWarning("Введите логин", "Предупреждение"); return; }
            if (String.IsNullOrEmpty(Password)) { _DialogService.ShowWarning("Введите пароль", "Предупреждение"); return; }
            try
            {
                var password = User.HashPassword(Password);

                var user = await _db.Users.Where(u => u.Login == Login && u.Password == password).FirstOrDefaultAsync();

                if (user == null) { _DialogService.ShowWarning("Вы ввели неверный логин или пароль", "Предупреждение"); return; }
                else if(!user.IsAdmin) // изменить потом
                {
                    _MainWindowViewModel.CurrrentViewModel = _AdminViewModel;
                }
                else
                {

                }
            }
            catch (NpgsqlException ex)
            {
                _DialogService.ShowError("Нет доступа к базе данных. Обратитесь в службу поддержки" + Environment.NewLine
                    + "Код ошибки:" + ex.ErrorCode
                    + Environment.NewLine + ex.Message,
                    "Ошибка связи с базой данных");
            }
            catch (InvalidOperationException ex)
            {
                _DialogService.ShowError("Нет доступа к базе данных. Обратитесь в службу поддержки" + Environment.NewLine
                    + ex.Message,
                    "Ошибка связи с базой данных");
            }
            catch (Exception ex)
            {
                _DialogService.ShowError("Возникла ошибка. Обратитесь в службу поддержки." + Environment.NewLine
                    + ex.Message,
                    "Непредвиденная ошибка");
            }
        }
        #endregion

        public AuthorizationViewModel(ApplicationContext db,AdminViewModel adminViewModel, IUserDialogService dialogService)
        {
            _db = db;
            _DialogService = dialogService;
            _AdminViewModel = adminViewModel;
        }
    }
}
