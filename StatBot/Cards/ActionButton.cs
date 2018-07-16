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
            private int _actionType;
            private string _buttonTittle;
            private bool _isStatistic;
            private int _statistic_Id;

            public int ActionType { get => _actionType; set => _actionType = value; }
            public int Statistic_Id { get => _statistic_Id; set => _statistic_Id = value; }
            public string ButtonTittle { get => _buttonTittle; set => _buttonTittle = value; }
            public bool IsStatistic { get => _isStatistic; set => _isStatistic = value; }

            public ButtonSettings(int Type, int Id, string Tittle, bool IsStat)
            {
                _actionType = Type;
                _statistic_Id = Id;
                _buttonTittle = Tittle;
                _isStatistic = IsStat;
            }
        }
        private ButtonSettings _buttonSettings;
        public ButtonSettings BtnSettings { get => _buttonSettings; set => _buttonSettings = value; }

        public Activity OnClick(Activity activity)
        {
            if (!_buttonSettings.IsStatistic)
            {
                return DoAction(activity, _buttonSettings.Statistic_Id);
            }
            else
            {
                return ShowButtons(activity, _buttonSettings.Statistic_Id);
            }
        }

        private Activity DoAction(Activity activity, int ID)
        {
            var bd = new PostgresStatsRepository();
            var stat = bd.GetById(ID);

            switch (_buttonSettings.ActionType)
            {
                case ((int)Constants.ActionTypes.TurnOn):
                    {
                        if (!stat.IsActive)
                        {
                            stat.IsActive = true;
                            activity.Text = $"Cтатистика №{ID} включена.";
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
                            activity.Text = $"Cтатистика № {ID} отключена.";
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
                            activity.Text = $"Вы подписались на статистикиу";
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
                            activity.Text = $"Вы отписались от статистики";
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

            bd.Save();
            return activity;
        }

        private Activity ShowButtons(Activity activity, int ID)
        {
            activity.Attachments = new List<Attachment>();
            Attachment attachment;
            var actions = new List<CardAction>();
            if (_buttonSettings.ActionType == (int)Constants.ActionTypes.ShowTurn)
            {
                actions.Add(new ActionButton(ID, Constants.TurnOn, (int)Constants.ActionTypes.TurnOn, Constants.NotShowButtons).Action);
                actions.Add(new ActionButton(ID, Constants.TurnOff, (int)Constants.ActionTypes.TurnOff, Constants.NotShowButtons).Action);
            }
            else if (_buttonSettings.ActionType == (int)Constants.ActionTypes.ShowSubs)
            {
                actions.Add(new ActionButton(ID, Constants.Subscribe, (int)Constants.ActionTypes.Subscribe, Constants.NotShowButtons).Action);
                actions.Add(new ActionButton(ID, Constants.Unsubscribe, (int)Constants.ActionTypes.Unsubscribe, Constants.NotShowButtons).Action);
                actions.Add(new ActionButton(ID, Constants.Back, (int)Constants.ActionTypes.Back, Constants.NotShowButtons).Action);
            }

            activity.Text = null;

            var bd = new PostgresStatsRepository();
            var stat = bd.GetById(ID);

            var heroCard = new HeroCard
            {
                Title = "Настройка статистики: "+ stat.Name,
                Buttons = actions,
            };
            attachment = heroCard.ToAttachment();
            activity.Attachments.Add(attachment);
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
            _buttonSettings = new ButtonSettings(ActionType, Id, ButtonTittle, IsStat);
            Action = new CardAction(ActionTypes.PostBack, ButtonTittle, null, ActionType.ToString(), null, $"ButtonClick {Id} {_buttonSettings.ActionType}");
            EchoBot.ShowedButtons.Add(this);
        }
    }
}
