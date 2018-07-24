using System;
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
            var botClient = await Bot.GetBotClientAsync();

            var user = DataModel.RememberUser(update.Message);

            if (update == null) return Ok();


            try
            {
                if (update.CallbackQuery != null)
                {
                    var message = update.CallbackQuery.Message;
                    ButtonAction(update.CallbackQuery.Data, message.Chat.Id, message.MessageId);
                }

                else if (update.Message != null && update.Message.Type == MessageType.Text)
                {
                    var message = update.Message;

                    var str = message.Text.Trim();
                    if (str == "/createstat" || Bot.IsDialogStart1)
                    {
                        CreateStat stat = new CreateStat();
                        stat.Run(message);
                        return Ok();
                    }
                    var indexOfSpace = str.IndexOf(" ", StringComparison.Ordinal);
                    var command = indexOfSpace != -1 ? str.Substring(0, indexOfSpace).ToLower() : str.ToLower();
                    if (command[0] != '/')
                    {
                        command = "/" + command;
                    }

                    var help = new Help();

                    var tool = Bot.Tools.FirstOrDefault(x => x.CommandsName.Any(y => y.Equals(command)));
                    if (tool != null)
                    {
                        message.Text = indexOfSpace >= 0 ? message.Text.Substring(indexOfSpace, str.Length - indexOfSpace) : String.Empty;
                        //message.Text = indexOfSpace >= 0 ? message.Text.Substring(indexOfSpace + 1, str.Length - indexOfSpace - 1) : String.Empty;
                        if (user == null || (!user.IsAdmin && (tool).IsAdmin))
                        {
                            help.Run(message);
                            return Ok();
                        }
                        tool.Run(message);
                    }
                    else
                    {
                        help.Run(message);
                        return Ok();
                    }
                }

            }
            catch
            {
                await Bot.BotClient1.SendTextMessageAsync(update.CallbackQuery.Id, "Упс, что-то пошло не так. Бот не может обработать команду/нажатие на кнопку. Но он продолжит работу");
            }
            return Ok();
        }

        public static async void ButtonAction(string callbackInfo, long chatId, int messageId)
        {
            if (callbackInfo.Split(' ')[2].ToLower().Equals("false"))
                for (int i = 0; i < Bot.NextButtons.Count; i++)
                {
                    if (Bot.NextButtons[i][0].CallbackData.Equals(callbackInfo))
                    {
                        ActionButton.DoAction(chatId, messageId, callbackInfo.Split(' '));
                        return;
                    }
                }
            else
                for (int i = 0; i < Bot.StatButtons.Count; i++)
                {
                    if (Bot.StatButtons[i][0].CallbackData.Equals(callbackInfo))
                    {
                        ActionButton.ShowButtons(chatId, messageId, callbackInfo.Split(' '));
                        return;
                    }
                }
            try
            {
                await Bot.BotClient1.SendTextMessageAsync(chatId, "Попробуйте заново ввести команду /tunestat(/statonoff - для админа)");
            }
            catch
            {

            }
            return;
        }
    }
}
