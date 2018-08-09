using System;
using ModulBot.Attributes;
using System.Collections.Generic;
using ModulBot.Interfaces;
using ModulBot.Models;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ModulBot.Commands
{
    [NotShowInHelp]
    [Serializable]
    public class StatOnOff : ITool
    {
        public string Description { get; set; }
        public List<string> CommandsName { get; set; }
        public bool IsAdmin { get; set; }

        public async void Run(Message message)
        {                        
            Bot.TextOnMessageWithButtons = "Настройка статистик:";
            Bot.LastCommand = "statonoff";
            await Bot.BotClient.SendTextMessageAsync(message.Chat.Id,
                "Настройка статистик:", Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: new InlineKeyboardMarkup(Bot.GenerateStatButtons(false,message.Chat.Id)));
        }

        public StatOnOff()
        {
            Description = "Включение\\Выключение статистик";
            CommandsName = new List<string> { "/statonoff" };
            IsAdmin = true;
        }
    }
}
