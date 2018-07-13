using StatBot.Attributes;
using StatBot.Interfaces;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StatBot.Commands
{
    [NotShowInHelp]
    [Serializable]
    public class Help : ITool
    {
        public string Description { get; set; }
        public List<string> CommandsName { get; set; }
        public bool IsAdmin { get; set; }

        protected string CommandText { get; set; }
        public Activity Run(Activity activity)
        {
            if (activity?.Conversation != null)
            {
                activity.Text = CommandText;
            }
            return activity;
        }

        public Help()
        {
            CommandsName = new List<string>() { "/help" };
            Description = "Оказание помощи";

            var result = new StringBuilder();
            var baseInterfaceType = typeof(ITool);
            var botCommands = Assembly.GetAssembly(baseInterfaceType)
                .GetTypes()
                .Where(types => types.IsClass && !types.IsAbstract && types.GetInterface("ITool") != null);
            foreach (var botCommand in botCommands)
            {
                if (!botCommand.GetCustomAttributes(typeof(NotShowInHelpAttribute)).Any())
                {
                    var command = (ITool)Activator.CreateInstance(botCommand);
                    result.Append($"{command.CommandsName.First()} - {command.Description}\n");
                }
            }
            CommandText = result.ToString();
        }
    }
}
