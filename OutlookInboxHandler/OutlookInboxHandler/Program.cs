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
            int treshold = 1000;
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
                var notification = await telegramNotificator.Notify($"{progName} started on {System.Environment.MachineName}");


                if (args.Count() != 3 || args[1] == null || args[2] == null || ((String.Compare($"Firefox", args[0], true) != 0) && (String.Compare($"Chrome", args[0], true) != 0)))
                {
                    throw new System.Exception("Parameters ERROR: Invalid parameters or number of parameters");
                }

                List<string> addresses = new List<string>();

                var outlookChecker = new OutlookChecker(treshold, mailFolderPath, windowsFolderPath, logger);

                outlookChecker.GetAddressesFromOutlook(ref addresses);
                logger.Log($"{outlookChecker.messagesNumber} messages readed, {outlookChecker.attachmentsNumber} attachments analyzed, {addresses.Count()} addresses is(are) ready to adding");
                notification = await telegramNotificator.Notify($"{outlookChecker.messagesNumber} messages readed, {outlookChecker.attachmentsNumber} attachments analyzed, {addresses.Count()} address(es) is(are) ready to adding");
                if(outlookChecker.messagesNumber == 0)
                {
                    notification = await telegramNotificator.Notify($"WARNING! There are no messages in the \\{mailFolderPath} for the current hour!");
                }
                else if(outlookChecker.attachmentsNumber == 0)
                {
                    notification = await telegramNotificator.Notify($"WARNING! There are no atachments in current hour messages!");
                }

                if (addresses.Any())
                {
                    addresses = addresses.Distinct().ToList<string>();

                    var arborHandler = new ArborHandler(args, mitigationId, logger);
                    arborHandler.AddToFilterList(ref addresses);
                    if (addresses.Any())
                    {
                        notification = await telegramNotificator.Notify($"{progName} stopped succesfully. Addresses added to the mitigation:\n{String.Join(",\n", addresses)}");
                    }
                    else
                    {
                        notification = await telegramNotificator.Notify($"{progName} stopped succesfully. No addresses added to the mitigation: all addresses from the current mailing list are already in the filter.");
                    }
                }
                else
                {
                    logger.Log("There aren't addresses in the current mailing list!");
                    notification = await telegramNotificator.Notify($"{progName} stopped succesfully. No addresses added to the mitigation: there aren't addresses in the current mailing list.");
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
