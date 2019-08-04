using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace OutlookInboxHandler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine($"ELKAddressAdder started at {DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}");

                if (args.Count() != 3 || args[1] == null || args[2] == null || (String.Compare($"Firefox", args[0], true) != 0) || (String.Compare($"Chrome", args[0], true) != 0))
                {
                    throw new System.Exception("Parameters ERROR: Invalid parameters or number of parameters");
                }

                List<string> addresses = new List<string>();

                var outlookChecker = new OutlookChecker();

                outlookChecker.GetAddressesFromOutlook(ref addresses);

                addresses.Distinct().ToList<string>();

                var arborHandler = new ArborHandler(args);

                if (addresses.Any())
                {
                    arborHandler.AddToFilterList(addresses);
                }

                var telegramNotificator = new TelegramNotificator();

                await telegramNotificator.TelegramNotification(addresses);

                Console.WriteLine("Exiting.");
            }
            catch(System.Exception ex)
            {
                if(ex.Source == "OutlookInboxHandler")      //если ошибка в args или в Telegram Proxy
                {
                    Console.WriteLine($"{ex.Message}\nExiting.");
                }
                else if(ex.Source == "mscorlib")     //если закрыт OutLook - уведомить
                {
                    Console.WriteLine("ERROR: Microsoft Outlook isn't running.\nExiting.");
                }
                else if (ex.Source == "Microsoft Outlook")        //если неверный путь к папке - уведомить
                {
                    Console.WriteLine("ERROR: folder C:\\ELKAddress not found.\nExiting.");
                }
                else if (ex.Source == "WebDriver")   //если ошибка в работе с Арбор
                {
                    Console.WriteLine($"{ex.Message}\nExiting.");
                }
            }
        }
    }
}
