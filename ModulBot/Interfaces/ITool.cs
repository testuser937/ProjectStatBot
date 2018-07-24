using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ModulBot.Interfaces
{
    interface ITool
    {
        string Description { get; set; }
        List<string> CommandsName { get; set; }
        bool IsAdmin { get; set; }
        void Run(Message message);
    }
}