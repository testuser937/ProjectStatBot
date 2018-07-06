using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using StatBot.Commands;
using StatBot.Interfaces;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using StatBot.Models;

namespace StatBot
{
    public class EchoBot : IBot
    {
        /// <summary>
        /// Every Conversation turn for our EchoBot will call this method. In here
        /// the bot checks the Activty type to verify it's a message, bumps the 
        /// turn conversation 'Turn' count, and then echoes the users typing
        /// back to them. 
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>        
        public async Task OnTurn(ITurnContext context)
        {
            // This bot is only handling Messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                if (!String.IsNullOrEmpty(context.Activity.Text))
                {
                    var user = DataModel.RememberUser(context.Activity);
                    List<ITool> _tools;
                    _tools = new List<ITool>();

                    var baseInterfaceType = typeof(ITool);
                    var botCommands = Assembly.GetAssembly(baseInterfaceType)
                        .GetTypes()
                        .Where(types => types.IsClass && !types.IsAbstract && types.GetInterface("ITool") != null);
                    foreach (var botCommand in botCommands)
                    {
                        _tools.Add((ITool)Activator.CreateInstance(botCommand));
                    }

                    var str = context.Activity.Text.Trim();
                    var indexOfSpace = str.IndexOf(" ", StringComparison.Ordinal);
                    var command = indexOfSpace != -1 ? str.Substring(0, indexOfSpace).ToLower() : str.ToLower();
                    if (command[0] != '/')
                    {
                        command = "/" + command;
                    }

                    var help = new Help();

                    var tool = _tools.FirstOrDefault(x => x.CommandsName.Any(y => y.Equals(command)));
                    if (tool != null)
                    {
                        context.Activity.Text = indexOfSpace >= 0 ? context.Activity.Text.Substring(indexOfSpace, str.Length - indexOfSpace) : String.Empty;
                        await context.SendActivity(tool.Run(context.Activity));
                    }
                    else
                    {
                        await context.SendActivity(help.Run(context.Activity));
                    }
                }

            }
        }
    }    
}
