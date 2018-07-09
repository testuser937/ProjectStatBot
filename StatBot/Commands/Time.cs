using Microsoft.Bot.Schema;
using StatBot.Interfaces;
using System;
using System.Collections.Generic;

namespace StatBot.Commands
{
    [Serializable]
    public class Time : ITool
    {
        public string Description { get; set; }
        public List<string> CommandsName { get; set; }
        public bool IsAdmin { get; set; } = true;

        public Activity Run(Activity activity)
        {
            if (activity?.Conversation != null)
            {
                activity.Text = DateTime.Now.ToShortDateString();
            }
            return activity;
        }

        public Time()
        {
            CommandsName = new List<string> { "/time" };
            Description = "Выводит текущую дату (только для админов)";
        }
    }
}
