using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModulBot.Cards;
using Telegram.Bot;
using Telegram.Bot.Types;
using ModulBot.Interfaces;
using System.Reflection;
using Telegram.Bot.Types.ReplyMarkups;
using ModulBot.Models;
using System.Text;

namespace ModulBot
{
    public static class Bot
    {
        private static TelegramBotClient botClient;
        private static List<List<InlineKeyboardButton>> statbuttons = new List<List<InlineKeyboardButton>>();
        private static List<List<InlineKeyboardButton>> nextbuttons = new List<List<InlineKeyboardButton>>();
        private static List<ITool> tools = new List<ITool>();
        private static bool isDialogStart = false;
        private static string textOnMessageWithButtons;
        private static bool isSetAdminStart = false;
        private static string lastCommand;

        public static List<List<InlineKeyboardButton>> StatButtons { get => statbuttons;
            set => statbuttons = value; }        
        public static List<List<InlineKeyboardButton>> NextButtons { get => nextbuttons;
            set => nextbuttons = value; }
        internal static List<ITool> Tools { get => tools; set => tools = value; }
        public static TelegramBotClient BotClient { get => botClient; set => botClient = value; }
        public static bool IsDialogStart { get => isDialogStart; set => isDialogStart = value; }
        public static string TextOnMessageWithButtons { get => textOnMessageWithButtons;
            set => textOnMessageWithButtons = value; }
        public static bool IsSetAdminStart { get => isSetAdminStart; set => isSetAdminStart = value; }
        public static string LastCommand { get => lastCommand; set => lastCommand = value; }
        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (BotClient != null)
            {
                return BotClient;
            }

            BotClient = new TelegramBotClient(Startup.GetToken());

            var baseInterfaceType = typeof(ITool);
            var botCommands = Assembly.GetAssembly(baseInterfaceType)
                .GetTypes()
                .Where(types => types.IsClass && !types.IsAbstract && types.GetInterface("ITool") != null);
            foreach (var botCommand in botCommands)
            {
                Tools.Add((ITool)Activator.CreateInstance(botCommand));
            }
            Program.MessageSender.Start();
            return BotClient;
        }

        public static async Task ButtonAction(string callbackInfo, long chatId, int messageId)
        {
            if (callbackInfo.Split(' ')[2].ToLower().Equals("false"))
                for (int i = 0; i < Bot.NextButtons.Count; i++)
                {
                    if (Bot.NextButtons[i][0].CallbackData.Equals(callbackInfo))
                    {
                        await ActionButton.DoAction(chatId, messageId, callbackInfo.Split(' '));
                        return;
                    }
                }
            else
                for (int i = 0; i < Bot.StatButtons.Count; i++)
                {
                    if (Bot.StatButtons[i][0].CallbackData.Equals(callbackInfo))
                    {
                        await ActionButton.ShowButtons(chatId, messageId, callbackInfo.Split(' '));
                        return;
                    }
                }

            await Bot.BotClient.SendTextMessageAsync(chatId, "Попробуйте заново ввести команду /tunestat(/statonoff - для админа)");
        }

        public static List<List<InlineKeyboardButton>> GenerateStatButtons(bool isTunestat, long userId)
        {
            StatButtons = new List<List<InlineKeyboardButton>>();
            if (isTunestat)
            {
                for (var i = 0; i < DataModel.Statistics.Count; i++)
                {
                    var _builder = new StringBuilder("");
                    var stat = DataModel.Statistics[i];
                    _builder.Append(stat.Name);                    
                    if (stat.IsActive)
                    {
                        if (stat.Subscribers.Contains(userId))
                        {
                            _builder.Append("(sub)");
                            stat.Name = _builder.ToString();
                            _builder.Clear();
                        }

                        InlineKeyboardButton button = new InlineKeyboardButton()
                        {
                            CallbackData = $"{stat.Id} {(int)Constants.ActionTypes.ShowSubs}" +
                            $" {Constants.ShowButtons}",
                            Text = stat.Name
                        };
                        // 1. Номер статистики, за которую отвечает кнопка 2.Номер действия 3.Тип кнопки                 
                        StatButtons.Add(new List<InlineKeyboardButton> { button });
                    }
                }
            }
            else
            {
                for (var i = 0; i < DataModel.Statistics.Count; i++)
                {
                    var _builder = new StringBuilder("");
                    var stat = DataModel.Statistics[i];
                    _builder.Append(stat.Name);
                    if (!stat.IsActive)
                    {
                        _builder.Append("(Off)");
                        stat.Name = _builder.ToString();
                        _builder.Clear();
                    }

                    InlineKeyboardButton button = new InlineKeyboardButton()
                    {
                        CallbackData = $"{stat.Id} {(int)Constants.ActionTypes.ShowTurn}" +
                        $" {Constants.ShowButtons}",
                        Text = $"{stat.Id} {stat.Name}"
                    };
                    StatButtons.Add(new List<InlineKeyboardButton> { button });
                }
            }
            return StatButtons;
        }
    }
}