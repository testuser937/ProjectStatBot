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
        /// Every Conversation turn for our EchoBot will call this method.
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>        
        public async Task OnTurn(ITurnContext context)
        {
            if (context.Activity.Type == ActivityTypes.Message)
            {
                if (!String.IsNullOrEmpty(context.Activity.Text))
                {
                    var data = context.Activity.Text.Split(' ');
                    int Id = 0;
                    try
                    {
                        context.Activity.Text = data[0];
                        Id = Convert.ToInt32(data[1]);
                    }
                    catch { }

                    List<string> ButtonNames = new List<string> { "Unsubscribe", "Subscribe", "StatisticButton" };
                    if (ButtonNames.Contains(context.Activity.Text))
                    {
                        List<ICard> _cards = new List<ICard>();

                        var baseInterfaceType_ = typeof(ICard);
                        var botButtons = Assembly.GetAssembly(baseInterfaceType_)
                            .GetTypes()
                            .Where(types => types.IsClass && !types.IsAbstract && types.GetInterface("ICard") != null);


                        foreach (var botButton in botButtons)
                        {
                            _cards.Add((ICard)Activator.CreateInstance(botButton, Id, null));
                        }

                        var str_ = context.Activity.Text.Trim();

                        var card = _cards.FirstOrDefault(x => x.ButtonsName.Any(y => y.Equals(str_)));
                        if (card != null)
                        {
                            await context.SendActivity(card.OnClick(context.Activity));
                        }
                    }
                    else
                    {
                        var user = DataModel.RememberUser(context.Activity);
                        List<ITool> _tools = new List<ITool>();


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
                            context.Activity.Text = indexOfSpace >= 0 ? context.Activity.Text.Substring(indexOfSpace + 1, str.Length - indexOfSpace - 1) : String.Empty;
                            if (user == null || (!user.IsAdmin && ((ITool)tool).IsAdmin))
                            {
                                await context.SendActivity(help.Run(context.Activity));
                                return;
                            }
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
}
