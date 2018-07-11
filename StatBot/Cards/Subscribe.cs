using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using StatBot.Interfaces;

namespace StatBot.Cards
{
    public class Subscribe : ICard
    {
        public CardAction Action { get; set; }
        public List<string> ButtonsName { get; set; }

        public Activity OnClick(Activity activity)
        {
            activity.Text = "Была произведена подписка на статистики № " + Action.Value.ToString().Split(' ')[1];
            return activity;
        }

        public Subscribe(int Id, string Name = "Подписаться")
        {
            ButtonsName = new List<string> { "Subscribe" };
            Action = new CardAction(ActionTypes.PostBack, Name, null, null, null, "Subscribe " + Id.ToString());

        }
    }
}
