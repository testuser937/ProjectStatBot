using System;
using StatBot.Attributes;
using System.Collections.Generic;
using StatBot.Interfaces;
using Microsoft.Bot.Schema;
using StatBot.Models;
using StatBot.Cards;

namespace StatBot.Commands
{
    [NotShowInHelp]
    [Serializable]
    public class StatOnOff : ITool
    {
        public string Description { get; set; }
        public List<string> CommandsName { get; set; }
        public bool IsAdmin { get; set; }

        public Activity Run(Activity activity)
        {
            if (activity?.Conversation != null)
            {
                activity.Attachments = new List<Attachment>();

                Attachment attachment;
                var actions = new List<CardAction>();
                EchoBot.ShowedButtons.Clear(); // удаляем из памяти все кнопки

                foreach (var stat in DataModel.Statistics)
                {
                    actions.Add(new ActionButton(stat.Id, $"{stat.Id}.{stat.Name}", (int)Constants.ActionTypes.ShowTurn, Constants.ShowButtons).Action);
                }

                Attachment a = new Attachment()
                {
                    Content = actions,
                };
                var heroCard = new HeroCard
                {
                    Title = "Список статистик",
                    Buttons = actions,
                };

                attachment = heroCard.ToAttachment();
                activity.Attachments.Add(attachment);

            }
            return activity;
        }

        public StatOnOff()
        {
            Description = "Включение\\Выключение статистик";
            CommandsName = new List<string> { "/statonoff" };
            IsAdmin = true;
        }
    }
}
