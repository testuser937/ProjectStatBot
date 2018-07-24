using ModulBot.Interfaces;
using ModulBot.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ModulBot.Commands
{
    [Serializable]
    public class Stats : ITool
    {
        public string Description { get; set; }
        public List<string> CommandsName { get; set; }
        public bool IsAdmin { get; set; }

        public async void Run(Message message)
        {
            string StatNames = "";
            foreach (var stat in DataModel.Statistics)
            {
                if (stat.IsActive)
                    StatNames += $"{stat.Name}\n";
            }
            if (StatNames != "")
                await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, StatNames);
            else
                await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "Нет доступных статистик");
        }

        public Stats()
        {
            Description = "Выводит список статистик";
            CommandsName = new List<string> { "/stats" };
            IsAdmin = false;
        }
    }
}
