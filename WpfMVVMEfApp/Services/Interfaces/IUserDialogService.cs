using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMVVMEfApp.Services.Interfaces
{
    internal interface IUserDialogService
    {
        /// <summary>
        /// Редактирование и добавление
        /// </summary>
        /// <param name="item">Сущность, которую редактируем или добавляем</param>
        /// <returns></returns>
        bool Edit(object item);

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
