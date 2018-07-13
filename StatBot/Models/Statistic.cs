using System.Collections.Generic;

namespace StatBot
{
    public class Statistic
    {
        public Statistic(string name, string message, string query)
        {
            Name = name;
            Message = message;
            Query = query;
            Subscribers = new List<string>();
            IsActive = true;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; } // выдаваемое сообщение
        public string Query { get; set; } // sql-запрос
        public List<string> Subscribers { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
