using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;

namespace WpfMVVMEfApp.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region Поля
        private ApplicationContext _db;
        #region Заголовок
        private string _Title = "Библиотека";

        public string Title { get => _Title; set => Set(ref _Title, value); }
        #endregion

        #region Выбранная категория

        /// <summary> /// Выбранная категория /// </summary>
        private Category _SelectedCategory;

        /// <summary> /// Выбранная категория /// </summary>
        public Category SelectedCategory 
        { get => _SelectedCategory;
            set
            {
                if (AddBooksInCategoriesCommand.CanExecute(value))
                    AddBooksInCategoriesCommand.Execute(value);

                Set(ref _SelectedCategory, value);
            }
        }

        #endregion

        #region Выбранный автор

        /// <summary> /// Выбранный автор /// </summary>
        private Author _SelectedAuthor;

        /// <summary> /// Выбранный автор /// </summary>
        public Author SelectedAuthor { get => _SelectedAuthor; set => Set(ref _SelectedAuthor, value); }

        #endregion

        #region Категории

        /// <summary> /// Книги /// </summary>
        private ObservableCollection<Category> _Categories;

        /// <summary> /// Книги /// </summary>
        public ObservableCollection<Category> Categories { get => _Categories; set => Set(ref _Categories, value); }

        #endregion

        #region Авторы

        /// <summary> /// Авторы /// </summary>
        private ObservableCollection<Author> _Authors;

        /// <summary> /// Авторы /// </summary>
        public ObservableCollection<Author> Authors { get => _Authors; set => Set(ref _Authors, value); }

        #endregion

        #endregion

        #region Команды

        #region команда Добавления книг в категории при выборе категории

        /// <summary> /// Добавления книг в коллекцию при выборе категории /// </summary>
        public ICommand AddBooksInCategoriesCommand;

        /// <summary> /// Добавления книг в коллекцию при выборе категории /// </summary>
        public bool CanAddBooksInCategoriesCommandExecute(object? p)
        {
            if (p == null) return false;
            if (((Category) p).Books != null) return false;

            return true;
        }

        /// <summary> /// Добавления книг в коллекцию при выборе категории /// </summary>
        public void OnAddBooksInCategoriesCommandExecuted(object? p)
        {
            ((Category)p).Books = _db.Categories.Where(c => c.Id == ((Category)p).Id).Select(c => c.Books).FirstOrDefault();
        }

        #endregion

        #endregion

        public MainWindowViewModel(ApplicationContext db)
        {
            _db = db;
            Categories = new ObservableCollection<Category>(db.Categories.ToArray());
            Authors = new ObservableCollection<Author>(db.Authors.ToArray());
            AddBooksInCategoriesCommand = new RelayCommand(OnAddBooksInCategoriesCommandExecuted, CanAddBooksInCategoriesCommandExecute);
        }

    }
}
