using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Schema;
using StatBot.Database.PostgresRepositories;

namespace StatBot.Models
{
    public static class DataModel
    {
        private static List<User> _users;
        private static List<Statistic> _stats;

        public static List<User> Users
        {
            get
            {
                var db = new PostgresUserRepository();                
                return db.GetAll().ToList();
            }
            set { _users = value; }
        }

        public static List<Statistic> Statistics
        {
            get
            {
                var db = new PostgresStatsRepository();                
                return db.GetAll().ToList().OrderBy(x => x.Id).ToList();
            }
            set { _stats = value; }
        }

        public static User RememberUser(Activity activity)
        {
            if (activity != null)
            {
                try
                {
                    var db = new PostgresUserRepository();

                    var user = db.GetAll().FirstOrDefault(x => x.ChannelId == activity.ChannelId && x.UserId == activity.From.Id &&
                                                 x.UserName == activity.From.Name);
                    if (user == null)
                    {
                        db.Add(new User(activity));
                        db.Save();
                    }
                    else
                    {
                        return user;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return null;
        }

    }
}
