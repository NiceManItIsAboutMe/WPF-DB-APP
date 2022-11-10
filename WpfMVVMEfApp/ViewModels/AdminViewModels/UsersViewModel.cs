using Microsoft.EntityFrameworkCore;
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
using static WpfMVVMEfApp.ViewModels.Editors.CategoryEditorViewModel;
using static WpfMVVMEfApp.ViewModels.Editors.BookEditorViewModel;

namespace WpfMVVMEfApp.ViewModels.AdminViewModels
{
    internal class UsersViewModel : ViewModel
    {
        private IDbContextFactory<ApplicationDbContext> _dbFactory;

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

        #region команда Сбросить пароль только сбрасывает пароль, но не меняет его
        /// <summary> /// Сбросить пароль /// </summary>
        private ICommand _PasswordResetCommand;

        /// <summary> /// Сбросить пароль /// </summary>
        public ICommand PasswordResetCommand => _PasswordResetCommand ??= new RelayCommand(OnPasswordResetCommandExecuted, CanPasswordResetCommandExecute);

        /// <summary> /// Сбросить пароль /// </summary>
        public bool CanPasswordResetCommandExecute(object? p) => SelectedUser is User;

        /// <summary> /// Сбросить пароль /// </summary>
        public async void OnPasswordResetCommandExecuted(object? p)
        {
            if (SelectedUser == null) return;

            var result = _DialogService.Confirm($"Вы уверены, что хотите сбросить пароль " +
                $"{SelectedUser.Surname + ' ' + SelectedUser.Name + ' ' + SelectedUser.Patronymic}?",
                "Сброс пароля");
            if (result)
            {
                SelectedUser.Password = User.HashPassword("PasswordResetedPleaseEnterNewPassword");
                using (var db = await _dbFactory.CreateDbContextAsync())
                {
                    db.Users.Update(SelectedUser);
                    await db.SaveChangesAsync();
                }
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
        public async void OnLoadUsersDataCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            Users = new ObservableCollection<User>(await db.Users.Include(a => a.Books).AsNoTracking().ToListAsync());
        }

        #endregion

        #region команда Удаление пользователя

        /// <summary> /// Удаление пользователя /// </summary>
        private ICommand _RemoveSelectedUserCommand;

        /// <summary> /// Удаление пользователя /// </summary>
        public ICommand RemoveSelectedUserCommand => _RemoveSelectedUserCommand
               ??= new RelayCommand(OnRemoveSelectedUserCommandExecuted, CanRemoveSelectedUserCommandExecute);

        /// <summary> /// Удаление пользователя /// </summary>
        public bool CanRemoveSelectedUserCommandExecute(object? p) => SelectedUser is User;

        /// <summary> /// Удаление пользователя /// </summary>
        public async void OnRemoveSelectedUserCommandExecuted(object? p)
        {
            if (SelectedUser == null) return;

            using var db = await _dbFactory.CreateDbContextAsync();

            User user = await db.Users.FirstAsync(u => u.Id == SelectedUser.Id);
            if (!_DialogService.Confirm($"Вы действительно хотите удалить пользователя: {SelectedUser}?", "Удаление"))
                return;
            db.Remove(user);
            await db.SaveChangesAsync();
            Users.Remove(SelectedUser);
        }

        #endregion

        #region команда Редактирование пользователя и сброс пароля если p=true

        /// <summary> /// Редактирование пользователя /// </summary>
        private ICommand _EditSelectedUserCommand;

        /// <summary> /// Редактирование пользователя /// </summary>
        public ICommand EditSelectedUserCommand => _EditSelectedUserCommand
               ??= new RelayCommand(OnEditSelectedUserCommandExecuted, CanEditSelectedUserCommandExecute);

        /// <summary>
        /// Редактирование пользователя
        /// </summary>
        /// <param name="p"> bool - сброс пароля</param>
        /// <returns></returns>
        public bool CanEditSelectedUserCommandExecute(object? p) => SelectedUser is User;

        /// <summary>
        /// Редактирование пользователя
        /// </summary>
        /// <param name="p"> bool - сброс пароля </param>
        public async void OnEditSelectedUserCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();

            User user = await db.Users.FirstAsync(u => u.Id == SelectedUser.Id);

            if(p != null)
            {
                if (Convert.ToBoolean(p))
                    user.Password = null;
            }

            bool result = _DialogService.Edit(user);
            if (!result)
            {
                return;
            }
            try
            {
                db.Update(user);
                await db.SaveChangesAsync();
                Users.Remove(SelectedUser);
                Users.Add(user);
                SelectedUser = user;
                _UsersViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                _DialogService.ShowError("Возможно вы попытались создать объект, имя которого уже существует." +
                    "\nВ ином случае обратитесь в службу поддержки" +
                    $"\n{ex.Message}", "Ошибка сохранения объекта");
            }
        }

        #endregion

        #region команда Добавить пользователя

        /// <summary> /// Добавить пользователя /// </summary>
        private ICommand _AddUserCommand;

        /// <summary> /// Добавить пользователя /// </summary>
        public ICommand AddUserCommand => _AddUserCommand
               ??= new RelayCommand(OnAddUserCommandExecuted, CanAddUserCommandExecute);

        /// <summary> /// Добавить пользователя /// </summary>
        public bool CanAddUserCommandExecute(object? p) => true;

        /// <summary> /// Добавить пользователя /// </summary>
        public async void OnAddUserCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();

            User user = new User();
            bool result = _DialogService.Edit(user);
            if (!result)
            {
                return;
            }
            try
            {
                db.Add(user);
                await db.SaveChangesAsync();
                Users.Add(user);
                SelectedUser = user;
                _UsersViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                db.Remove(user);
                _DialogService.ShowError("Возможно вы попытались создать объект, имя которого уже существует." +
                    "\nВ ином случае обратитесь в службу поддержки" +
                    $"\n{ex.Message}", "Ошибка сохранения объекта");
            }
        }

        #endregion

        public UsersViewModel(IDbContextFactory<ApplicationDbContext> dbFactory, IUserDialogService dialogService)
        {
            _dbFactory = dbFactory;
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
