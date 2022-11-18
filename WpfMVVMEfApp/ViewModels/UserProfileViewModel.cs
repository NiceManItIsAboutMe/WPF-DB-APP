using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;

namespace WpfMVVMEfApp.ViewModels
{
    internal class UserProfileViewModel : ViewModel
    {
        private IDbContextFactory<ApplicationDbContext> _dbFactory;

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

        #region команда Отменить изменения

        /// <summary> /// Отменить изменения /// </summary>
        private ICommand _CancelChangesCommand;

        /// <summary> /// Отменить изменения /// </summary>
        public ICommand CancelChangesCommand => _CancelChangesCommand
               ??= new RelayCommand(OnCancelChangesCommandExecuted, CanCancelChangesCommandExecute);

        /// <summary> /// Отменить изменения /// </summary>
        public bool CanCancelChangesCommandExecute(object? p) => true;

        /// <summary> /// Отменить изменения /// </summary>
        public async void OnCancelChangesCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            User = await db.Users.Include(u => u.Books).FirstAsync(u => u.Id == User.Id);
        }

        #endregion

        #region команда Сохранить изменения

        /// <summary> /// Сохранить изменения /// </summary>
        private ICommand _SaveChangesCommand;

        /// <summary> /// Сохранить изменения /// </summary>
        public ICommand SaveChangesCommand => _SaveChangesCommand
               ??= new RelayCommand(OnSaveChangesCommandExecuted, CanSaveChangesCommandExecute);

        /// <summary> /// Сохранить изменения /// </summary>
        public bool CanSaveChangesCommandExecute(object? p)
        {
            var context = new ValidationContext(User);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(User, context, results, true)) return false;

            if (!string.IsNullOrEmpty(Password) && Password?.Length < 6) return false;

            return true;
        }

        /// <summary> /// Сохранить изменения /// </summary>
        public async void OnSaveChangesCommandExecuted(object? p)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            if (!string.IsNullOrEmpty(Password))
                User.Password = User.HashPassword(Password);
            db.Update(User);
            await db.SaveChangesAsync();
        }

        #endregion

        public UserProfileViewModel(User user, IDbContextFactory<ApplicationDbContext> dbFactory)
        {
            User = user;
            _dbFactory = dbFactory;
        }
    }
}
