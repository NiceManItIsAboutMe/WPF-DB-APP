using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using static System.Reflection.Metadata.BlobBuilder;

namespace WpfMVVMEfApp.ViewModels.Editors
{
    internal class BookEditorViewModel : ViewModel
    {
        internal class SelectedCategory
        {
            public bool IsSelected { get; set; } = false;
            public Category Category { get; set; }

            public SelectedCategory(Category category)
            {
                Category = category;
            }
        }

        #region Книга

        /// <summary> /// Книга /// </summary>
        private Book _Book;

        /// <summary> /// Книга /// </summary>
        public Book Book { get => _Book; set => Set(ref _Book, value); }

        #endregion

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
                    _CategoriesViewSource.View.Refresh();
            }
        }

        #endregion



        private CollectionViewSource _CategoriesViewSource;

        /// <summary>/// Отображение пользователей /// </summary>
        public ICollectionView CategoriesView => _CategoriesViewSource?.View;

        #region Категории

        /// <summary> /// Категории /// </summary>
        private ObservableCollection<SelectedCategory> _Categories;

        /// <summary> /// Категории /// </summary>
        public ObservableCollection<SelectedCategory> Categories
        {
            get => _Categories;
            set
            {
                if (Set(ref _Categories, value))
                {
                    _CategoriesViewSource = new CollectionViewSource
                    {
                        Source = Categories,
                        SortDescriptions =
                        {
                            new SortDescription(nameof(SelectedCategory.IsSelected), ListSortDirection.Descending),
                        }
                    };
                    _CategoriesViewSource.Filter += OnCategoriesViewSourceFilter;
                    OnPropertyChanged(nameof(CategoriesView));
                    _CategoriesViewSource.View.Refresh();
                }
            }
        }
        #endregion

        #region Авторы

        /// <summary> /// Авторы /// </summary>
        private ObservableCollection<Author> _Authors;

        /// <summary> /// Авторы /// </summary>
        public ObservableCollection<Author> Authors { get => _Authors; set => Set(ref _Authors, value); }

        #endregion


        #region команда ChangeBookCategories SaveButtonClick Изменяем выбранные категории в книге

        /// <summary> /// SaveButtonClick Изменяем выбранные категории в книге /// </summary>
        private ICommand _ChangeBookCategoriesCommand;

        /// <summary> /// SaveButtonClick Изменяем выбранные категории в книге /// </summary>
        public ICommand ChangeBookCategoriesCommand => _ChangeBookCategoriesCommand
               ??= new RelayCommand(OnChangeBookCategoriesCommandExecuted, CanChangeBookCategoriesCommandExecute);

        /// <summary> /// Проверям валидаторы/// </summary>
        public bool CanChangeBookCategoriesCommandExecute(object? p)
        {
            var context = new ValidationContext(Book);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(Book, context, results, true)) return false;

            return true;
        }


        /// <summary> /// SaveButtonClick Изменяем выбранные категории в книге /// </summary>
        public void OnChangeBookCategoriesCommandExecuted(object? p)
        {
            if (Categories == null) return;

            if (Book.Categories == null) Book.Categories = new ObservableCollection<Category>();

            Book.Categories.Clear();
            foreach (var item in Categories.Where(c => c.IsSelected == true))
            {
                Book.Categories.Add(item.Category);
            }

        }

        #endregion

        public BookEditorViewModel(Book book, ObservableCollection<Category>? categories, ObservableCollection<Author>? authors)
        {
            Authors = authors;

            Book = book;

            if (categories != null)
            {
                Categories = new ObservableCollection<SelectedCategory>();
                foreach (var item in categories)
                {
                    Categories.Add(new SelectedCategory(item));
                }

                //заполняем коллекцию категорий, к которым относится данная книга
                if (Book.Categories != null)
                {
                    foreach (var item in Book.Categories)
                    {
                        if (Categories.Where(c => c.Category.Id == item.Id).FirstOrDefault() is SelectedCategory category)
                            category.IsSelected = true;
                    }
                    _CategoriesViewSource?.View.Refresh();
                }
            }

        }

        private void OnCategoriesViewSourceFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is SelectedCategory category) || string.IsNullOrEmpty(SearchFilter)) return;

            if (!category.Category.Name.Contains(_SearchFilter, StringComparison.OrdinalIgnoreCase))
                e.Accepted = false;
        }
    }
}
