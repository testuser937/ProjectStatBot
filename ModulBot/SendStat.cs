using ModulBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ModulBot
{
    public class SendStat
    {
        protected SendStat(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; set; }

        public async static Task SendStatistic()
        {
            Dictionary<long, List<string>> _stats = new Dictionary<long, List<string>>(); //для рассылки статистик
            int _count = 0;
            try
            {
                using (var conn = new NpgsqlConnection(Startup.GetConnectionString()))
                {
                    conn.Open();
                    NpgsqlDataReader dr;
                    foreach (var stat in DataModel.Statistics)
                    {
                        if (stat.IsActive)
                        {
                            var cmd = new NpgsqlCommand(stat.Query, conn);
                            dr = cmd.ExecuteReader();
                            while (dr.Read())
                                _count += 1;
                            foreach (var chatId in stat.Subscribers)
                            {
                                if (!_stats.Keys.Contains(chatId))
                                    _stats.Add(chatId, new List<string>());
                                _stats[chatId].Add($"{stat.Name}:\n" + 
                                    string.Format(stat.Message, _count) + "\n");
                            }
                            dr.Close();
                        }
                        _count = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            foreach (KeyValuePair<long, List<string>> keyValue in _stats)
            {
                await Bot.BotClient.SendTextMessageAsync(keyValue.Key, "Рассылка статистики:\n"
                    + string.Join('\n', keyValue.Value));
            }

        }
    }
}
