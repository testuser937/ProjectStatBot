using ModulBot.Attributes;
using ModulBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Telegram.Bot.Types;
using System.Threading.Tasks;


namespace ModulBot.Commands
{
    [NotShowInHelp]
    [Serializable]
    public class Help : ITool
    {
        public string Description { get; set; }
        public List<string> CommandsName { get; set; }
        public bool IsAdmin { get; set; }

        protected string CommandText { get; set; }
        public async void Run(Message message)
        {
            await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "Список доступных команд:\n" + CommandText);
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
                    result.Append($"{command.CommandsName.First()} - {command.Description}\n\r");
                }
            }
            CommandText = result.ToString();
        }
    }
}
