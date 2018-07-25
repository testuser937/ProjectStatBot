using ModulBot.Interfaces;
using ModulBot.Models;
using System;
using System.Collections.Generic;
using System.Text;
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
            StringBuilder StatNames = new StringBuilder("");
            foreach (var stat in DataModel.Statistics)
            {
                if (stat.IsActive)
                    StatNames.Append($"{stat.Name}\n");
            }
            if (StatNames.Length != 0)
                await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, StatNames.ToString());
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
