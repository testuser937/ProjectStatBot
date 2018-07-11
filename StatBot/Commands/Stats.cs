using Microsoft.Bot.Schema;
using StatBot.Interfaces;
using StatBot.Models;
using System.Collections.Generic;

namespace StatBot.Commands
{
    public class Stats : ITool
    {
        public string Description { get; set; }
        public List<string> CommandsName { get; set; }
        public bool IsAdmin { get; set; }

        public Activity Run(Activity activity)
        {
            string StatNames = "";
            if (activity?.Conversation != null)
            {

                foreach (var stat in DataModel.Statistics)
                {
                    if (stat.IsActive)
                        StatNames += $"{stat.Id}. {stat.Name}\n";
                }
            }
            activity.Text = StatNames;
            return activity;
        }

        public Stats()
        {
            Description = "Выводит список статистик";
            CommandsName = new List<string> { "/stats" };
            IsAdmin = true;
        }
    }
}
