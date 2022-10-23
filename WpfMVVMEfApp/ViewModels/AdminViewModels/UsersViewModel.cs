﻿using Microsoft.EntityFrameworkCore;
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
using WpfMVVMEfApp.Services.Interfaces;
using System.Windows.Data;
using System.ComponentModel;
using WpfMVVMEfApp.Views.AdminViews;

namespace WpfMVVMEfApp.ViewModels.AdminViewModels
{
    internal class UsersViewModel : ViewModel
    {
        private ApplicationContext _db;

        private IUserDialogService _DialogService;


        #region Строка поиска

        /// <summary> /// Строка поиска /// </summary>
        private string _SearchFilter;

        /// <summary> /// Строка поиска /// </summary>
        public string SearchFilter
        {
            get => _SearchFilter;
            set
            {
                if (Set(ref _SearchFilter, value))
                    _UsersViewSource.View.Refresh();
            }
        }

        #endregion

        private CollectionViewSource _UsersViewSource;

        /// <summary>/// Отображение пользователей /// </summary>
        public ICollectionView UsersView => _UsersViewSource?.View;

        #region Выбранный пользователь

        /// <summary> /// Выбранный пользователь /// </summary>
        private User _SelectedUser;

        /// <summary> /// Выбранный пользователь /// </summary>
        public User SelectedUser { get => _SelectedUser; set => Set(ref _SelectedUser, value); }

        #endregion

        #region Пользователи

        /// <summary> /// Пользователи /// </summary>
        private ObservableCollection<User> _Users;

        /// <summary> /// Пользователи /// </summary>
        public ObservableCollection<User> Users
        {
            get => _Users;
            set
            {
                if(Set(ref _Users, value))
                {
                    _UsersViewSource = new CollectionViewSource
                    {
                        Source = Users,
                        SortDescriptions =
                        {
                            new SortDescription(nameof(User.Surname), ListSortDirection.Ascending),
                        }
                    };
                    _UsersViewSource.Filter += OnSearchFilter;
                    OnPropertyChanged(nameof(UsersView));
                    _UsersViewSource.View.Refresh();
                }
            }
        }

        #endregion

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

            var result = _DialogService.Confirm($"Вы уверены, что хотите сбросить пароль " +
                $"{SelectedUser.Surname + ' ' + SelectedUser.Name + ' ' + SelectedUser.Patronymic}?",
                "Сброс пароля");
            if (result)
            {
                SelectedUser.Password = User.HashPassword("PasswordResetedPleaseEnterNewPassword");
                _db.Users.Update(SelectedUser);
                _db.SaveChanges();
            }
        }

        #endregion


        #region команда Загрузка данных пользователей из бд

        /// <summary> /// Загрузка данных пользователей из бд /// </summary>
        private ICommand _LoadUsersDataCommand;

        /// <summary> /// Загрузка данных пользователей из бд /// </summary>
        public ICommand LoadUsersDataCommand => _LoadUsersDataCommand
               ??= new RelayCommand(OnLoadUsersDataCommandExecuted, CanLoadUsersDataCommandExecute);

        /// <summary> /// Загрузка данных пользователей из бд /// </summary>
        public bool CanLoadUsersDataCommandExecute(object? p) => true;

        /// <summary> /// Загрузка данных пользователей из бд /// </summary>
        public void OnLoadUsersDataCommandExecuted(object? p)
        {
            Users = new ObservableCollection<User>(_db.Users.Include(a => a.Books));
        }

        #endregion



        public UsersViewModel(ApplicationContext db, IUserDialogService dialogService)
        {
            _db = db;
            _DialogService = dialogService;
        }

        private void OnSearchFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is User user) || string.IsNullOrEmpty(SearchFilter)) return;

            if (!user.Name.Contains(_SearchFilter, StringComparison.OrdinalIgnoreCase) 
                && !user.Surname.Contains(_SearchFilter, StringComparison.OrdinalIgnoreCase)
                && !user.Patronymic.Contains(_SearchFilter, StringComparison.OrdinalIgnoreCase) 
                && !user.Login.Contains(_SearchFilter, StringComparison.OrdinalIgnoreCase))
                e.Accepted = false;
        }
    }
}