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
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Core.Extensions;
using StatBot.Database.PostgresRepositories;
using StatBot.Cards;

namespace StatBot
{
    public class EchoBot : IBot
    {
        private readonly DialogSet dialogs;
        private static bool IsDialogStart = false;
        private static List<ActionButton> showedButtons = new List<ActionButton>();
        private static List<ITool> _tools = new List<ITool>();

        public static List<ActionButton> ShowedButtons { get => showedButtons; set => showedButtons = value; }

        public EchoBot()
        {
            dialogs = new DialogSet();
            InitDialog();

            var baseInterfaceType = typeof(ITool);
            var botCommands = Assembly.GetAssembly(baseInterfaceType)
                .GetTypes()
                .Where(types => types.IsClass && !types.IsAbstract && types.GetInterface("ITool") != null);
            foreach (var botCommand in botCommands)
            {
                _tools.Add((ITool)Activator.CreateInstance(botCommand));
            }
        }


        /// <summary>
        /// Every Conversation turn for our EchoBot will call this method.
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>        
        public async Task OnTurn(ITurnContext context)
        {
            if (context.Activity.Type == ActivityTypes.Message)
            {
                var user = DataModel.RememberUser(context.Activity);
                var state = ConversationState<Dictionary<string, object>>.Get(context);
                var dc = dialogs.CreateContext(context, state);
                await dc.Continue();

                if (!context.Responded)
                {
                    if (context.Activity.Text.ToLowerInvariant().Contains("/createstat"))
                    {
                        await dc.Begin("createStatDialig");
                        return;
                    }
                }

                if (!String.IsNullOrEmpty(context.Activity.Text))
                {
                    var a = context.Activity.Text.Split(' ');
                    if (a[0].Equals("ButtonClick") && a.Length == 3)
                    {
                        await DoButtonAction(a,context);
                    }
                    else
                    {
                        await DoCommand(context, user);
                    }
                }
            }
        }

        public async Task DoButtonAction(string[] a, ITurnContext context)
        {
            int Id = Convert.ToInt32(a[1]);
            int ButtonAction = Convert.ToInt32(a[2]);
            for (int i = 0; i < ShowedButtons.Count; i++)
            {
                if (ShowedButtons[i].BtnSettings.Statistic_Id == Id && ShowedButtons[i].BtnSettings.ActionType == ButtonAction)
                {
                    await context.SendActivity(ShowedButtons[i].OnClick(context.Activity));
                    return;
                }
            }
            await context.SendActivity("Вы нажали на кнопку которой уже нет в памяти!\n\rЗаново наберите команду /tunestat");
        }


        public async Task DoCommand(ITurnContext context,User user)
        {
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
                if (user == null || (!user.IsAdmin && (tool).IsAdmin))
                {
                    await context.SendActivity(help.Run(context.Activity));
                    return;
                }
                await context.SendActivity(tool.Run(context.Activity));
            }
            else if (!IsDialogStart)
            {
                await context.SendActivity(help.Run(context.Activity));
            }
        }

        public void InitDialog()
        {
            dialogs.Add("createStatDialig", new WaterfallStep[]
            {
                async (dc, args, next) =>
                {
                    dc.ActiveDialog.State = new Dictionary<string, object>();
                    // Prompt for the guest's name.

                    await dc.Prompt("textPrompt1", "Введите имя статистики");
                },
                async(dc, args, next) =>
                {
                    dc.ActiveDialog.State["name"] = args["Value"];
                    var userState = UserState<BotUserState>.Get(dc.Context);
                    userState.statName = Convert.ToString(dc.ActiveDialog.State["name"]);
            
                    // Ask for next info
                    await dc.Prompt("textPrompt2", "Введите sql-запрос");

                },
                async(dc, args, next) =>
                {
                    dc.ActiveDialog.State["query"] = args["Value"];

                            // Save UserName to userState
                            var userState = UserState<BotUserState>.Get(dc.Context);
                            userState.statQuery = Convert.ToString(dc.ActiveDialog.State["query"]);

                    // Ask for next info
                    await dc.Prompt("textPrompt3", "Введите шаблон сообщения");
                },

                async(dc, args, next) =>
                {
                    dc.ActiveDialog.State["message"] = args["Value"];

                            // Save UserName to userState
                            var userState = UserState<BotUserState>.Get(dc.Context);
                            userState.statMessage = Convert.ToString(dc.ActiveDialog.State["message"]);


                    await dc.Prompt("textPrompt4", "Активировать статистику?(да/нет)[да]");

                },
                async(dc, args, next) =>
                {
                    string isActive = args["Value"].ToString();


                    var userState = UserState<BotUserState>.Get(dc.Context);
                     if(isActive == "нет")
                    {
                        userState.statIsActive = false;
                    }
                    else
                    {
                        userState.statIsActive = true;
                    }

                    string msg = "Статистика создана - " +
                    $"\nНазвание: {userState.statName} " +
                    $"\nSQL-запрос: {userState.statQuery} " +
                    $"\nСообщение: {userState.statMessage}" +
                    $"\nАктивна: {userState.statIsActive}";
                    await dc.Context.SendActivity(msg);
                    // Ask for next info
                    await dc.Prompt("textPrompt5", "Все верно?(да/нет)");

                },
                async(dc, args, next) =>
                {
                    string reply = args["Value"].ToString();
                    if(reply.ToLower() == "да")
                    {
                        await dc.Context.SendActivity("Статистика успешно создана");
                        await dc.End();

                        var userState = UserState<BotUserState>.Get(dc.Context);
                        var db = new PostgresStatsRepository();
                        Statistic stat = new Statistic(userState.statName, userState.statMessage, userState.statQuery);
                        db.Add(stat);
                        db.Save();
                    }
                    else if(reply.ToLower() == "нет")
                    {
                        await dc.Begin("createStatDialig");

                    }
                    else
                    {
                        await dc.Context.SendActivity("Неверная команда. Попробуйте еще раз.\nДля создания" +
                            " статистики наберите /createstat");
                    }
                }
            });

            dialogs.Add("textPrompt1", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
            dialogs.Add("textPrompt2", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
            dialogs.Add("textPrompt3", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
            dialogs.Add("textPrompt4", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
            dialogs.Add("textPrompt5", new Microsoft.Bot.Builder.Dialogs.TextPrompt());
        }
    }
}
