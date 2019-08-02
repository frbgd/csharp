using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using Microsoft.Office.Interop.Outlook;
using System;
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
                                if (Convert.ToInt32(splitLine[0]) >= 1000)
                                {
                                    addresses.Add(splitLine[1]);
                                }
                            }
                        }
                    }
                }
                else
                    break;
            }

            IWebDriver driver = new FirefoxDriver();
            driver.Navigate().GoToUrl("https://vpi1.soc.rt.ru/page?id=mitigation_status&mitigation_id=58640");
            driver.FindElement(By.Name("username")).SendKeys("a.kucheryavenko");
            driver.FindElement(By.Name("password")).SendKeys("4_c`&MjLjq");
            driver.FindElement(By.Name("Submit")).Click();

            driver.FindElement(By.CssSelector(".alt:nth-child(5) a"));
            IWebElement filterForm = driver.FindElement(By.Name("filter_MitigationRealTimeExpandBWList_bcfea401019cccd2db81b44b4b11d7c9"));
            string filter = filterForm.Text;
            foreach (string address in addresses)
            {
                filter = $"drop src host {address}\n{filter}";
            }
            filterForm.SendKeys(filter);
            driver.FindElement(By.CssSelector(".tableheader:nth-child(8) .tick"));

            driver.FindElement(By.LinkText("Log Out")).Click();
        }
    }
}
