using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;

namespace WpfMVVMEfApp.ViewModels.Editors
{
    internal class UserEditorViewModel : ViewModel
    {

        #region Пользователь

        /// <summary> /// Пользователь /// </summary>
        private User _User;

        /// <summary> /// Пользователь /// </summary>
        public User User { get => _User; set => Set(ref _User, value); }

        #endregion


        #region Пароль

        /// <summary> /// Пароль /// </summary>
        private string _Password;

        /// <summary> /// Пароль /// </summary>
        public string Password { get => _Password; set => Set(ref _Password, value); }

        #endregion

        #region команда SaveChangesButtonClick сохранить изменения

        /// <summary> /// SaveChangesButtonClick сохранить изменения /// </summary>
        private ICommand _SaveChangesCommand;

        /// <summary> /// SaveChangesButtonClick сохранить изменения /// </summary>
        public ICommand SaveChangesCommand => _SaveChangesCommand
               ??= new RelayCommand(OnSaveChangesCommandExecuted, CanSaveChangesCommandExecute);

        /// <summary> /// SaveChangesButtonClick сохранить изменения /// </summary>
        public bool CanSaveChangesCommandExecute(object? p)
        {
            var context = new ValidationContext(User);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(User, context, results, true)) return false;

            if ( (User.Password==null || User.Password?.Length==0) 
                && (String.IsNullOrEmpty(Password) || Password?.Length < 6))
                return false;

            if (!String.IsNullOrEmpty(Password) && Password?.Length < 6) return false;

            return true;
        }

        /// <summary> /// SaveChangesButtonClick сохранить изменения /// </summary>
        public void OnSaveChangesCommandExecuted(object? p)
        {
            if (String.IsNullOrEmpty(Password)) return;

            User.Password = User.HashPassword(Password);
        }

        #endregion

        public UserEditorViewModel(User user)
        {
            User = user;
        }
    }
}
