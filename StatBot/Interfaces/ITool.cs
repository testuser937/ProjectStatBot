using System;
using System.Collections.Generic;
using Microsoft.Bot.Schema;

namespace StatBot.Interfaces
{
    interface ITool
    {
        string Description { get; set; }
        List<string> CommandsName { get; set; }
        bool IsAdmin { get; set; }
        Activity Run(Activity activity);
    }
}