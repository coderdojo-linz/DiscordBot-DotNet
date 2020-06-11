namespace DiscordBot.Modules.Services
{
    public class MCServerAddress
    {
        private int _port;
        private string _ip;
        private string _password;

        public MCServerAddress(string ip, int port, string password)
        {
            _port = port;
            _ip = ip;
            _password = password;
        }
    }
}
