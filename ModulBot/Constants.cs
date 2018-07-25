using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ModulBot
{
    public class Constants
    {
        public enum ActionTypes
        {   /// <summary>
            /// Включение статистики админом
            /// </summary>
            TurnOn = 1,
            /// <summary>
            /// Выключение статистики админом
            /// </summary>
            TurnOff = 2,
            /// <summary>
            /// Подписка на статистику
            /// </summary>
            Subscribe = 3,
            /// <summary>
            /// Отписка от статистики
            /// </summary>
            Unsubscribe = 4,
            /// <summary>
            /// Отображение новых кнопок - включить\выключить статистику
            /// </summary>
            ShowTurn = 5,
            /// <summary>
            /// Отображение новых кнопок - подписаться\отписаться от статистики
            /// </summary>
            ShowSubs = 6,
            /// <summary>
            /// Вернуться на уровень вверх
            /// </summary>
            Back = 7,
            /// <summary>
            /// Получить выбранную статистику
            /// </summary>
            GetStat = 8
        }

        /// <summary>
        /// Надпись отображающаяся на кнопке, отвечающая за включение статистики
        /// </summary>
        public const string TurnOn = "Включить статистику";

        /// <summary>
        /// Надпись отображающаяся на кнопке, отвечающая за выключение статистики
        /// </summary>
        public const string TurnOff = "Выключить статистику";

        /// <summary>
        /// Надпись отображающаяся на кнопке, отвечающая за подписку на статистику
        /// </summary>
        public const string Subscribe = "Подписаться";

        /// <summary>
        /// Надпись отображающаяся на кнопке, отвечающая за отписку от статистики
        /// </summary>
        public const string Unsubscribe = "Отписаться";

        /// <summary>
        /// Надпись отображающаяся на кнопке, возвращающая на уровень вверх
        /// </summary>
        public const string Back = "Назад";

        /// <summary>
        /// Надпись отображающаяся на кнопке, возвращающаю пользователю статистику
        /// </summary>
        public const string GetStat = "Получить статистику";

        /// <summary>
        /// Кнопка при нажатии на нее будет выполнять команду
        /// </summary>
        public const bool NotShowButtons = false;

        /// <summary>
        /// Кнопка при нажатии на нее будет отображать другие кнопки
        /// </summary>
        public const bool ShowButtons = true;
    }
}
