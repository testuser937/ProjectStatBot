﻿using System;
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
        private static  TelegramBotClient botClient;
        private static List<List<InlineKeyboardButton>> statbuttons = new List<List<InlineKeyboardButton>>();
        private static List<List<InlineKeyboardButton>> nextbuttons = new List<List<InlineKeyboardButton>>();
        private static List<ITool> tools = new List<ITool>();
        private static bool isDialogStart = false;
        private static string textOnMessageWithButtons;
        private static IConfiguration Configuration { get; }


        public static List<List<InlineKeyboardButton>> StatButtons { get => statbuttons; set => statbuttons = value; }        
        public static List<List<InlineKeyboardButton>> NextButtons { get => nextbuttons; set => nextbuttons = value; }
        internal static List<ITool> Tools { get => tools; set => tools = value; }
        public static TelegramBotClient BotClient { get => botClient; set => botClient = value; }
        public static bool IsDialogStart { get => isDialogStart; set => isDialogStart = value; }
        public static string TextOnMessageWithButtons { get => textOnMessageWithButtons; set => textOnMessageWithButtons = value; }

        


        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (BotClient != null)
            {
                return BotClient;
            }

            BotClient = new TelegramBotClient(Configuration["Bot:Token"]);

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

            await Bot.BotClient.SendTextMessageAsync(chatId, "Попробуйте заново ввести команду /tunestat(/statonoff - для админа)");
        }
    }
}