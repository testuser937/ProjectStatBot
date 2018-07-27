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
            Bot.StatButtons = new List<List<InlineKeyboardButton>>();
            var _builder = new StringBuilder("");
            for (var i = 0; i < DataModel.Statistics.Count; i++)
            {
                var stat = DataModel.Statistics[i];
                _builder.Append(stat.Name);
                if (!stat.IsActive)
                {
                    _builder.Append("(Off)");
                    stat.Name = _builder.ToString();
                    _builder.Clear();
                }                  
                    
                InlineKeyboardButton button = new InlineKeyboardButton() {
                    CallbackData = $"{stat.Id} {(int)Constants.ActionTypes.ShowTurn}" +
                    $" {Constants.ShowButtons}", Text = $"{stat.Id} {stat.Name}" };
                Bot.StatButtons.Add(new List<InlineKeyboardButton> { button });
            }
            Bot.TextOnMessageWithButtons = "Настройка статистик:";
            await Bot.BotClient.SendTextMessageAsync(message.Chat.Id,
                "Настройка статистик:", replyMarkup: new InlineKeyboardMarkup(Bot.StatButtons));
        }

        public StatOnOff()
        {
            Description = "Включение\\Выключение статистик";
            CommandsName = new List<string> { "/statonoff" };
            IsAdmin = true;
        }
    }
}
