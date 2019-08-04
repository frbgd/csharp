﻿using Microsoft.Office.Interop.Outlook;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace OutlookInboxHandler
{
    public class OutlookChecker
    {
        NameSpace _NS;
        Folder _folder;
        Logger _logger;

        public OutlookChecker(Logger logger)
        {
            _logger = logger;
            _logger.Log("Connecting to Outlook...");
            _NS = (Marshal.GetActiveObject("Outlook.Application") as Application).GetNamespace("MAPI");    //здесь может выброситься ex.Source == "mscorlib"
            _logger.Log("Done");
            _logger.Log("Searching for folder \\\\soc@RT.RU\\ELK...");
            _folder = (Folder)_NS.Folders["soc@RT.RU"].Folders["Входящие"].Folders["ELK"];       //здесь может выброситься ex.Source == "Microsoft Outlook"
            _logger.Log("Done");
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
            _logger.Log("Messages Scanning started");
            int messageNumber = 0;
            foreach (MailItem mailItem in _folder.Items)
            {
                if (mailItem.ReceivedTime.Year == DateTime.Now.Year && mailItem.ReceivedTime.DayOfYear == DateTime.Now.DayOfYear && mailItem.ReceivedTime.Hour == DateTime.Now.Hour)
                {
                    _logger.Log($"Message {++messageNumber}");
                    if (mailItem.Attachments.Count > 0)
                    {
                        foreach (Attachment txt in mailItem.Attachments)
                        {
                            _logger.Log($"Saving attachment in the file C:\\ELKAddressAdder\\{mailItem.ConversationTopic}_{DateTime.Now.ToString("yyyy-MM-dd HH")}h.txt...");
                            if (!Directory.Exists("C:\\ELKAddressAdder"))
                            {
                                Directory.CreateDirectory("C:\\ELKAddressAdder");
                            }
                            var path = $"C:\\ELKAddressAdder\\{mailItem.ConversationTopic}_{DateTime.Now.ToString("yyyy-MM-dd HH")}h.txt";
                            txt.SaveAsFile(path);
                            _logger.Log("Done");

                            _logger.Log($"Reading file C:\\ELKAddressAdder\\{mailItem.ConversationTopic}_{DateTime.Now.ToString("yyyy-MM-dd HH")}h.txt");
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
                                        _logger.Log($"Address {splitLine[1]} added in list for adding.");
                                    }
                                }
                            }
                            _logger.Log("Reading file finished\tNext file.");
                        }
                    }
                    else            //если в письме нет вложений, переходим к следующему письму - добавить в уведомление
                    {
                        _logger.Log("Message have not attachments");
                    }
                    try
                    {
                        mailItem.Move(_folder.Folders["Done"]);
                        _logger.Log("Message moved to \\\\soc@RT.RU\\ELK\\Done");
                        _logger.Log("Next message");
                    }
                    catch       //не существует папка для перемещения, переходим к следующему письму - добавить в уведомление
                    {
                        _logger.Log("Message didn't move to \\\\soc@RT.RU\\ELK\\Done.");
                        _logger.Log("Next message");
                    }
                }
            }
            _logger.Log("Scanning finished.");
        }
    }
}
