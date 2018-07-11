using System;
using System.Collections.Generic;
using Microsoft.Bot.Schema;
using StatBot.Interfaces;
using StatBot.Models;
using StatBot.Database.PostgresRepositories;
namespace StatBot.Cards
{
    public class ActionButton : ICard
    {
        public List<string> ButtonsName { get; set;  }
        public CardAction Action { get; set; }

        public Activity OnClick(Activity activity)
        {
            string[] InfoAboutButton = activity.Text.Split(' ');
            int Id = Convert.ToInt32(InfoAboutButton[1]);
            string Type = InfoAboutButton[2];

            var bd = new PostgresStatsRepository();
            var stat = bd.GetById(Id);
            switch (Type) {
                case "SwitchOff":
                    {
                        
                        if (stat.IsActive)
                        {
                            stat.IsActive = false;
                            activity.Text = $"Cтатистика № {Id} отключена.";
                            bd.Update(stat);
                        }
                        else
                            activity.Text = $"Эта статистика уже выключена";
                        break;
                    }
                case "SwitchOn":
                    {
                        if (!stat.IsActive)
                        {
                            stat.IsActive = true;
                            activity.Text = $"Cтатистика №{Id} включена.";
                            bd.Update(stat);
                        }
                        else
                            activity.Text = $"Эта статистика уже включена";
                        break;
                    }
                case "Subscribe":
                    {
                        string userId = activity.From.Id;
                        if (!stat.Subscribers.Contains(userId))
                        {
                            stat.Subscribers.Add(userId);
                            bd.Update(stat);
                            activity.Text = $"Вы подписались на статистикиу №{Id}";
                        }
                        else
                        {
                            activity.Text = "Вы уже подписаны на эту статистикиу";
                        }
                        break;
                    }
                case "Unsubscribe":
                    {
                        string userId = activity.From.Id;
                        if (stat.Subscribers.Contains(userId))
                        {
                            stat.Subscribers.Remove(userId);
                            bd.Update(stat);
                            activity.Text = $"Вы отписались от статистики №{Id}";
                        }
                        else
                        {
                            activity.Text = "Вы уже отписаны от этой статистики";
                        }
                        break;
                    }
            }
            bd.Save();
            return activity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id">Id статистики, которая будет настраиваться</param>
        /// <param name="Name">Надпись создаваемой кнопки</param>
        /// <param name="Type">Тип события кнопки, есть 4 собития: SwitchOff, SwitchOn, Subscribe, Unsubscribe</param>
        public ActionButton(int Id, string Name = "ActionButton title", string Type=null)
        {
            ButtonsName = new List<string> { "ActionButton" };
            Action = new CardAction(ActionTypes.PostBack, Name, null, null, null, $"ActionButton {Id} {Type}");
        }
    }
}
