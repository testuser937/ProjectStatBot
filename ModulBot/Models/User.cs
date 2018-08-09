using Telegram.Bot.Types;

namespace ModulBot.Models
{
    public class User
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long ChatId { get; set; }
        public bool IsAdmin { get; set; }
        public string FirstName { get; set; }

        public User()
        {
        }

        public User(Message message)
        {
            if (message != null)
            {
                ChatId = message.Chat.Id;
                FirstName = message.From.FirstName + message.From.LastName;
                if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Private)
                {
                    UserName = message.From.Username;
                }
                else if (message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Group)
                {
                    UserName = "GroupChatBot";
                }

            }
        }
    }
}