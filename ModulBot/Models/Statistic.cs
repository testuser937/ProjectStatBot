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

        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; } // выдаваемое сообщение
        public string Query { get; set; } // sql-запрос
        public List<long> Subscribers { get; set; }
        public bool IsActive { get; set; }
    }
}
