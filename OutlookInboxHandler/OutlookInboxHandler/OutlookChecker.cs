using Microsoft.Office.Interop.Outlook;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace OutlookInboxHandler
{
    public class OutlookChecker
    {
        NameSpace NS;
        Folder folder;

        public OutlookChecker()
        {
            Console.Write("Connecting to Outlook...");
            NS = (Marshal.GetActiveObject("Outlook.Application") as Application).GetNamespace("MAPI");    //здесь может выброситься ex.Source == "mscorlib"
            Console.Write("Done\nSearching for folder \\\\soc@RT.RU\\ELK...");
            //Folder folder = (Folder)NS.Folders["frbgd7@mail.ru"].Folders["test"];
            folder = (Folder)NS.Folders["soc@RT.RU"].Folders["Входящие"].Folders["ELK"];       //здесь может выброситься ex.Source == "Microsoft Outlook"
            Console.WriteLine("Done\n");
        }

        static bool IsInvalidCount(string count)
        {
            try
            {
                if (Convert.ToInt32(count) < 0)
                {
                    return true;            //проверка на положительное значение
                }
            }
            catch
            {
                return true;        //проверка на то, что количество адресов - число
            }
            return false;
        }
        static bool IsInvalidAddress(string[] address)
        {
            foreach (var octet in address)
            {
                try
                {
                    if (Convert.ToInt32(octet) < 0 || Convert.ToInt32(octet) > 255)
                    {
                        return true;        //проверка на валидность чисел в октетах
                    }
                }
                catch
                {
                    return true;        //проверка на то, что октет - число
                }
            }
            return false;
        }

        public void GetAddressesFromOutlook(ref List<string> addresses)
        {
            Console.WriteLine("Messages Scanning:");
            int messageNumber = 0;
            foreach (MailItem mailItem in folder.Items)
            {
                if (mailItem.ReceivedTime.DayOfYear == DateTime.Now.DayOfYear && mailItem.ReceivedTime.Hour == DateTime.Now.Hour)
                {
                    Console.WriteLine($"Message {++messageNumber}");
                    if (mailItem.Attachments.Count > 0)
                    {
                        foreach (Attachment txt in mailItem.Attachments)
                        {
                            Console.Write($"Saving attachment in the file C:\\ELKAddressAdder\\{mailItem.ConversationTopic}_{DateTime.Now.ToString("yyyy-MM-dd HH")}h.txt...");
                            if (!Directory.Exists("C:\\ELK"))
                            {
                                Directory.CreateDirectory("C:\\ELKAddressAdder");
                            }
                            var path = $"C:\\ELKAddressAdder\\{mailItem.ConversationTopic}_{DateTime.Now.ToString("yyyy-MM-dd HH")}h.txt";
                            txt.SaveAsFile(path);

                            Console.WriteLine($"Done\nReading file C:\\ELKAddressAdder\\{mailItem.ConversationTopic}_{DateTime.Now.ToString("yyyy-MM-dd HH")}h.txt");
                            using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                            {
                                string line;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    string[] splitLine = line.Trim().Split(' ');
                                    if (splitLine.Count() != 2 || IsInvalidCount(splitLine[0]) || splitLine[1].Split('.').Count() != 4 || IsInvalidAddress(splitLine[1].Split('.')))      //если неверный формат строки (файла), переходим к следующему файлу - добавить в уведомление
                                    {
                                        break;
                                    }
                                    if (Convert.ToInt32(splitLine[0]) >= 1000)
                                    {
                                        addresses.Add(splitLine[1]);
                                        Console.WriteLine($"Address {splitLine[1]} added in list for adding.");
                                    }
                                }
                            }
                            Console.WriteLine("Done\nNext file.");
                        }
                    }
                    else            //если в письме нет вложений, переходим к следующему письму - добавить в уведомление
                    {
                        Console.Write("Message have not attachments.\t");
                    }
                    try
                    {
                        mailItem.Move(folder.Folders["Done"]);
                        Console.Write("Message moved to \\\\soc@RT.RU\\ELK\\Done.\t");
                    }
                    catch       //не существует папка для перемещения, переходим к следующему письму - добавить в уведомление
                    {
                        Console.Write("Message didn't move to \\\\soc@RT.RU\\ELK\\Done.\t");
                        Console.WriteLine("Next message.\n");
                    }
                    Console.WriteLine("Next message.\n");
                }
            }
            Console.WriteLine("End of messages.\n");
        }
    }
}
