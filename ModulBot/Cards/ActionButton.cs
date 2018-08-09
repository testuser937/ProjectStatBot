using System;
using System.Collections.Generic;
using ModulBot.Database.PostgresRepositories;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Npgsql;
using System.Threading.Tasks;

namespace ModulBot.Cards
{
    public static class ActionButton
    {
        public static async Task DoAction(long chatId, int messageId, string[] buttonCallbackData)
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
                        stat.IsActive = true;
                        text = $"Cтатистика №{statId} включена.";
                        bd.Update(stat);
                        break;
                    }
                case ((int)Constants.ActionTypes.TurnOff):
                    {
                        stat.IsActive = false;
                        text = $"Cтатистика №{statId} отключена.";
                        bd.Update(stat);
                        break;
                    }

                case ((int)Constants.ActionTypes.Subscribe):
                    {
                        stat.Subscribers.Add(chatId);
                        bd.Update(stat);
                        text = $"Вы подписались на статистикиу";
                        break;
                    }

                case ((int)Constants.ActionTypes.Unsubscribe):
                    {
                        stat.Subscribers.Remove(chatId);
                        bd.Update(stat);
                        text = $"Вы отписались от статистики";
                        break;
                    }
                case ((int)Constants.ActionTypes.UnsibscribeAll):
                    {
                        List<Models.Statistic> stats = new List<Models.Statistic>(bd.GetAll());
                        for (int i = 0; i < stats.Count; i++  )
                        {
                            if (stats[i].Subscribers.Contains(chatId))
                            {
                                stats[i].Subscribers.Remove(chatId);
                                bd.Update(stats[i]);
                            }
                        }
                        
                        text = "Вы были отписаны от всех статистик";
                        break;
                    }
                case ((int)Constants.ActionTypes.GetStat):
                    {
                        text = GetStat(statId);
                        break;
                    }
                case ((int)Constants.ActionTypes.Back):
                    {
                        bool isTunestat = (Bot.LastCommand == "tunestat") ? true : false;
                        
                        await Bot.BotClient.EditMessageTextAsync(new ChatId(chatId), messageId, Bot.TextOnMessageWithButtons, Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: new InlineKeyboardMarkup(Bot.GenerateStatButtons(isTunestat,chatId)));
                        return;
                    }
            }
            bd.Save();
            await Bot.BotClient.SendTextMessageAsync(chatId, text);
        }

        public static async Task ShowButtons(long chatId, int messageId, string[] buttonCallbackData)
        {
            int statId = Convert.ToInt32(buttonCallbackData[0]);
            int actionType = Convert.ToInt32(buttonCallbackData[1]);
            var bd = new PostgresStatsRepository();
            var stat = bd.GetById(statId);
            Bot.NextButtons = new List<List<InlineKeyboardButton>>();
            if (actionType == (int)Constants.ActionTypes.ShowTurn)
            {
                if (!stat.IsActive)
                {
                    Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.TurnOn} {Constants.NotShowButtons}", Text = Constants.TurnOn } });
                }
                else
                {
                    Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.TurnOff} {Constants.NotShowButtons}", Text = Constants.TurnOff } });
                }
                Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.Back} {Constants.NotShowButtons}", Text = Constants.Back } });
            }
            else
            {
                if (!stat.Subscribers.Contains(chatId))
                {
                    Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.Subscribe} {Constants.NotShowButtons}", Text = Constants.Subscribe } });
                }
                else
                {
                    Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.Unsubscribe} {Constants.NotShowButtons}", Text = Constants.Unsubscribe } });
                }
                Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.GetStat} {Constants.NotShowButtons}", Text = Constants.GetStat } });
                Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{0} {(int)Constants.ActionTypes.UnsibscribeAll} {Constants.NotShowButtons}", Text = Constants.UnsubscribeAll } });
                Bot.NextButtons.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton { CallbackData = $"{statId} {(int)Constants.ActionTypes.Back} {Constants.NotShowButtons}", Text = Constants.Back } });
            }

            await Bot.BotClient.EditMessageTextAsync(new ChatId(chatId), messageId, "Настройка статистики: " + stat.Name, replyMarkup: new InlineKeyboardMarkup(Bot.NextButtons));
        }
        private static string GetStat(int statId)
        {
            string _connStr = Startup.GetConnectionString();
            int count = 0;
            var db = new PostgresStatsRepository();
            var selected_stat = db.GetById(statId);

            try
            {
                using (var conn = new NpgsqlConnection(_connStr))
                {
                    conn.Open();
                    string query = selected_stat.Query;
                    var cmd = new NpgsqlCommand(query, conn);
                    NpgsqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                        count += 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return string.Format(selected_stat.Message, count);
        }
    }
}