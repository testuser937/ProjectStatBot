using ModulBot.Interfaces;
using ModulBot.Models;
using System.Collections.Generic;
using ModulBot.Cards;
using System;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;
using ModulBot;
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
            Bot.StatButtons = new List<List<InlineKeyboardButton>>();
            for (var i = 0; i < DataModel.Statistics.Count; i++)
            {
                var stat = DataModel.Statistics[i];
                if (stat.IsActive)
                {
                    InlineKeyboardButton button = new InlineKeyboardButton() { CallbackData = $"{stat.Id} {(int)Constants.ActionTypes.ShowSubs} {Constants.ShowButtons}", Text = stat.Name };
                    // 1. Номер статистики, за которую отвечает кнопка 2.Номер действия 3.Тип кнопки                 
                    Bot.StatButtons.Add(new List<InlineKeyboardButton> { button }); ;
                }
            }
            Bot.TextOnMessageWithButtons = "Список статистик: ";
            await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, Bot.TextOnMessageWithButtons, replyMarkup: new InlineKeyboardMarkup(Bot.StatButtons));
        }

        public TuneStat()
        {
            Description = "Настройка статистик";
            CommandsName = new List<string> { "/tunestat" };
            IsAdmin = false;
        }
    }
}