using StatBot.Interfaces;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;


namespace StatBot.Commands
{
    public class ToUpper : ITool
    {
        public string Description { get; set; }
        public List<string> CommandsName { get; set; }
        public bool IsAdmin { get; set; }

        public Activity Run(Activity activity)
        {
            if (activity?.Conversation != null)
            {
                activity.Text = activity.Text.ToUpper();
            }
            return activity;
        }

        public ToUpper()
        {
            CommandsName = new List<string> { "/toupper" };
            Description = "Приводит текст к верхнему регистру";
        }
    }
}
