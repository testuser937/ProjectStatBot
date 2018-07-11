using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatBot
{
    public class Statistic
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; } // выдаваемое сообщение
        public string Query { get; set; } // sql-запрос
        public List<string> Subscribers { get; set; }
        public bool IsActive { get; set; }
    }
}
