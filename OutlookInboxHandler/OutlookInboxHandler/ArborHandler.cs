using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace OutlookInboxHandler
{
    public class ArborHandler
    {
        Browser _browser;
        string _login;
        string _pass;
        string _mitigationId;
        Logger _logger;
        public ArborHandler(string[] args, string mitigationId, Logger logger)
        {
            if(String.Compare($"Firefox", args[0], true) == 0)
            {
                _browser = Browser.Firefox;
            }
            if (String.Compare($"Chrome", args[0], true) == 0)
            {
                _browser = Browser.Chrome;
            }
            _login = args[1];
            _pass = args[2];
            _mitigationId = mitigationId;
            _logger = logger;
        }
        public enum Browser
        {
            Firefox,
            Chrome
        }

        public List<string> DuplicateDeleting(List<string> addresses, string filter)
        {
            var newAddresses = new List<string>(addresses);
            var filterLines = filter.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach(var line in filterLines)
            {
                if(!line.StartsWith("drop src host"))
                {
                    continue;
                }
                else
                {
                    foreach(var address in addresses)
                    {
                        if (address == line.Split(' ')[3])
                        {
                            newAddresses.Remove(address);
                            _logger.Log($"Address {address} already in filter!");
                            break;
                        }
                    }
                }
                if (!newAddresses.Any())
                {
                    break;
                }
            }

            return newAddresses;
        }

        public void AddToFilterList(ref List<string> addresses)
        {
            _logger.Log("Opening browser window...");
            OpenQA.Selenium.Remote.RemoteWebDriver driver;
            if (_browser == 0)
            {
                driver = new FirefoxDriver();
            }
            else
            {
                driver = new ChromeDriver();
            }
            _logger.Log("Window opened");
            try
            {
                _logger.Log("Going to the navigation page...");
                driver.Url = $"https://vpi1.soc.rt.ru/page?id=mitigation_status&mitigation_id={_mitigationId}";     //здесь и далее в функции может выброситься ex.Source == "WebDriver"

                bool wasAuthorized = true;
                if (driver.Title.Contains("Login"))
                {
                    wasAuthorized = false;
                    _logger.Log("Authorization needed...");
                    driver.FindElement(By.Name("username")).SendKeys(_login);
                    driver.FindElement(By.Name("password")).SendKeys(_pass);
                    driver.FindElement(By.Name("Submit")).Click();
                    Thread.Sleep(10000);
                    if (driver.Title.Contains("Login"))
                    {
                        throw new Exception("Arbor ERROR: wrong credentials") { Source = "WebDriver" };
                    }
                }
                _logger.Log("Done");

                _logger.Log("Editing BW Filter list...");
                driver.FindElement(By.CssSelector(".alt:nth-child(5) a")).Click();
                IWebElement filterForm = driver.FindElement(By.Name("filter_MitigationRealTimeExpandBWList_bcfea401019cccd2db81b44b4b11d7c9"));
                string firstFilter = filterForm.Text;
                string filter = firstFilter;
                foreach (string address in addresses)
                {
                    filter = $"drop src host {address}\r\n{filter}";
                }

                addresses = DuplicateDeleting(addresses, filter);
                if (!addresses.Any())
                {
                    _logger.Log("All addresses from the current mailing list are already in the filter!");
                    _logger.Log("Closing browser window");
                    driver.Dispose();
                    return;
                }

                filterForm.Clear();
                filterForm.SendKeys(filter);
                driver.FindElement(By.CssSelector(".tableheader:nth-child(8) .tick")).Click();

                Thread.Sleep(10000);
                _logger.Log("Done");


                //проверка
                _logger.Log("Validating changes...");
                driver.Url = $"https://vpi1.soc.rt.ru/page?id=mitigation_status&mitigation_id={_mitigationId}";
                driver.FindElement(By.CssSelector(".alt:nth-child(5) a")).Click();
                filterForm = driver.FindElement(By.Name("filter_MitigationRealTimeExpandBWList_bcfea401019cccd2db81b44b4b11d7c9"));
                if (filterForm.Text != filter)
                {
                    if (filterForm.Text != firstFilter)
                    {
                        filterForm.Clear();
                        filterForm.SendKeys(firstFilter);
                        driver.FindElement(By.CssSelector(".tableheader:nth-child(8) .tick")).Click();
                        Thread.Sleep(10000);
                    }
                    driver.FindElement(By.ClassName("user")).FindElement(By.TagName("a")).Click();
                    throw new System.Exception("Arbor ERROR: can't add addreses to filter lists") { Source = "WebDriver" };     //если адреса не добавились
                }
                _logger.Log("Done");

                if (!wasAuthorized)
                {
                    _logger.Log("Logging out...");
                    driver.FindElement(By.ClassName("user")).FindElement(By.TagName("a")).Click();
                    _logger.Log("Done");
                }
            }
            catch(Exception ex)
            {
                _logger.Log("Closing browser window...");
                driver.Dispose();
                throw ex;
            }
            _logger.Log("Closing browser window...");
            driver.Dispose();
            _logger.Log("Done\n");
        }
    }
}
