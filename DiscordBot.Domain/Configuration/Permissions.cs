namespace DiscordBot.Domain.Configuration
{
    public class Permissions
    {
        public string[] banned_users { get; set; }
        public string[] trusted_users { get; set; }

        public string[] admin_users { get; set; }
    }
}
