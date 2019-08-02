using Microsoft.Office.Interop.Outlook;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;

namespace OutlookInboxHandler
{
    class Program
    {

        static void Main(string[] args)
        {
            NameSpace NS = (Marshal.GetActiveObject("Outlook.Application") as Application).GetNamespace("MAPI");

            Folder folder = (Folder)NS.Folders["frbgd7@mail.ru"].Folders["test"];

            List<string> addresses = new List<string>();

            foreach (MailItem mailItem in folder.Items)
            {
                if (mailItem.ReceivedTime.Hour == DateTime.Now.Hour)
                {
                    foreach (Attachment txt in mailItem.Attachments)
                    {
                        var path = $"C:\\test\\{mailItem.ConversationTopic}_{DateTime.Now.ToString("yyyy-MM-dd HH")}h";
                        txt.SaveAsFile(path);

                        using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                string[] splitLine = line.Trim().Split(' ');
                                if(Convert.ToInt32(splitLine[0]) >= 1000)
                                {
                                    addresses.Add(splitLine[1]);
                                }
                            }
                        }


                    }
                    
                }
            }
        }
    }
}
