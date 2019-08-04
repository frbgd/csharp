using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using MihaZupan;

namespace OutlookInboxHandler
{
    class Program
    {
        static void GetAddressesFromOutlook(ref List<string> addresses)
        {
            NameSpace NS = (Marshal.GetActiveObject("Outlook.Application") as Application).GetNamespace("MAPI");
            //Folder folder = (Folder)NS.Folders["frbgd7@mail.ru"].Folders["test"];
            Folder folder = (Folder)NS.Folders["soc@RT.RU"].Folders["Входящие"].Folders["ELK"];

            foreach (MailItem mailItem in folder.Items)
            {
                if (mailItem.ReceivedTime.Hour == DateTime.Now.Hour)
                {
                    foreach (Attachment txt in mailItem.Attachments)
                    {
                        var path = $"C:\\ELK\\{mailItem.ConversationTopic}_{DateTime.Now.ToString("yyyy-MM-dd HH")}h";
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
            }
        }

        static void AddToFilterList(List<string> addresses)
        {
            IWebDriver driver = new FirefoxDriver();
            driver.Navigate().GoToUrl("https://vpi1.soc.rt.ru/page?id=mitigation_status&mitigation_id=58640");
            driver.FindElement(By.Name("username")).SendKeys("a.kucheryavenko");
            driver.FindElement(By.Name("password")).SendKeys("4_c`&MjLjq");
            driver.FindElement(By.Name("Submit")).Click();

            System.Threading.Thread.Sleep(10000);

            driver.FindElement(By.CssSelector(".alt:nth-child(5) a")).Click();
            IWebElement filterForm = driver.FindElement(By.Name("filter_MitigationRealTimeExpandBWList_bcfea401019cccd2db81b44b4b11d7c9"));
            string filter = filterForm.Text;
            foreach (string address in addresses)
            {
                filter = $"drop src host {address}\r\n{filter}";
            }
            filterForm.Clear();
            filterForm.SendKeys(filter);
            driver.FindElement(By.CssSelector(".tableheader:nth-child(8) .tick")).Click();

            System.Threading.Thread.Sleep(10000);
            driver.FindElement(By.ClassName("user")).FindElement(By.TagName("a")).Click();
        }

        static async Task<bool> TelegramNotification(List<string> addresses)
        {
            //var proxy = new HttpToSocks5Proxy("tmpx.soc.rt.ru", 1080, "cdc", "UZy58MNr2kW769s74Sn2dQ2xP7zKwLyy");
            var proxy = new HttpToSocks5Proxy("139.162.141.171", 31422, "pirates", "hmm_i_see_some_pirates_here_meeeew");
            var handler = new HttpClientHandler { Proxy = proxy };
            HttpClient client = new HttpClient(handler, true);

            string notificationBody = "";
            foreach(string address in addresses)
            {
                notificationBody = $"{notificationBody}{address}\n";
            }

            var result = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"https://api.telegram.org/bot952380349:AAGKIafp1PM4gMfZXBSodaJgLKwwHhiJmqE/sendMessage?chat_id=259571389&text=Addresses:{notificationBody}"));

            Console.WriteLine("HTTPS GET: " + await result.Content.ReadAsStringAsync());

            return true;
        }

        static async Task Main(string[] args)
        {
            try
            {
                List<string> addresses = new List<string>();

                GetAddressesFromOutlook(ref addresses);

                if (addresses.Any())
                {
                    AddToFilterList(addresses);
                }

                bool status = await TelegramNotification(addresses);
            }
            catch
            {

            }
        }
    }
}
