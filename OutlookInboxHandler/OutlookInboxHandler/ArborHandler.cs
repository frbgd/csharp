﻿using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;

namespace OutlookInboxHandler
{
    public class ArborHandler
    {
        Browser _browser;
        string _login;
        string _pass;
        public ArborHandler(string[] args)
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
        }
        public enum Browser
        {
            Firefox,
            Chrome
        }

        public void AddToFilterList(List<string> addresses)
        {
            Console.Write("Opening browser window...");
            OpenQA.Selenium.Remote.RemoteWebDriver driver;
            if (_browser == 0)
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
                driver.FindElement(By.Name("username")).SendKeys(_login);
                driver.FindElement(By.Name("password")).SendKeys(_pass);
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
    }
}