namespace OutlookInboxHandler
{
    public class TelegramProxy
    {
        public string _address { get; }
        public int _port { get; }
        public string _login { get; }
        public string _pass { get; }

        public TelegramProxy(string address, int port, string login, string pass)
        {
            _address = address;
            _port = port;
            _login = login;
            _pass = pass;
        }
    }
}
