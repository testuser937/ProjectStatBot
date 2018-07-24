using Telegram.Bot;
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


        public User()
        {
        }

        public User(Message message)
        {
            if (message != null)
            {
                //UserId = activity.From.Id;
                UserName = message.Chat.Username;
                ChatId = message.Chat.Id;
            }
        }
    }
}