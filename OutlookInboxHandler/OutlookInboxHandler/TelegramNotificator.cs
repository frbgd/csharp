using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MihaZupan;

namespace OutlookInboxHandler
{
    public class TelegramNotificator
    {
        string _chatId;
        string _botToken;
        HttpClient _client;
        Logger _logger;
        List<TelegramProxy> _proxies;

        private static TelegramNotificator _notificator;

        public static TelegramNotificator SetGetNotificator(Logger logger, string chatId, string botToken, List<TelegramProxy> proxies)
        {
            if (_notificator != null)
                return _notificator;
            else
            {
                _notificator = new TelegramNotificator(logger, chatId, botToken, proxies);
                return _notificator;
            }
        }

        protected TelegramNotificator(Logger logger, string chatId, string botToken, List<TelegramProxy> proxies)
        {
            _chatId = chatId;
            _botToken = botToken;
            _logger = logger;
            _proxies = new List<TelegramProxy>(proxies);
        }
        
        async Task<bool> SetProxy()
        {
            _logger.Log("Connecting to Telegram proxy server...");
            foreach(var proxy in _proxies)
            {
                _client = new HttpClient(new HttpClientHandler { Proxy = new HttpToSocks5Proxy(proxy._address, proxy._port, proxy._login, proxy._pass) }, true);
                if(await ProxyAvailabilityChecking(_client))
                {
                    _logger.Log($"Connected to proxy {proxy._address}:{proxy._port}");
                    return true;
                }
            }
            _logger.Log("ERROR\tAll telegram proxies are unavailable!");
            return false;
        }
        static async Task<bool> ProxyAvailabilityChecking(HttpClient client)
        {
            try
            {
                var result = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://api.telegram.org/"));
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> Notify(string message)
        {
            _logger.Log($"Sending message to the Telegram chat with body: {message}.");

            if (!await SetProxy())
                return false;

            try
            {
                var result = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"https://api.telegram.org/bot{_botToken}/sendMessage?chat_id={_chatId}&text={message}"));
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    _logger.Log($"ERROR sending message: {content}");
                    return false;
                }
            }
            catch(Exception ex)
            {
                _logger.Log($"ERROR\t{ex.Message}");
                return false;
            }

            _logger.Log("Message sent");
            return true;
        }
    }
}
