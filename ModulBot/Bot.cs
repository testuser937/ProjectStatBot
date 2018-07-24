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
using System.IO;

namespace ModulBot
{
    public class Bot
    {
        public static TelegramBotClient BotClient;

        private static List<List<InlineKeyboardButton>> statbuttons = new List<List<InlineKeyboardButton>>();
        public static List<List<InlineKeyboardButton>> StatButtons { get => statbuttons; set => statbuttons = value; }

        private static List<List<InlineKeyboardButton>> nextbuttons = new List<List<InlineKeyboardButton>>();
        public static List<List<InlineKeyboardButton>> NextButtons { get => nextbuttons; set => nextbuttons = value; }
        internal static List<ITool> Tools { get => tools; set => tools = value; }
        private static List<ITool> tools = new List<ITool>();
        public static int step = 0;
        public static bool IsDialogStart = false;
        public static string TextOnMessageWithButtons;

        public static void InitTimer()
        {
            var DailyTime = "00:39:00";
            var dateNow = DateTime.Now;
            var timeParts = DailyTime.Split(new char[1] { ':' });
            var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day,
                       int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));
            if (date <= dateNow)
            {
                SendStat.SendStatistic();
            }
            else
            {
                System.Threading.Timer timer = null;
                timer = new System.Threading.Timer(
                    o => { SendStat.SendStatistic(); timer.Dispose(); },
                    null,
                    date - dateNow,
                    TimeSpan.Zero);
            }
        }


        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (BotClient != null)
            {
                return BotClient;
            }

            BotClient = new TelegramBotClient(AppSettings.Key);

            var me = await BotClient.GetMeAsync();

            Console.WriteLine($"Hello! My name is {me.FirstName}");
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

        public static async void ButtonAction(string callbackInfo, long chatId, int messageId)
        {
            if (callbackInfo.Split(' ')[2].ToLower().Equals("false"))
                for (int i = 0; i < Bot.NextButtons.Count; i++)
                {
                    if (Bot.NextButtons[i][0].CallbackData.Equals(callbackInfo))
                    {
                        ActionButton.DoAction(chatId, messageId, callbackInfo.Split(' '));
                        return;
                    }
                }
            else
                for (int i = 0; i < Bot.StatButtons.Count; i++)
                {
                    if (Bot.StatButtons[i][0].CallbackData.Equals(callbackInfo))
                    {
                        ActionButton.ShowButtons(chatId, messageId, callbackInfo.Split(' '));
                        return;
                    }
                }

            await Bot.BotClient.SendTextMessageAsync(chatId, "Попробуйте заново ввести команду /tunestat(/statonoff - для админа)");
            return;
        }
    }
}