using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using MihaZupan;

namespace OutlookInboxHandler
{
    class Program
    {
        enum Browser
        {
            Firefox,
            Chrome
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
            foreach(var octet in address)
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

        static void GetAddressesFromOutlook(ref List<string> addresses)
        {
            Console.Write("Connecting to Outlook...");
            NameSpace NS = (Marshal.GetActiveObject("Outlook.Application") as Application).GetNamespace("MAPI");    //здесь может выброситься ex.Source == "mscorlib"
            Console.Write("Done\nSearching for folder \\\\soc@RT.RU\\ELK...");
            //Folder folder = (Folder)NS.Folders["frbgd7@mail.ru"].Folders["test"];
            Folder folder = (Folder)NS.Folders["soc@RT.RU"].Folders["Входящие"].Folders["ELK"];       //здесь может выброситься ex.Source == "Microsoft Outlook"

            Console.WriteLine("Done\n\nMessages Scanning:");
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
                                    if(splitLine.Count() != 2 || IsInvalidCount(splitLine[0]) || splitLine[1].Split('.').Count() != 4 || IsInvalidAddress(splitLine[1].Split('.')))      //если неверный формат строки (файла), переходим к следующему файлу - добавить в уведомление
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

        static void AddToFilterList(List<string> addresses, Browser browser, string login, string pass)
        {
            Console.Write("Opening browser window...");
            OpenQA.Selenium.Remote.RemoteWebDriver driver;
            if(browser == 0)
            {
                driver = new FirefoxDriver();
            }
            else
            {
                driver = new ChromeDriver();
            }

            Console.Write("Done\nGoing to the navigation page...");
            driver.Navigate().GoToUrl("https://vpi1.soc.rt.ru/page?id=mitigation_status&mitigation_id=58640");      //здесь и далее в функции может выброситься ex.Source == "WebDriver"

            if (driver.Title.Contains("Login"))
            {
                Console.Write("Authorization needed...");
                driver.FindElement(By.Name("username")).SendKeys(login);
                driver.FindElement(By.Name("password")).SendKeys(pass);
                driver.FindElement(By.Name("Submit")).Click();
                Thread.Sleep(15000);
            }

            Console.Write("Done\nEditing BW Filter list...");
            driver.FindElement(By.CssSelector(".alt:nth-child(5) a")).Click();
            IWebElement filterForm = driver.FindElement(By.Name("filter_MitigationRealTimeExpandBWList_bcfea401019cccd2db81b44b4b11d7c9"));
            string firstFilter = filterForm.Text;
            string filter = firstFilter;
            foreach (string address in addresses)
            {
                filter = $"drop src host {address}\r\n{filter}";
            }
            filterForm.Clear();
            filterForm.SendKeys(filter);
            driver.FindElement(By.CssSelector(".tableheader:nth-child(8) .tick")).Click();

            Thread.Sleep(15000);
            Console.Write("Done\nChecking...");

            //проверка
            driver.Navigate().GoToUrl("https://vpi1.soc.rt.ru/page?id=mitigation_status&mitigation_id=58640");
            driver.FindElement(By.CssSelector(".alt:nth-child(5) a")).Click();
            filterForm = driver.FindElement(By.Name("filter_MitigationRealTimeExpandBWList_bcfea401019cccd2db81b44b4b11d7c9"));
            if (filterForm.Text != filter)
            {
                if (filterForm.Text != firstFilter)
                {
                    filterForm.Clear();
                    filterForm.SendKeys(firstFilter);
                    driver.FindElement(By.CssSelector(".tableheader:nth-child(8) .tick")).Click();
                    Thread.Sleep(15000);
                }
                driver.FindElement(By.ClassName("user")).FindElement(By.TagName("a")).Click();
                driver.Dispose();
                throw new System.Exception("Arbor ERROR: can't add addreses to filter lists") { Source = "WebDriver" };     //если адреса не добавились
            }
            Console.Write("Done\nLogging out and closing browser window...");

            driver.FindElement(By.ClassName("user")).FindElement(By.TagName("a")).Click();
            driver.Dispose();
            Console.WriteLine("Done\n");
        }

        static async Task<bool> ProxyAvailabilityChecking(HttpClient client)
        {
            try
            {
                var result = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://api.telegram.org/"));
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        static async Task<bool> TelegramNotification(List<string> addresses)
        {
            Console.Write("Checking Telegram proxy server...");

            var client = new HttpClient(new HttpClientHandler { Proxy = new HttpToSocks5Proxy("tmpx.soc.rt.ru", 1080, "cdc", "UZy58MNr2kW769s74Sn2dQ2xP7zKwLyy") }, true);

            if (!await ProxyAvailabilityChecking(client))
            {
                Console.Write("Error\nTrying another proxy server...");
                client = new HttpClient(new HttpClientHandler { Proxy = new HttpToSocks5Proxy("139.162.141.171", 31422, "pirates", "hmm_i_see_some_pirates_here_meeeew") }, true);
            }
            if (!await ProxyAvailabilityChecking(client))
            {
                Console.WriteLine("Error\nTelegram Proxy is unavailable!\n");
                return false;
            }
            Console.WriteLine("OK\nSending message...");

            string notificationBody = "";
            foreach(string address in addresses)
            {
                notificationBody = $"{notificationBody}{address}\n";
            }

            try
            {
                var result = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, $"https://api.telegram.org/bot952380349:AAGKIafp1PM4gMfZXBSodaJgLKwwHhiJmqE/sendMessage?chat_id=259571389&text=Addresses:{notificationBody}"));
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("ERROR\n");
                    return false;
                }
            }
            catch
            {
                Console.WriteLine("ERROR\n");
                return false;
            }

            Console.WriteLine("Done\n");
            return true;
        }

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine($"ELKAddressAdder started at {DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}");
                List<string> addresses = new List<string>();
                Browser browser = new Browser();

                if (args.Count() != 3 || args[1] == null || args[2] == null)
                {
                    throw new System.Exception("Parameters ERROR: Invalid parameters or number of parameters");
                }
                if (String.Compare($"Firefox", args[0], true) == 0)
                {
                    browser = Browser.Firefox;
                }
                else if(String.Compare($"Chrome", args[0], true) == 0)
                {
                    browser = Browser.Chrome;
                }
                else
                {
                    throw new System.Exception("Parameters ERROR: Invalid first parameter");
                }

                GetAddressesFromOutlook(ref addresses);

                addresses.Distinct().ToList<string>();

                if (addresses.Any())
                {
                    AddToFilterList(addresses, browser, args[1], args[2]);
                }

                bool status = await TelegramNotification(addresses);

                Console.WriteLine("Exiting.");
            }
            catch(System.Exception ex)
            {
                if(ex.Source == "OutlookInboxHandler")      //если ошибка в args или в Telegram Proxy
                {
                    Console.WriteLine($"{ex.Message}\nExiting.");
                }
                else if(ex.Source == "mscorlib")     //если закрыт OutLook - уведомить
                {
                    Console.WriteLine("ERROR: Microsoft Outlook isn't running.\nExiting.");
                }
                else if (ex.Source == "Microsoft Outlook")        //если неверный путь к папке - уведомить
                {
                    Console.WriteLine("ERROR: folder C:\\ELKAddress not found.\nExiting.");
                }
                else if (ex.Source == "WebDriver")   //если ошибка в работе с Арбор
                {
                    Console.WriteLine($"{ex.Message}\nExiting.");
                }
            }
        }
    }
}
