using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace OutlookInboxHandler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Initializing...");

            string progName = null;
            string windowsFolderPath = null;
            string botToken = null;
            int chatId = 0;
            string mailFolderPath = null;
            int treshold = 0;
            int mitigationId = 0;

            try
            {
                using (StreamReader sr = new StreamReader("config1.txt", System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("#"))
                        {
                            continue;
                        }
                        var splittedLine = line.Split(',');
                        if (splittedLine.Count() != 7 || Convert.ToInt32(splittedLine[3]) < 1 || Convert.ToInt32(splittedLine[5]) < 1 || Convert.ToInt32(splittedLine[6]) < 1)
                        {
                            throw new Exception("Invalid values in config.txt");
                        }
                        progName = splittedLine[0].Trim();
                        windowsFolderPath = splittedLine[1].Trim();
                        botToken = splittedLine[2].Trim();
                        chatId = Convert.ToInt32(splittedLine[3]);
                        mailFolderPath = splittedLine[4].Trim();
                        treshold = Convert.ToInt32(splittedLine[5]);
                        mitigationId = Convert.ToInt32(splittedLine[6]);
                        break;
                    }
                }
                if(String.IsNullOrEmpty(progName) || String.IsNullOrEmpty(windowsFolderPath) || windowsFolderPath[1] != ':' || windowsFolderPath[2] != '\\' || String.IsNullOrEmpty(botToken) || String.IsNullOrEmpty(mailFolderPath) || chatId == 0 || treshold == 0 || mitigationId == 0)
                {
                    throw new Exception("Invalid values in config.txt");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Initialization failed!");
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("Initialization done");

            var logger = Logger.SetGetLogger(progName, windowsFolderPath);
            logger.Log($"{progName} started on {System.Environment.MachineName}");
            var telegramNotificator = TelegramNotificator.SetGetNotificator(logger, chatId.ToString(), botToken);
            try
            {
                var notification = await telegramNotificator.Notify($"{progName} started on {System.Environment.MachineName}");


                if (args.Count() != 3 || args[1] == null || args[2] == null || ((String.Compare($"Firefox", args[0], true) != 0) && (String.Compare($"Chrome", args[0], true) != 0)))
                {
                    throw new System.Exception("Parameters ERROR: Invalid parameters or number of parameters");
                }

                List<string> addresses = new List<string>();

                var outlookChecker = new OutlookChecker(treshold, mailFolderPath, progName, windowsFolderPath, logger);

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

                    var arborHandler = new ArborHandler(args, mitigationId.ToString(), logger);
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
                if(ex.Source == "mscorlib")     //если закрыт OutLook - уведомить
                {
                    logger.Log("ERROR: Microsoft Outlook isn't running");
                    var notification = await telegramNotificator.Notify($"{progName} FAILED!\tMicrosoft Outlook isn't running");
                    logger.Log("Exiting");
                }
                else if (ex.Source == "Microsoft Outlook")        //если неверный путь к папке - уведомить
                {
                    logger.Log($"ERROR: Mail folder {mailFolderPath} doesn't exists");
                    var notification = await telegramNotificator.Notify($"{progName} FAILED!\tMail folder {mailFolderPath} doesn't exists");
                    logger.Log("Exiting");
                }
                else
                {
                    logger.Log($"{ex.Message}");
                    var notification = await telegramNotificator.Notify($"{progName} FAILED!\t{ex.Message}");
                    logger.Log("Exiting");
                }
            }
        }
    }
}