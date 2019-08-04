using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MihaZupan;

namespace OutlookInboxHandler
{
    public class TelegramNotificator
    {
        HttpClient client;
        Logger _logger;

        public TelegramNotificator(Logger logger)
        {
            _logger = logger;
        }

        async Task<bool> SetProxy()
        {
            _logger.Log("Checking Telegram proxy server...");

            client = new HttpClient(new HttpClientHandler { Proxy = new HttpToSocks5Proxy("tmpx.soc.rt.ru", 1080, "cdc", "UZy58MNr2kW769s74Sn2dQ2xP7zKwLyy") }, true);

            if (!await ProxyAvailabilityChecking(client))
            {
                _logger.Log("Error\tTrying another proxy server...");
                client = new HttpClient(new HttpClientHandler { Proxy = new HttpToSocks5Proxy("139.162.141.171", 31422, "pirates", "hmm_i_see_some_pirates_here_meeeew") }, true);
            }
            if (!await ProxyAvailabilityChecking(client))
            {
                _logger.Log("Error\tTelegram Proxy is unavailable!");
                return false;
            }
            _logger.Log("Done");
            return true;
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

        public async Task<bool> TelegramNotification(List<string> addresses)
        {
            _logger.Log("Sending message...");

            string notificationBody = "";
            foreach (string address in addresses)
            {
                notificationBody = $"{notificationBody}{address}\n";
            }

            try
            {
                var result = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"https://api.telegram.org/bot952380349:AAGKIafp1PM4gMfZXBSodaJgLKwwHhiJmqE/sendMessage?chat_id=259571389&text=Addresses:{notificationBody}"));
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.Log("ERROR");
                    return false;
                }
            }
            catch
            {
                _logger.Log("ERROR");
                return false;
            }

            _logger.Log("Done");
            return true;
        }
    }
}
