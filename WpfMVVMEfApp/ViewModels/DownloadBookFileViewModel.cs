using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using WpfDBApp.ViewModels.Base;
using WpfMVVMEfApp.Commands.Base;
using WpfMVVMEfApp.Models;
using WpfMVVMEfApp.Models.PostgreSqlDB;
using WpfMVVMEfApp.Services.Interfaces;

namespace WpfMVVMEfApp.ViewModels
{
    internal class DownloadBookFileViewModel : ViewModel
    {
        private IDbContextFactory<ApplicationDbContext> _dbFactory;
        private IUserDialogService _DialogService;

        #region Книга

        /// <summary> /// Книга /// </summary>
        private Book _Book;

        /// <summary> /// Книга /// </summary>
        public Book Book { get => _Book; set => Set(ref _Book, value); }

        #endregion

        #region BookFileDescription Файлы

        /// <summary> /// BookFileDescription Файлы /// </summary>
        private ObservableCollection<BookFileDescription> _BookFiles;

        /// <summary> /// BookFileDescription Файлы /// </summary>
        public ObservableCollection<BookFileDescription> BookFiles { get => _BookFiles; set => Set(ref _BookFiles, value); }

        #endregion

        #region Выбранный файл

        /// <summary> /// Выбранный файл /// </summary>
        private BookFileDescription _SelectedFile;

        /// <summary> /// Выбранный файл /// </summary>
        public BookFileDescription SelectedFile { get => _SelectedFile; set => Set(ref _SelectedFile, value); }

        #endregion

        #region команда Скачивание книги

        /// <summary> /// Скачивание книги /// </summary>
        private ICommand _DownloadBookFileCommand;

        /// <summary> /// Скачивание книги /// </summary>
        public ICommand DownloadBookFileCommand => _DownloadBookFileCommand
               ??= new RelayCommand(OnDownloadBookFileCommandExecuted, CanDownloadBookFileCommandExecute);

        /// <summary> /// Скачивание книги /// </summary>
        public bool CanDownloadBookFileCommandExecute(object? p) => SelectedFile is BookFileDescription;

        /// <summary> /// Скачивание книги /// </summary>
        public async void OnDownloadBookFileCommandExecuted(object? p)
        {
            string path = string.Empty;
            if (!_DialogService.SaveFile("Скачивание файла", out path, SelectedFile.Name))
                return;
            using var db = await _dbFactory.CreateDbContextAsync();
            var file = await db.BookFiles.FirstAsync(f => f.BookFileDescriptionId == SelectedFile.Id);
            await File.WriteAllBytesAsync(path, file.File);
        }

        #endregion

        public DownloadBookFileViewModel(Book book,
            IDbContextFactory<ApplicationDbContext> dbFactory,
            IUserDialogService dialogService)
        {
            Book = book;
            BookFiles = new ObservableCollection<BookFileDescription>(book.BookFilesDescription);
            _dbFactory = dbFactory;
            _DialogService = dialogService;
        }

    }
}
