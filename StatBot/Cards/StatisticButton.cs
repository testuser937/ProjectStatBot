using System;
using System.Collections.Generic;
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
                Attachment attachment;
                var actions = new List<CardAction>();

                var data = activity.Text.Split(' ');
                string CommandType = data[2];
                int StatisticId = Convert.ToInt32(data[1]);

                if (CommandType == "TurnOffOn")
                {
                    actions.Add(new ActionButton(StatisticId, "Включить статистику", "SwitchOn").Action);
                    actions.Add(new ActionButton(StatisticId, "Выключить статистику", "SwitchOff").Action);
                }
                else if (CommandType == "Subs")
                {
                    actions.Add(new ActionButton(StatisticId, "Подписаться", "Subscribe").Action);
                    actions.Add(new ActionButton(StatisticId, "Отписаться", "Unsubscribe").Action);
                }
                Attachment a = new Attachment()
                {
                    Content = actions,
                };
                var heroCard = new HeroCard
                {
                    Title = "Настройка статистики № " + Action.Value.ToString().Split(' ')[1],
                    Buttons = actions,
                };
                activity.Text = null;
                attachment = heroCard.ToAttachment();
                activity.Attachments.Add(attachment);
            }
            return activity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id">Id статистики, которая будет настраиваться</param>
        /// <param name="Name">Надпись создаваемой кнопки</param>
        /// <param name="Type">Тип создаваемой кнопки, на данный момент есть 2 типа: TurnOffOn, Subs</param>
        public StatisticButton(int Id, string Name = null,string Type="")
        {
            ButtonsName = new List<string> { "StatisticButton" };
            Action = new CardAction(ActionTypes.PostBack, $"{Id}. {Name}", null, null, null, $"StatisticButton {Id} {Type}");
        }
    }
}
