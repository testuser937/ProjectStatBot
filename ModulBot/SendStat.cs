using ModulBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using ModulBot.Database;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace ModulBot
{
    public static class SendStat
    {
        private static IConfiguration Configuration { get; }

        public async static void SendStatistic()
        {
            int _count = 0;
            string _connStr = Configuration.GetConnectionString("PostgreSQL");
            try
            {
                using (var conn = new NpgsqlConnection(_connStr))
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
                                await Bot.BotClient1.SendTextMessageAsync(chatId, string.Format(stat.Message, _count));
                                Thread.Sleep(100);
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

        }
    }
}
