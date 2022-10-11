using WpfDBApp.ViewModels.Base;

namespace WpfMVVMEfApp.ViewModels
{
    internal class MainWindowViewModel:ViewModel
    {
        #region Заголовок
        private string _Title = "Библиотека";

        public string Title { get => _Title; set=> Set(ref _Title, value); }
        #endregion

    }
}
