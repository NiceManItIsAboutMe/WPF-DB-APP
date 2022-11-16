using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;

namespace WpfMVVMEfApp.ViewModels.AdminViewModels.Editors
{
    internal class AuthorEditorViewModel : ViewModel
    {


        #region Автор

        /// <summary> /// Автор /// </summary>
        private Author _SelectedAuthor;

        /// <summary> /// Автор /// </summary>
        public Author SelectedAuthor { get => _SelectedAuthor; set => Set(ref _SelectedAuthor, value); }

        #endregion


        #region команда Сохранение. Проверка валидатора

        /// <summary> /// Сохранение. Проверка валидатора /// </summary>
        private ICommand _SaveChangesCommand;

        /// <summary> /// Сохранение. Проверка валидатора /// </summary>
        public ICommand SaveChangesCommand => _SaveChangesCommand
               ??= new RelayCommand(OnSaveChangesCommandExecuted, CanSaveChangesCommandExecute);

        /// <summary> /// Сохранение. Проверка валидатора /// </summary>
        public bool CanSaveChangesCommandExecute(object? p)
        {
            var context = new ValidationContext(SelectedAuthor);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(SelectedAuthor, context, results, true)) return false;

            return true;
        }

        /// <summary> /// Сохранение. Проверка валидатора /// </summary>
        public void OnSaveChangesCommandExecuted(object? p)
        {
        }

        #endregion

        public AuthorEditorViewModel(Author author)
        {
            SelectedAuthor = author;
        }

    }
}
