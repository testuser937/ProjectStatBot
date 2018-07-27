using System.Collections.Generic;

namespace ModulBot.Models
{
    public class Statistic
    {
        public Statistic(string name, string message, string query, bool isActive)
        {
            Name = name;
            Message = message;
            Query = query;
            Subscribers = new List<long>();
            IsActive = isActive;
        }

        public int Id { get; set; } //id в БД
        public string Name { get; set; } //название
        public string Message { get; set; } // выдаваемое сообщение
        public string Query { get; set; } // sql-запрос
        public List<long> Subscribers { get; set; } // подписчики
        public bool IsActive { get; set; } //включена ли статистика
    }
}
