using System;
using ModulBot.Attributes;
using System.Collections.Generic;
using ModulBot.Interfaces;
using ModulBot.Models;
using ModulBot.Cards;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;

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
            Bot.StatButtons = new List<List<InlineKeyboardButton>>();
            for (var i = 0; i < DataModel.Statistics.Count; i++)
            {
                var stat = DataModel.Statistics[i];
                if (!stat.IsActive)
                    stat.Name += " (Off)";
                InlineKeyboardButton button = new InlineKeyboardButton() { CallbackData = $"{stat.Id} {(int)Constants.ActionTypes.ShowTurn} {Constants.ShowButtons}", Text = $"{stat.Id} {stat.Name}" };
                Bot.StatButtons.Add(new List<InlineKeyboardButton> { button }); ;
            }
            Bot.TextOnMessageWithButtons = "Настройка статистик:";
            await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "Настройка статистик:", replyMarkup: new InlineKeyboardMarkup(Bot.StatButtons));
        }

        public StatOnOff()
        {
            Description = "Включение\\Выключение статистик";
            CommandsName = new List<string> { "/statonoff" };
            IsAdmin = true;
        }
    }
}
