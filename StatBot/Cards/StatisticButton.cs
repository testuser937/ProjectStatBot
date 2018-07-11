using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using StatBot.Interfaces;
using StatBot.Models;

namespace StatBot.Cards
{
    public class StatisticButton : ICard
    {
        public CardAction Action { get; set; }
        public List<string> ButtonsName { get; set; }

        public Activity OnClick(Activity activity)
        {
            if (activity?.Conversation != null)
            {
                activity.Attachments = new List<Attachment>();
                activity.Text = null;

                Attachment attachment;
                var actions = new List<CardAction>();

                var data = Action.Value.ToString().Split(' ');
                int num = Convert.ToInt32(data[1]);
                actions.Add(new Subscribe(num).Action);
                actions.Add(new Unsubscribe(num).Action);

                Attachment a = new Attachment()
                {
                    Content = actions,
                };
                var heroCard = new HeroCard
                {
                    Title = "Настройка статистики № " + Action.Value.ToString().Split(' ')[1],
                    Buttons = actions,
                };

                attachment = heroCard.ToAttachment();
                activity.Attachments.Add(attachment);
            }
            return activity;
        }

        public StatisticButton(int Id, string Name = null)
        {
            ButtonsName = new List<string> { "StatisticButton" };
            Action = new CardAction(ActionTypes.PostBack, $"{Id}. {Name}", null, null, null, "StatisticButton " + Id.ToString());
            //Action = new CardAction(ActionTypes.ImBack, $"{Id}. {Name}", null, "WORK WORK WORK", "MESSAGEBACK WORK 100%", "StatisticButton " + Id.ToString());
        }
    }
}
