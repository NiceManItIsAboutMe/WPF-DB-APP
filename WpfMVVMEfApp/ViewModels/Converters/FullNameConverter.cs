using System;
using System.Globalization;
using System.Windows.Data;
using WpfMVVMEfApp.Models.Base;

namespace WpfMVVMEfApp.ViewModels.Converters
{
    /// <summary>  ///  Конвертирует объект в полное имя  /// </summary>
    internal class FullNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Person person)) return value;
            return person.Surname + " " + person.Name + " " + person.Patronymic;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
