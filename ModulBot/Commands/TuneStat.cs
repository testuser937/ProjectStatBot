using ModulBot.Interfaces;
using ModulBot.Models;
using System.Collections.Generic;
using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
namespace ModulBot.Commands
{
    [Serializable]
    public class TuneStat : ITool
    {
        public string Description { get; set; }
        public List<string> CommandsName { get; set; }
        public bool IsAdmin { get; set; }

        public async void Run(Message message)
        {
            Bot.TextOnMessageWithButtons = "Список статистик: ";
            Bot.LastCommand = "tunestat";
            await Bot.BotClient.SendTextMessageAsync(message.Chat.Id,
                Bot.TextOnMessageWithButtons, replyMarkup: new InlineKeyboardMarkup(Bot.GenerateStatButtons(true,message.Chat.Id)));
        }

        public TuneStat()
        {
            Description = "Настройка статистик";
            CommandsName = new List<string> { "/tunestat" };
            IsAdmin = false;
        }
    }
}