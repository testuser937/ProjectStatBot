using System;
using System.Collections.Generic;
using ModulBot.Attributes;
using ModulBot.Database.PostgresRepositories;
using ModulBot.Interfaces;
using ModulBot.Models;
using Telegram.Bot.Types;

namespace ModulBot.Commands
{
    [NotShowInHelp]
    [Serializable]
    public class CreateStat : ITool
    {

        public string Description { get; set; }
        public List<string> CommandsName { get; set; }
        public bool IsAdmin { get; set; }
        private static int step = 0;

        public async void Run(Message message)
        {
            if (message.Text[0] == '/')
            {
                message.Text = message.Text.Substring(1);
            }

            switch (step)
            {
                case 0:
                    if (message.Chat.Id < 0) //в групповых чатах бот не видит обычных сообщений
                    {
                        await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "*Внимание!* В групповых" +
                            " чатах перед ответом ставьте слэш \"*/*\", " +
                            "т.к. бот не видит обычных сообщений.\nНапример: */да*, */1*, и т.д.",
                            Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }

                    Bot.IsDialogStart = true;
                    step++;
                    await Bot.BotClient.SendTextMessageAsync(message.Chat.Id,
                        "Введите название");

                    break;
                case 1:
                    UserState.statName = message.Text;
                    step++;
                    await Bot.BotClient.SendTextMessageAsync(message.Chat.Id,
                        "Введите сообщение");

                    break;
                case 2:
                    UserState.statMessage = message.Text;
                    step++;
                    await Bot.BotClient.SendTextMessageAsync(message.Chat.Id,
                        "Введите SQL-запрос");

                    break;
                case 3:
                    UserState.statQuery = message.Text;
                    step++;
                    await Bot.BotClient.SendTextMessageAsync(message.Chat.Id,
                        "Активировать статистику?[да/нет]\nПо умолчанию статистика будет активна");

                    break;
                case 4:

                    if (message.Text.ToLower() == "нет")
                    {
                        UserState.statIsActive = false;
                    }
                    else
                    {
                        UserState.statIsActive = true;
                    }

                    string result = $"Введенные данные:\n" +
                        $"Название: {UserState.statName}\n" +
                        $"Сообщение: {UserState.statMessage}\n" +
                        $"SQL-зарос: {UserState.statQuery}\n" +
                        $"Активировать: {UserState.statIsActive.ToString()}\n";
                    step++;
                    await Bot.BotClient.SendTextMessageAsync(message.Chat.Id,
                        result + "Все верно?[да/нет]\n");
                    break;
                case 5:
                    if (message.Text.ToLower() == "да")
                    {
                        Bot.IsDialogStart = false;

                        var db = new PostgresStatsRepository();
                        Statistic stat = new Statistic(UserState.statName,
                            UserState.statMessage, UserState.statQuery, UserState.statIsActive);
                        db.Add(stat);
                        db.Save();

                        step = 0;
                        await Bot.BotClient.SendTextMessageAsync(message.Chat.Id,
                            "Статистика успешно создана");
                    }
                    else if (message.Text.ToLower() == "нет")
                    {
                        Bot.IsDialogStart = false;
                        step = 0;
                        await Bot.BotClient.SendTextMessageAsync(message.Chat.Id,
                            "Попробуйте еще раз создать статистику с помощью команды /createstat");

                    }
                    else
                    {
                        await Bot.BotClient.SendTextMessageAsync(message.Chat.Id,
                            "Команда не распознана. Введите *\"да\"* или *\"нет\"*",
                            Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    }
                    break;
            }
        }

        public CreateStat()
        {
            CommandsName = new List<string>() { "/createstat" };
            Description = "Создание статистики";
            IsAdmin = true;
        }
    }
}
