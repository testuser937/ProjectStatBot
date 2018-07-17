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
                return GetUsers();
            }
            set { _users = value; }
        }

        public static List<Statistic> Statistics
        {
            get
            {
               return GetStatistics();
            }
            set { _stats = value; }
        }

        private static List<User> GetUsers()
        {
            var db = new PostgresUserRepository();
            return db.GetAll().ToList();
        }

        private static List<Statistic> GetStatistics()
        {
            var db = new PostgresStatsRepository();
            return db.GetAll().OrderBy(x => x.Id).ToList();
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
