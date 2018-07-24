using System;
using System.Collections.Generic;
using ModulBot.Interfaces;
using ModulBot.Models;
using ModulBot.Database.PostgresRepositories;
using ModulBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;
using Npgsql;
using ModulBot.Database;

namespace ModulBot.Cards
{
    public static class ActionButton
    {
        public static async void DoAction(long chatId, int messageId, string[] buttonCallbackData)
        {
            int statId = Convert.ToInt32(buttonCallbackData[0]);
            int actionType = Convert.ToInt32(buttonCallbackData[1]);
            var bd = new PostgresStatsRepository();
            var stat = bd.GetById(statId);
            string text = "";

            switch (actionType)
            {
                case ((int)Constants.ActionTypes.TurnOn):
                    {
                        if (!stat.IsActive)
                        {
                            stat.IsActive = true;
                            text = $"Cтатистика №{statId} включена.";
                            bd.Update(stat);
                        }
                        else
                            text = $"Cтатистика №{statId} уже включена";
                        break;
                    }
                case ((int)Constants.ActionTypes.TurnOff):
                    {
                        if (stat.IsActive)
                        {
                            stat.IsActive = false;
                            text = $"Cтатистика №{statId} отключена.";
                            bd.Update(stat);
                        }
                        else
                            text = $"Cтатистика №{statId} уже выключена";
                        break;
                    }

                case ((int)Constants.ActionTypes.Subscribe):
                    {
                        if (!stat.Subscribers.Contains(chatId))
                        {
                            stat.Subscribers.Add(chatId);
                            bd.Update(stat);
                            text = $"Вы подписались на статистикиу";
                        }
                        else
                            text = "Вы уже подписаны на эту статистикиу";
                        break;
                    }

                case ((int)Constants.ActionTypes.Unsubscribe):
                    {
                        if (stat.Subscribers.Contains(chatId))
                        {
                            stat.Subscribers.Remove(chatId);
                            bd.Update(stat);
                            text = $"Вы отписались от статистики";
                        }
                        else
                            text = "Вы уже отписаны от этой статистики";
                        break;
                    }
                case ((int)Constants.ActionTypes.GetStat):
                    {
                        int count = 0;
                        var db = new PostgresStatsRepository();
                        var selected_stat = db.GetById(statId);

                        try
                        {
                            using (var conn = new NpgsqlConnection(Constants.ConnectionString))
                            {
                                conn.Open();
                                string query = selected_stat.Query;
                                var cmd = new NpgsqlCommand(query, conn);
                                // Execute the query and obtain a result set
                                NpgsqlDataReader dr = cmd.ExecuteReader();
                                while (dr.Read())
                                    count += 1;
                            }
                            // await Bot.BotClient.SendTextMessageAsync(chatId, string.Format(selected_stat.Message, count));

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        //text = $"Статистика пользователя {chatId} :\n\r{stat.Message}";
                        text = string.Format(selected_stat.Message, count);
                        break;
                    }
                case ((int)Constants.ActionTypes.Back):
                    {
                        TuneStat tuneStat = new TuneStat();

                        await Bot.BotClient.EditMessageTextAsync(new ChatId(chatId), messageId, Bot.TextOnMessageWithButtons, replyMarkup: new InlineKeyboardMarkup(Bot.StatButtons));
                        return;
                        //break;
                    }
            }
            bd.Save();
            await Bot.BotClient.SendTextMessageAsync(chatId, text);
        }

        public static async void ShowButtons(long chatId, int messageId, string[] buttonCallbackData)
        {
            int statId = Convert.ToInt32(buttonCallbackData[0]);
            int actionType = Convert.ToInt32(buttonCallbackData[1]);

            Bot.NextButtons = new List<List<InlineKeyboardButton>>();
            if (actionType == (int)Constants.ActionTypes.ShowTurn)
            {
                Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.TurnOn} {Constants.NotShowButtons}", Text = Constants.TurnOn } });
                Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.TurnOff} {Constants.NotShowButtons}", Text = Constants.TurnOff } });
                Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.Back} {Constants.NotShowButtons}", Text = Constants.Back } });
            }
            else //if (_buttonSettings.ActionType == (int)Constants.ActionTypes.ShowSubs)
            {
                Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.Subscribe} {Constants.NotShowButtons}", Text = Constants.Subscribe } });
                Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.Unsubscribe} {Constants.NotShowButtons}", Text = Constants.Unsubscribe } });
                Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.GetStat} {Constants.NotShowButtons}", Text = Constants.GetStat } });
                Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.Back} {Constants.NotShowButtons}", Text = Constants.Back } });
            }

            var bd = new PostgresStatsRepository();
            var stat = bd.GetById(statId);
            await Bot.BotClient.EditMessageTextAsync(new ChatId(chatId), messageId, "Настройка статистики: " + stat.Name, replyMarkup: new InlineKeyboardMarkup(Bot.NextButtons));
        }
    }
}