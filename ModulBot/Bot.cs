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
using Microsoft.Extensions.Configuration;

namespace ModulBot
{
    public static class Bot
    {
        private static  TelegramBotClient BotClient;
        private static List<List<InlineKeyboardButton>> statbuttons = new List<List<InlineKeyboardButton>>();
        private static List<List<InlineKeyboardButton>> nextbuttons = new List<List<InlineKeyboardButton>>();
        private static List<ITool> tools = new List<ITool>();
        private static bool IsDialogStart = false;
        private static string TextOnMessageWithButtons;
        private static IConfiguration Configuration { get; }


        public static List<List<InlineKeyboardButton>> StatButtons { get => statbuttons; set => statbuttons = value; }        
        public static List<List<InlineKeyboardButton>> NextButtons { get => nextbuttons; set => nextbuttons = value; }
        internal static List<ITool> Tools { get => tools; set => tools = value; }
        public static TelegramBotClient BotClient1 { get => BotClient; set => BotClient = value; }
        public static bool IsDialogStart1 { get => IsDialogStart; set => IsDialogStart = value; }
        public static string TextOnMessageWithButtons1 { get => TextOnMessageWithButtons; set => TextOnMessageWithButtons = value; }

        


        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (BotClient1 != null)
            {
                return BotClient1;
            }

            BotClient1 = new TelegramBotClient(Configuration["Bot:Token"]);

            var me = await BotClient1.GetMeAsync();

            Console.WriteLine($"Hello! My name is {me.FirstName}");
            var baseInterfaceType = typeof(ITool);
            var botCommands = Assembly.GetAssembly(baseInterfaceType)
                .GetTypes()
                .Where(types => types.IsClass && !types.IsAbstract && types.GetInterface("ITool") != null);
            foreach (var botCommand in botCommands)
            {
                Tools.Add((ITool)Activator.CreateInstance(botCommand));
            }
            Program.MessageSender1.Start();
            return BotClient1;
        }

        public static async void ButtonAction(string callbackInfo, long chatId, int messageId)
        {
            if (callbackInfo.Split(' ')[2].ToLower().Equals("false"))
                for (int i = 0; i < Bot.NextButtons.Count; i++)
                {
                    if (Bot.NextButtons[i][0].CallbackData.Equals(callbackInfo))
                    {
                        ActionButton.DoAction(chatId, messageId, callbackInfo.Split(' '));
                    }
                }
            else
                for (int i = 0; i < Bot.StatButtons.Count; i++)
                {
                    if (Bot.StatButtons[i][0].CallbackData.Equals(callbackInfo))
                    {
                        ActionButton.ShowButtons(chatId, messageId, callbackInfo.Split(' '));
                    }
                }

            await Bot.BotClient1.SendTextMessageAsync(chatId, "Попробуйте заново ввести команду /tunestat(/statonoff - для админа)");
            return;
        }
    }
}