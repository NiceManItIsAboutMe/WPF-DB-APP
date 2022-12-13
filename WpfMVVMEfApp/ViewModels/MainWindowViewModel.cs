using Microsoft.EntityFrameworkCore;
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
        private IUserDialogService _DialogService;
        IDbContextFactory<ApplicationDbContext> _dbFactory;

        #region User

        /// <summary> /// User /// </summary>
        private User _User;

        /// <summary> /// User /// </summary>
        public User User { get => _User; set { if (Set(ref _User, value)) IsAdmin = _User?.IsAdmin ?? false; } }

        #endregion

        #region IsAdmin

        /// <summary> /// IsAdmin /// </summary>
        private bool _IsAdmin = false;

        /// <summary> /// IsAdmin /// </summary>
        public bool IsAdmin { get => _IsAdmin; set => Set(ref _IsAdmin, value); }

        #endregion

        public AuthorizationViewModel _AuthorizationViewModel;

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

        #region команда Сменить интерфейс

        /// <summary> /// Сменить интерфейс /// </summary>
        private ICommand _SwitchInterfaceCommand;

        /// <summary> /// Сменить интерфейс /// </summary>
        public ICommand SwitchInterfaceCommand => _SwitchInterfaceCommand
               ??= new RelayCommand(OnSwitchInterfaceCommandExecuted, CanSwitchInterfaceCommandExecute);

        /// <summary> /// Сменить интерфейс /// </summary>
        public bool CanSwitchInterfaceCommandExecute(object? p) => true;

        /// <summary> /// Сменить интерфейс /// </summary>
        public void OnSwitchInterfaceCommandExecuted(object? p)
        {
            IsAdmin = !IsAdmin;
        }

        #endregion

        #region команда О программе

        /// <summary> /// О программе /// </summary>
        private ICommand _AboutProgramCommand;

        /// <summary> /// О программе /// </summary>
        public ICommand AboutProgramCommand => _AboutProgramCommand
               ??= new RelayCommand(OnAboutProgramCommandExecuted, CanAboutProgramCommandExecute);

        /// <summary> /// О программе /// </summary>
        public bool CanAboutProgramCommandExecute(object? p) => true;

        /// <summary> /// О программе /// </summary>
        public void OnAboutProgramCommandExecuted(object? p)
        {
            _DialogService.OpenDialogWindow("About");
        }

        #endregion

        #region команда Выход из профиля

        /// <summary> /// Выход из профиля /// </summary>
        private ICommand _ExitProfileCommand;

        /// <summary> /// Выход из профиля /// </summary>
        public ICommand ExitProfileCommand => _ExitProfileCommand
               ??= new RelayCommand(OnExitProfileCommandExecuted, CanExitProfileCommandExecute);

        /// <summary> /// Выход из профиля /// </summary>
        public bool CanExitProfileCommandExecute(object? p) => true;

        /// <summary> /// Выход из профиля /// </summary>
        public void OnExitProfileCommandExecuted(object? p)
        {
            if (!_DialogService.Confirm("Вы уверены, что хотите выйти из аккаунта", "Выход")) return;
            User = null;
            CurrrentViewModel = _AuthorizationViewModel;
        }

        #endregion

        public MainWindowViewModel(AuthorizationViewModel authorizationViewModel,
            IUserDialogService dialogService,
            IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
            _DialogService = dialogService;
            _AuthorizationViewModel = authorizationViewModel;
            //чтобы создать авторизацию нам надо данный сервис а чтобы создать данный сервис надо авторизацию разрываем это кольцо
            _AuthorizationViewModel._MainWindowViewModel = this;
        }

    }
}
