using System;
using System.Collections.Generic;
using Microsoft.Bot.Schema;
using StatBot.Attributes;
using StatBot.Interfaces;

namespace StatBot.Commands
{
    [NotShowInHelp]
    [Serializable]
    public class CreateStat : ITool
    {
        public string Description { get; set; }
        public List<string> CommandsName { get; set; }
        public bool IsAdmin { get; set; }        

        public Activity Run(Activity activity)
        {
            throw new System.NotImplementedException();
        }

        public CreateStat()
        {
            CommandsName = new List<string>() { "/createstat" };
            Description = "Создание статистики";
        }
    }
}
