namespace StatBot
{
    public class BotUserState
    {
        public string statName { get; set; } = "";
        public string statQuery { get; set; } = "";
        public string statMessage { get; set; } = "";
        public bool statIsActive { get; set; }
    }
}
