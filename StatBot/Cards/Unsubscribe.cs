using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using StatBot.Interfaces;

namespace StatBot.Cards
{
    public class Unsubscribe : ICard
    {
        public CardAction Action { get; set; }
        public List<string> ButtonsName { get; set; }

        public Activity OnClick(Activity activity)
        {
            activity.Text = "Была произведена отписка от статистики № " + Action.Value.ToString().Split(' ')[1];
            return activity;
        }

        public Unsubscribe(int Id, string Name = "Отписаться")
        {
            ButtonsName = new List<string> { "Unsubscribe" };
            Action = new CardAction(ActionTypes.PostBack, Name, null, null, null, "Unsubscribe " + Id.ToString());
        }
    }
}