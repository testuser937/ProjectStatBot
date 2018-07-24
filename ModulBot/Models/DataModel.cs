using System;
using System.Collections.Generic;
using System.Linq;
using ModulBot.Database.PostgresRepositories;
using Telegram.Bot.Types;

namespace ModulBot.Models
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

        public static User RememberUser(Message message)
        {
            User _newUser;
            if (message != null)
            {
                try
                {
                    var db = new PostgresUserRepository();

                    var user = db.GetAll().FirstOrDefault(x => x.ChatId == message.Chat.Id); //&&
                                                                                             //x.UserName == message.Chat.FirstName);
                    if (user == null)
                    {
                        _newUser = new User(message);
                        db.Add(_newUser);
                        db.Save();
                        return _newUser;
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
