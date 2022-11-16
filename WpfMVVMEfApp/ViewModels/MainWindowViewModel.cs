using System.Windows.Input;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;

namespace WpfMVVMEfApp.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Поля
        #region User

        /// <summary> /// User /// </summary>
        private User _User;

        /// <summary> /// User /// </summary>
        public User User { get => _User; set { if (Set(ref _User, value)) IsAdmin = value.IsAdmin; }}

        #endregion

        #region IsAdmin

        /// <summary> /// IsAdmin /// </summary>
        private bool _IsAdmin = false;

        /// <summary> /// IsAdmin /// </summary>
        public bool IsAdmin { get => _IsAdmin; set => Set(ref _IsAdmin, value); }

        #endregion

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

        public MainWindowViewModel(AuthorizationViewModel authorizationViewModel)
        {
            _AuthorizationViewModel = authorizationViewModel;
            //чтобы создать авторизацию нам надо данный сервис а чтобы создать данный сервис надо авторизацию разрываем это кольцо
            _AuthorizationViewModel._MainWindowViewModel = this;
        }

    }
}
