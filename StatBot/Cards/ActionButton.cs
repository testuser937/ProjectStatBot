using System;
using System.Collections.Generic;
using Microsoft.Bot.Schema;
using StatBot.Interfaces;
using StatBot.Models;
using StatBot.Database.PostgresRepositories;
using StatBot.Commands;
namespace StatBot.Cards
{
    public class ActionButton
    {
        public CardAction Action { get; set; }
        public struct ButtonSettings
        {
            public int ActionType;
            public int Statistic_Id;
            public string ButtonTittle;
            public bool IsStatistic;
            public ButtonSettings(int Type, int Id, string Tittle, bool IsStat)
            {
                ActionType = Type;
                Statistic_Id = Id;
                ButtonTittle = Tittle;
                IsStatistic = IsStat;
            }
        }
        public ButtonSettings buttonSettings;


        public Activity OnClick(Activity activity)
        {
            var bd = new PostgresStatsRepository();
            var stat = bd.GetById(buttonSettings.Statistic_Id);

            if (!buttonSettings.IsStatistic)
            {
                switch (buttonSettings.ActionType)
                {
                    case ((int)Constants.ActionTypes.TurnOn):
                        {
                            if (!stat.IsActive)
                            {
                                stat.IsActive = true;
                                activity.Text = $"Cтатистика №{buttonSettings.Statistic_Id} включена.";
                                bd.Update(stat);
                            }
                            else
                                activity.Text = $"Эта статистика уже включена";
                            break;
                        }
                    case ((int)Constants.ActionTypes.TurnOff):
                        {
                            if (stat.IsActive)
                            {
                                stat.IsActive = false;
                                activity.Text = $"Cтатистика № {buttonSettings.Statistic_Id} отключена.";
                                bd.Update(stat);
                            }
                            else
                                activity.Text = $"Эта статистика уже выключена";
                            break;
                        }

                    case ((int)Constants.ActionTypes.Subscribe):
                        {
                            string userId = activity.From.Id;
                            if (!stat.Subscribers.Contains(userId))
                            {
                                stat.Subscribers.Add(userId);
                                bd.Update(stat);
                                activity.Text = $"Вы подписались на статистикиу №{buttonSettings.Statistic_Id}";
                            }
                            else
                                activity.Text = "Вы уже подписаны на эту статистикиу";
                            break;
                        }

                    case ((int)Constants.ActionTypes.Unsubscribe):
                        {
                            string userId = activity.From.Id;
                            if (stat.Subscribers.Contains(userId))
                            {
                                stat.Subscribers.Remove(userId);
                                bd.Update(stat);
                                activity.Text = $"Вы отписались от статистики №{buttonSettings.Statistic_Id}";
                            }
                            else
                                activity.Text = "Вы уже отписаны от этой статистики";
                            break;
                        }
                    case ((int)Constants.ActionTypes.Back):
                        {
                            TuneStat tuneStat = new TuneStat();
                            activity.Text = "";
                            tuneStat.Run(activity);
                            break;
                        }
                }

            }
            else
            {
                activity.Attachments = new List<Attachment>();
                Attachment attachment;
                var actions = new List<CardAction>();
                EchoBot.ShowedButtons.Clear();
                if (buttonSettings.ActionType == (int)Constants.ActionTypes.ShowTurn)
                {
                    actions.Add(new ActionButton(buttonSettings.Statistic_Id, Constants.TurnOn, (int)Constants.ActionTypes.TurnOn, Constants.NotShowButtons).Action);
                    actions.Add(new ActionButton(buttonSettings.Statistic_Id, Constants.TurnOff, (int)Constants.ActionTypes.TurnOff, Constants.NotShowButtons).Action);
                    //actions.Add(new ActionButton(buttonSettings.Statistic_Id, Constants.Back, (int)Constants.ActionTypes.Back, Constants.NotShowButtons).Action);
                }
                else if (buttonSettings.ActionType == (int)Constants.ActionTypes.ShowSubs)
                {
                    actions.Add(new ActionButton(buttonSettings.Statistic_Id, Constants.Subscribe, (int)Constants.ActionTypes.Subscribe, Constants.NotShowButtons).Action);
                    actions.Add(new ActionButton(buttonSettings.Statistic_Id, Constants.Unsubscribe, (int)Constants.ActionTypes.Unsubscribe, Constants.NotShowButtons).Action);
                    actions.Add(new ActionButton(buttonSettings.Statistic_Id, Constants.Back, (int)Constants.ActionTypes.Back, Constants.NotShowButtons).Action);
                }
                Attachment a = new Attachment()
                {
                    Content = actions,
                };
                activity.Text = null;
                var heroCard = new HeroCard
                {
                    Title = "Настройка статистики № " + buttonSettings.Statistic_Id,
                    Buttons = actions,
                };
                attachment = heroCard.ToAttachment();
                activity.Attachments.Add(attachment);
            }
            bd.Save();
            return activity;
        }
        /// <summary>
        /// Инициализиурет новую кнопку
        /// </summary>
        /// <param name="Id">Идентификатор статистики за которую отвечает кнопка</param>
        /// <param name="ButtonTittle">Надпись, отображаемая на кнопке</param>
        /// <param name="ActionType">Действие, которое должна выполнять кнопка. Действия описаны в файле Constants.cs</param>
        /// <param name="IsStat">Определяет может ли кнопка отображать кнопки</param>
        public ActionButton(int Id, string ButtonTittle, int ActionType, bool IsStat)
        {
            buttonSettings = new ButtonSettings(ActionType, Id, ButtonTittle, IsStat);
            Action = new CardAction(ActionTypes.PostBack, ButtonTittle, null, ActionType.ToString(), null, $"ButtonClick {Id} {buttonSettings.ActionType}");
            EchoBot.ShowedButtons.Add(this);
        }
    }
}
