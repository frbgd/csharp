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
            string chatId = "259571389";
            string botToken = "952380349:AAGKIafp1PM4gMfZXBSodaJgLKwwHhiJmqE";
            string progName = "ELKAddressAdder";
            string mailFolderPath = "soc@RT.RU\\Входящие\\ELK";
            string windowsFolderPath = "C:\\ELKAddressAdder";
            string mitigationId = "58640";


            var logger = Logger.SetGetLogger(progName, windowsFolderPath);
            logger.Log($"{progName} started.");
            var telegramNotificator = TelegramNotificator.SetGetNotificator(logger, chatId, botToken);
            try
            {
                var notification = await telegramNotificator.Notify($"{progName} started");


                if (args.Count() != 3 || args[1] == null || args[2] == null || ((String.Compare($"Firefox", args[0], true) != 0) && (String.Compare($"Chrome", args[0], true) != 0)))
                {
                    throw new System.Exception("Parameters ERROR: Invalid parameters or number of parameters");
                }

                List<string> addresses = new List<string>();

                var outlookChecker = new OutlookChecker(mailFolderPath, windowsFolderPath, logger);

                outlookChecker.GetAddressesFromOutlook(ref addresses);

                addresses = addresses.Distinct().ToList<string>();

                var arborHandler = new ArborHandler(args, mitigationId, logger);

                if (addresses.Any())
                {
                    arborHandler.AddToFilterList(addresses);
                    notification = await telegramNotificator.Notify($"{progName} stopped succesfully. Addresses added in mitigation:\n{String.Join(",\n", addresses)}");
                }
                else
                {
                    notification = await telegramNotificator.Notify($"{progName} stopped succesfully. No addresses added to mitigation");
                }

                logger.Log("Exiting");
            }
            catch(System.Exception ex)
            {
                if(ex.Source == "OutlookInboxHandler")      //если ошибка в args или в Telegram Proxy
                {
                    logger.Log($"{ex.Message}\tExiting");
                    var notification = await telegramNotificator.Notify($"{progName} FAILED!\t{ex.Message}");
                }
                else if(ex.Source == "mscorlib")     //если закрыт OutLook - уведомить
                {
                    logger.Log("ERROR: Microsoft Outlook isn't running\tExiting");
                    var notification = await telegramNotificator.Notify($"{progName} FAILED!\tMicrosoft Outlook isn't running");
                }
                else if (ex.Source == "Microsoft Outlook")        //если неверный путь к папке - уведомить
                {
                    logger.Log($"ERROR: Mail folder {mailFolderPath} doesn't exists\tExiting");
                    var notification = await telegramNotificator.Notify($"{progName} FAILED!\tMail folder {mailFolderPath} doesn't exists");
                }
                else if (ex.Source == "WebDriver")   //если ошибка в работе с Арбор
                {
                    logger.Log($"{ex.Message}\tExiting");
                    var notification = await telegramNotificator.Notify($"{progName} FAILED!\t{ex.Message}");
                }
            }
        }
    }
}
