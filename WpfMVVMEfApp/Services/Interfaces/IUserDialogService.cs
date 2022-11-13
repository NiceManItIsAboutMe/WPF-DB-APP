using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMVVMEfApp.Models.PostgreSqlDB;

namespace WpfMVVMEfApp.Services.Interfaces
{
    internal interface IUserDialogService
    {
        /// <summary>
        /// Редактирование и добавление
        /// </summary>
        /// <param name="item">Сущность, которую редактируем или добавляем. Или ViewModel нужного Editor'а</param>
        /// <returns></returns>
        bool Edit(object item);

        bool OpenDialogWindow(object item);

        bool OpenFile(string Title, out string SelectedFile, string Filter = "Все файлы (*.*)|*.*");

        bool SaveFile(string Title, out string SelectedFile, string DefaultFileName = null, string Filter = "Все файлы (*.*)|*.*");

        /// <summary>
        /// Показать информацию
        /// </summary>
        /// <param name="Information"> Информация </param>
        /// <param name="Caption"> Заголовок </param>
        void ShowInformation(string Information, string Caption);

        /// <summary>
        /// Отобразить предупреждение
        /// </summary>
        /// <param name="Information"> Информация о предупреждении</param>
        /// <param name="Caption"> Заголовок </param>
        void ShowWarning(string Information, string Caption);

        /// <summary>
        /// Отобразить ошибку
        /// </summary>
        /// <param name="Information"> Информация об ошике</param>
        /// <param name="Caption"> Заголовок </param>
        void ShowError(string Information, string Caption);

        /// <summary>
        /// Подтверждение (Да,Нет)
        /// </summary>
        /// <param name="Message">Сообщение</param>
        /// <param name="Caption">Заголовок</param>
        /// <returns></returns>
        bool Confirm(string Message, string Caption);

    }
}
