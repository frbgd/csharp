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
        string _mailFolderPath;
        string _windowsFolderPath;
        NameSpace _NS;
        Folder _folder;
        Logger _logger;

        public OutlookChecker(string mailFolderPath, string windowsFolderPath, Logger logger)
        {
            _mailFolderPath = mailFolderPath;
            _windowsFolderPath = windowsFolderPath;
            _logger = logger;
            _logger.Log("Connecting to Outlook...");
            _NS = (Marshal.GetActiveObject("Outlook.Application") as Application).GetNamespace("MAPI");    //здесь может выброситься ex.Source == "mscorlib"
            _logger.Log("Done");
            _logger.Log($"Searching for {_mailFolderPath}...");
            foreach(var folder in _mailFolderPath.Split('\\'))
            {
                _folder = (Folder)_NS.Folders[folder];
            }
            //_folder = (Folder)_NS.Folders["soc@RT.RU"].Folders["Входящие"].Folders["ELK"];       //здесь может выброситься ex.Source == "Microsoft Outlook"
            _logger.Log("Done");
            _logger.Log($"Searching for directory {_windowsFolderPath}");
            if (!Directory.Exists($"{_windowsFolderPath}"))
            {
                _logger.Log("Creating tt...");
                Directory.CreateDirectory($"{_windowsFolderPath}");
            }
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
                            _logger.Log($"Saving attachment in the file {_windowsFolderPath}\\{mailItem.ConversationTopic}_{DateTime.Now.ToString("yyyy-MM-dd HH")}h.txt...");
                            var path = $"{_windowsFolderPath}\\{mailItem.ConversationTopic}_{DateTime.Now.ToString("yyyy-MM-dd HH")}h.txt";
                            txt.SaveAsFile(path);
                            _logger.Log("Done");

                            _logger.Log($"Reading file {_windowsFolderPath}\\{mailItem.ConversationTopic}_{DateTime.Now.ToString("yyyy-MM-dd HH")}h.txt");
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
                    _logger.Log("Next message");
                }
            }
            _logger.Log("Scanning finished.");
        }
    }
}
