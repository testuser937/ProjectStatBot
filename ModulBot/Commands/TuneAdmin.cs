using ModulBot.Database.PostgresRepositories;
using ModulBot.Interfaces;
using ModulBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using ModulBot.Attributes;

namespace ModulBot.Commands
{
    [NotShowInHelp]
    [Serializable]
    public class TuneAdmin : ITool
    {
        public string Description { get; set; }
        public List<string> CommandsName { get; set; }
        public bool IsAdmin { get; set; }
        private static int step = 0;
        private static int slctUserId = 0;

        public async void Run(Message message)
        {
            var db = new PostgresUserRepository();
            StringBuilder allUsers = new StringBuilder();
            foreach (var user in db.GetAll().OrderBy(x => x.Id))
            {
                if (user.ChatId > 0) // не берем групповые чаты
                {
                    string _isAdmin = user.IsAdmin ? "admin" : "user";
                    allUsers.Append($"{user.Id}. {user.FirstName} - ({_isAdmin})\n");
                }
            }
            if (message.Text[0] == '/')
            {
                message.Text = message.Text.Substring(1);
            }

            switch (step)
            {
                case 0:
                    if (message.Chat.Id < 0) //в групповых чатах бот не видит обычных сообщений
                    {
                        await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, Constants.ChatMessage, Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }

                    Bot.IsSetAdminStart = true;

                    await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "Вот список всех" +
                        " пользователей. Отправьте номер пользователя, права которого" +
                        " вы хотите настроить\n" + allUsers);
                    step++;
                    break;
                case 1:

                    int usersCount = DataModel.Users.Count;
                    if (int.TryParse(message.Text, out int choice) && choice > 0 && choice <= usersCount)
                    {
                        slctUserId = choice;
                        await Bot.BotClient.SendTextMessageAsync(message.Chat.Id,
                            "Выберите одну из команд:\n" +
                            "1. Дать админку\n2. Убрать админку");
                        step++;
                    }
                    else
                    {
                        await Bot.BotClient.SendTextMessageAsync(message.Chat.Id,
                            "Такого пользователя нет в БД. Попробуйте ввести номер еще раз");
                    }
                    break;
                case 2:
                    string res = message.Text;
                    if (res.ToLower() != "1" && res.ToLower() != "2")
                    {
                        await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "Такой команды нет. " +
                            "Введите \"1\" или \"2\"");
                    }
                    else
                    {
                        var user = db.GetById(slctUserId);
                        string resText = "";

                        if (res.ToLower() == "1")
                        {
                            user.IsAdmin = true;
                            resText = "Пользователь" + $" {user.FirstName} теперь админ";
                        }
                        else if (res.ToLower() == "2")
                        {
                            user.IsAdmin = false;
                            resText = "Пользователь" + $" {user.FirstName} теперь не админ.";
                        }

                        db.Update(user);
                        db.Save();
                        step = 0;
                        Bot.IsSetAdminStart = false;
                        await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, resText);
                    }
                    break;
            }
        }


        public TuneAdmin()
        {
            Description = "настройка админки";
            CommandsName = new List<string>() { "/tuneadmin" };
            IsAdmin = true;
        }
    }
}
