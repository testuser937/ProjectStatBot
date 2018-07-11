﻿using Microsoft.Bot.Schema;
using StatBot.Interfaces;
using StatBot.Models;
using System.Collections.Generic;
using StatBot.Cards;


namespace StatBot.Commands
{
    public class TuneStat : ITool
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

                foreach (var stat in DataModel.Statistics)
                {
                    if (stat.IsActive)
                        actions.Add(new StatisticButton(stat.Id, stat.Name,"Subs").Action);
                }

                Attachment a = new Attachment()
                {
                    Content = actions,
                };
                var heroCard = new HeroCard
                {
                    Title = "Список доступных статистик для настройки",
                    Buttons = actions,
                };

                attachment = heroCard.ToAttachment();
                activity.Attachments.Add(attachment);

            }
            return activity;
        }
        public TuneStat()
        {
            Description = "Настройка статистик";
            CommandsName = new List<string> { "/tunestat" };
            IsAdmin = false;
        }

    }
}

