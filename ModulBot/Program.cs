using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ModulBot
{
    public static class Program
    {
        private static Thread messageSender;

        public static Thread MessageSender { get => messageSender; set => messageSender = value; }

        public static void Main(string[] args)
        {
            MessageSender = new Thread(SendMessagesDaily);
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                 .UseStartup<Startup>()
                .Build();

        public async static void SendMessagesDaily()
        {
            while (true)
            {
                var DailyTime = "10:00:00";
                var dateNow = DateTime.Now;
                var timeParts = DailyTime.Split(new char[1] { ':' });
                var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day,
                           int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));
                if (date <= dateNow)
                {
                    await SendStat.SendStatistic();
                    Thread.Sleep((int)(24 * 60 * 60 * 1000 - (dateNow - date).TotalMilliseconds));
                }
                else
                {
                    Thread.Sleep((int)(date - dateNow).TotalMilliseconds);
                }
            }
        }
    }
}
