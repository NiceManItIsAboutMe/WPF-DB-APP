using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using static System.Reflection.Metadata.BlobBuilder;

namespace WpfMVVMEfApp.ViewModels.Editors
{
    internal class BookEditorViewModel:ViewModel
    {
        internal class SelectedCategory : Category
        {
            public bool IsSelected { get; set; } = false;

            public SelectedCategory(Category category)
            {
                Books = category.Books;
                Name = category.Name;
                Id = category.Id;
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
        public ObservableCollection<SelectedCategory> Categories { get => _Categories;
            set 
            { 
                if(Set(ref _Categories, value))
                {
                    _CategoriesViewSource = new CollectionViewSource
                    {
                        Source = Categories,
                        SortDescriptions =
                        {
                            new SortDescription(nameof(Book.Name), ListSortDirection.Ascending),
                        }
                    };
                    _CategoriesViewSource.Filter += OnCategoriesViewSourceFilter;
                    OnPropertyChanged(nameof(CategoriesView));
                    _CategoriesViewSource.View.Refresh();
                } 
            } }
        #endregion

        #region Авторы

        /// <summary> /// Авторы /// </summary>
        private ObservableCollection<Author> _Authors;

        /// <summary> /// Авторы /// </summary>
        public ObservableCollection<Author> Authors { get => _Authors; set => Set(ref _Authors, value); }

        #endregion


        public BookEditorViewModel(Book book, ObservableCollection<Category>? categories, ObservableCollection<Author>? authors)
        {
            Categories = new ObservableCollection<SelectedCategory>();
            foreach (var item in categories)
            {
                Categories.Add(new SelectedCategory(item));
            }
            Authors = authors;

            Book = book;

            //заполняем коллекцию категорий, к которым относится данная книга
            foreach (var item in Book.Category)
            {
                if (Categories.Where(c => c.Id == item.Id).FirstOrDefault() is SelectedCategory category)
                    category.IsSelected = true;
            }
        }

        private void OnCategoriesViewSourceFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is SelectedCategory category) || string.IsNullOrEmpty(SearchFilter)) return;

            if (!category.Name.Contains(_SearchFilter, StringComparison.OrdinalIgnoreCase))
                e.Accepted = false;
        }
    }
}
