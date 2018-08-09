﻿using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types.Enums;
using ModulBot.Models;
using System.Linq;
using ModulBot.Commands;
using ModulBot.Cards;
namespace ModulBot.Controllers
{
    [Route("api/message/update")]
    public class MessageController : Controller
    {
        [HttpPost]
        public async Task<OkResult> Post([FromBody]Update update)
        {
            if (update == null)
            {
                return Ok();
            }
            else
            {   
                await MessageAction(update);
                return Ok();
            }
        }

        public static async Task MessageAction(Update update)
        {
            var me = await Bot.BotClient.GetMeAsync();

            var user = DataModel.RememberUser(update.Message);
            try
            {
                if (update.CallbackQuery != null)
                {
                    var message = update.CallbackQuery.Message;
                    await ButtonAction(update.CallbackQuery.Data, message.Chat.Id, message.MessageId);
                }

                else if (update.Message != null && update.Message.Type == MessageType.Text && (update.Message.Chat.Type == ChatType.Group && (Bot.IsDialogStart || Bot.IsSetAdminStart|| update.Message.Text[0]=='/')
                    || update.Message.Chat.Type == ChatType.Private))
                {
                    var message = update.Message;
                    var str = message.Text.Trim();
                    if ((str == "/createstat" || Bot.IsDialogStart) && user.IsAdmin)
                    {
                        CreateStat stat = new CreateStat();
                        stat.Run(message);
                        return;
                    }
                    if ((str == "/tuneadmin" || Bot.IsSetAdminStart) && user.IsAdmin)
                    {
                        TuneAdmin tune = new TuneAdmin();
                        tune.Run(message);
                        return;
                    }
                    var indexOfSpace = str.IndexOf(" ", StringComparison.Ordinal);
                    var command = indexOfSpace != -1 ? str.Substring(0, indexOfSpace).ToLower()
                        : str.ToLower();
                    if (command[0] != '/')
                    {
                        command = "/" + command;
                    }
                    if (command.Contains($"@{me.Username.ToLower()}"))
                    {
                        int startIndex = command.IndexOf('@');
                        command = command.Substring(0, startIndex);
                    }
                    var help = new Help();

                    var tool = Bot.Tools.FirstOrDefault(x => x.CommandsName.Any(y => y.Equals(command)));
                    if (tool != null)
                    {
                        message.Text = indexOfSpace >= 0 ?
                            message.Text.Substring(indexOfSpace, str.Length - indexOfSpace) : String.Empty;

                        if (user == null || (!user.IsAdmin && (tool).IsAdmin))
                        {
                            help.Run(message);
                            return;
                        }
                        tool.Run(message);
                    }
                    else
                    {
                        help.Run(message);
                        return;
                    }
                }

            }
            catch
            {
                await Bot.BotClient.SendTextMessageAsync(update.CallbackQuery.Id,
                    "Упс, что-то пошло не так. Бот не может обработать команду/нажатие" +
                    " на кнопку. Но он продолжит работу");
            }
        }

        public static async Task ButtonAction(string callbackInfo, long chatId, int messageId)
        {
            if (callbackInfo.Split(' ')[2].ToLower().Equals("false"))
                for (int i = 0; i < Bot.NextButtons.Count; i++)
                {
                    if (Bot.NextButtons[i][0].CallbackData.Equals(callbackInfo))
                    {
                        await ActionButton.DoAction(chatId, messageId, callbackInfo.Split(' '));
                        return;
                    }
                }
            else
                for (int i = 0; i < Bot.StatButtons.Count; i++)
                {
                    if (Bot.StatButtons[i][0].CallbackData.Equals(callbackInfo))
                    {
                        await ActionButton.ShowButtons(chatId, messageId, callbackInfo.Split(' '));
                        return;
                    }
                }
            try
            {
                await Bot.BotClient.SendTextMessageAsync(chatId,
                    "Попробуйте заново ввести команду /tunestat(/statonoff - для админа)");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
