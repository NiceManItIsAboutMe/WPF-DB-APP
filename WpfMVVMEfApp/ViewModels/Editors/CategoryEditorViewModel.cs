using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;

namespace WpfMVVMEfApp.ViewModels.Editors
{
    internal class CategoryEditorViewModel : ViewModel
    {
        internal class SelectedBook
        {
            public Book Book { get; set; }
            public bool IsSelected { get; set; } = false;

            public SelectedBook(Book book)
            {
                Book = book;
            }
        }

        #region Категория

        /// <summary> /// Категория /// </summary>
        private Category _Category;

        /// <summary> /// Категория /// </summary>
        public Category Category { get => _Category; set => Set(ref _Category, value); }

        #endregion

        #region Поиск

        /// <summary> /// Поиск /// </summary>
        private string _SearchFilter;

        /// <summary> /// Поиск /// </summary>
        public string SearchFilter
        {
            get => _SearchFilter;
            set
            {
                if (Set(ref _SearchFilter, value))
                    _BooksViewSource.View.Refresh();
            }
        }

        #endregion

        private CollectionViewSource _BooksViewSource;
        /// <summary>/// Отображение пользователей /// </summary>
        public ICollectionView BooksView => _BooksViewSource?.View;

        #region Книги

        /// <summary> /// Книги /// </summary>
        private ObservableCollection<SelectedBook> _Books;

        /// <summary> /// Книги /// </summary>
        public ObservableCollection<SelectedBook> Books
        {
            get => _Books;
            set
            {
                if (Set(ref _Books, value))
                {
                    _BooksViewSource = new CollectionViewSource
                    {
                        Source = Books,
                        SortDescriptions =
                        {
                            new SortDescription(nameof(SelectedBook.IsSelected), ListSortDirection.Descending),
                        }
                    };
                    _BooksViewSource.Filter += OnBooksViewSourceFilter;
                    OnPropertyChanged(nameof(BooksView));
                    _BooksViewSource.View.Refresh();
                }
            }
        }

        #endregion


        #region команда SaveChangesButtonClick Проверяем валидацию и добавляем выбранные книги в категорию

        /// <summary> /// SaveChangesButton /// </summary>
        private ICommand _SaveChangesCommand;

        /// <summary> /// SaveChangesButton /// </summary>
        public ICommand SaveChangesCommand => _SaveChangesCommand
               ??= new RelayCommand(OnSaveChangesCommandExecuted, CanSaveChangesCommandExecute);

        /// <summary> /// SaveChangesButton /// </summary>
        public bool CanSaveChangesCommandExecute(object? p)
        {
            var context = new ValidationContext(Category);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(Category, context, results, true)) return false;

            return true;
        }

        /// <summary> /// SaveChangesButton /// </summary>
        public void OnSaveChangesCommandExecuted(object? p)
        {
            if (Books == null) return;

            if (Category.Books == null) { Category.Books = new ObservableCollection<Book>(); }

            Category.Books.Clear();

            foreach (var item in Books.Where(c => c.IsSelected == true))
            {
                Category.Books.Add(item.Book);
            }

        }

        #endregion

        public CategoryEditorViewModel(Category category, ObservableCollection<Book>? books = null)
        {
            Category = category;

            if (books != null)
            {
                Books = new ObservableCollection<SelectedBook>();
                foreach (var item in books)
                {
                    Books.Add(new SelectedBook(item));
                }


                if (category.Books != null)
                {
                    foreach (var item in category.Books)
                    {
                        if (Books.Where(b => b.Book.Id == item.Id).FirstOrDefault() is SelectedBook book)
                            book.IsSelected = true;
                    }
                    _BooksViewSource?.View.Refresh();
                }
            }
        }

        private void OnBooksViewSourceFilter(object sender, FilterEventArgs e)
        {
            if (!(e.Item is SelectedBook book) || string.IsNullOrEmpty(SearchFilter)) return;

            if (!book.Book.Name.Contains(_SearchFilter, StringComparison.OrdinalIgnoreCase))
                e.Accepted = false;
        }
    }
}
