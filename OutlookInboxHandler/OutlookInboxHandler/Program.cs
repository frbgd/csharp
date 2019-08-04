﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace OutlookInboxHandler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var logger = Logger.SetGetLogger();
            logger.Log($"ELKAddressAdder started.");
            var telegramNotificator = TelegramNotificator.SetGetNotificator(logger, "259571389");
            try
            {
                var notification = await telegramNotificator.Notify("ELKAddressAdder started");

                
                if (args.Count() != 3 || args[1] == null || args[2] == null || ((String.Compare($"Firefox", args[0], true) != 0) && (String.Compare($"Chrome", args[0], true) != 0)))
                {
                    throw new System.Exception("Parameters ERROR: Invalid parameters or number of parameters");
                }

                List<string> addresses = new List<string>();

                var outlookChecker = new OutlookChecker(logger, telegramNotificator);
                if (!outlookChecker.DoneFolderExists())
                    notification = await telegramNotificator.Notify("Warning!\tFolder \\\\soc@RT.RU\\ELK\\Done doesn't exist");

                outlookChecker.GetAddressesFromOutlook(ref addresses);

                addresses.Distinct().ToList<string>();

                var arborHandler = new ArborHandler(args, logger);

                if (addresses.Any())
                {
                    arborHandler.AddToFilterList(addresses);
                    notification = await telegramNotificator.Notify($"ELKAddressAdder stopped succesfully. Addresses added in mitigation:\n{String.Join(",\n", addresses)}.");
                }
                else
                {
                    notification = await telegramNotificator.Notify("ELKAddressAdder stopped succesfully. No address addedd to mitigation");
                }

                logger.Log("Exiting.");
            }
            catch(System.Exception ex)
            {
                if(ex.Source == "OutlookInboxHandler")      //если ошибка в args или в Telegram Proxy
                {
                    logger.Log($"{ex.Message}\tExiting.");
                    var notification = await telegramNotificator.Notify($"ELKAddressAdder FAILED!\t{ex.Message}");
                }
                else if(ex.Source == "mscorlib")     //если закрыт OutLook - уведомить
                {
                    logger.Log("ERROR: Microsoft Outlook isn't running.\tExiting.");
                    var notification = await telegramNotificator.Notify($"ELKAddressAdder FAILED!\tMicrosoft Outlook isn't running");
                }
                else if (ex.Source == "Microsoft Outlook")        //если неверный путь к папке - уведомить
                {
                    logger.Log($"ERROR: {ex.Message}");
                    var notification = await telegramNotificator.Notify($"ELKAddressAdder FAILED!\t{ex.Message}");
                }
                else if (ex.Source == "WebDriver")   //если ошибка в работе с Арбор
                {
                    logger.Log($"{ex.Message}\tExiting.");
                    var notification = await telegramNotificator.Notify($"ELKAddressAdder FAILED!\t{ex.Message}");
                }
            }
        }
    }
}
