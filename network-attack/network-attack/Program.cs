using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace network_attack
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Getting arguments...");
            if (args.ElementAtOrDefault<string>(0) != null)
            {
                string str = string.Join(" ", args);
                string[] split = str.Split(new char[] { ',' });
                Console.Write("Done\nGetting Outlook...");
                if (Process.GetProcessesByName("OUTLOOK").Count<Process>() <= 0)
                {
                    Console.WriteLine("ERROR\nUnable to create message. Outlook is not running!\nExiting.");
                }
                else
                {
                    try
                    {
                        Microsoft.Office.Interop.Outlook.Application activeObject = Marshal.GetActiveObject("Outlook.Application") as Microsoft.Office.Interop.Outlook.Application;
                        Microsoft.Office.Interop.Outlook.Application Application = activeObject;
                        Application = activeObject;
                        NameSpace NS = Application.GetNamespace("MAPI");
                        MailItem mailItem = (MailItem)((dynamic)Application.CreateItem(OlItemType.olMailItem));
                        NameSpace session = mailItem.Session;
                        Console.Write("Done\nSearching for soc@RT.RU mailbox...");
                        bool soc = false;
                        foreach (Account item in NS.Accounts)
                        {
                            if (String.Compare(item.DisplayName, "soc@RT.RU", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                mailItem.SendUsingAccount = item;
                                soc = true;
                                Console.WriteLine("Done");
                                break;
                            }
                        }
                        if (!soc)
                            Console.WriteLine("ERROR");

                        Console.Write("Creating messasge...");
                        
                        string target;
                        if (!String.IsNullOrEmpty(split[1]) && !String.IsNullOrEmpty(split[1]))
                            target = $"{split[1]} - {split[2]}";
                        else
                        {
                            if (String.IsNullOrEmpty(split[1]))
                                target = split[2];
                            else
                                target = split[1];
                        }
                        string atacker;
                        if (!String.IsNullOrEmpty(split[3]) && !String.IsNullOrEmpty(split[4]))
                            atacker = $"{split[3]} - {split[4]}";
                        else
                        {
                            if (String.IsNullOrEmpty(split[3]))
                                atacker = split[4];
                            else
                                atacker = split[3];
                        }
                        string device;
                        if (!String.IsNullOrEmpty(split[5]) && !String.IsNullOrEmpty(split[6]))
                            device = $"{split[5]} - {split[6]}";
                        else
                        {
                            if (String.IsNullOrEmpty(split[5]))
                                device = split[6];
                            else
                                device = split[5];
                        }

                        string atackerZone;
                        if (split[11].Contains("RFC1918"))
                            atackerZone = null;
                        else
                            atackerZone = $" сетевой зоны {split[11]}";
                        string targetZone;
                        if (split[12].Contains("RFC1918"))
                            targetZone = null;
                        else
                            targetZone = $" сетевой зоны {split[12]}";

                        mailItem.Subject = $"Обнаружение сетевых атак с системы из частной подсети({atacker})";
                        mailItem.CC = "RTSOC@rt.ru;soc2line@rt.ru;";
                        mailItem.Display(mailItem);
                        MailItem variable = mailItem;
                        string[] hTMLBody = new string[] { $"<font size ='4'><p>Добрый день!</p><br><p>Зафиксирован инцидент: Обнаружение сетевых атак с системы из частной подсети({atacker})</p>", null, null, null, null, null, null, null, null };
                        hTMLBody[1] = $"<p>=== Ключевая информация:===</p><p>Время детектирования: {split[0]}</p><p>Целевая система: {target}</p><p>Инициатор активности: {atacker}</p><p>Источник событий: {device}</p><p>Протокол: {split[7]}</p><p>Device Action: {split[8]}</p><br>";
                        hTMLBody[2] = $"<p>===Подробная информация===</p><p>Системой {split[9]} - {split[10]} ({device}) выявлено срабатывание критичной сигнатуры IPS при обращении на систему {target}{targetZone} с хоста {atacker}{atackerZone}.</p>";
                        hTMLBody[3] = $@"<p>Сообщение IPS\IDS: {split[13]}</p><br><br><p>Согласно данным {device}, срабатывания сигнатур к другим хостам <font color=""red"">не выявлено\выявлено к хостам</font>:</p>";
                        hTMLBody[4] = @"<p><font color=""red"">-список</font></p><p><font color=""red"">-список</font></p>";
                        hTMLBody[5] = @"<p><font color=""red"">\\Если зарегистрирована не одна сигнатура.</font>Во вложении подробная выгрузка по зарегистрированным аномалиям с данного источника.</p>";
                        hTMLBody[6] = @"<p>На текущий момент статус аномалии: <font color=""red"">продолжается\завершена</font>.</p><br><br>";
                        hTMLBody[7] = "<p>===Рекомендации в случае подтверждения инцидента===</p><p>Определить процесс-инициатор сетевой активности. Проверить хост-инициатор на наличие нерегламентированного ПО и сервисов. Стороннее ПО удалить. Провести внеплановую проверку хоста средствами АВПО. Проверить применимость эксплуатируемой атаки к целевой системе.</p><p>Просьба проверить легитимность активности и, по возможности, сообщить о результатах(легитимно/ нелегитимно / оповещать ли по подобным событиям в дальнейшем).</p>";
                        hTMLBody[8] = mailItem.HTMLBody;
                        variable.HTMLBody = string.Concat(hTMLBody);
                        Console.WriteLine("Done");
                    }
                    catch (System.Exception exception)
                    {
                        Console.WriteLine("ERROR.");
                        Console.WriteLine(exception.Message);
                        Console.WriteLine("Exiting.");
                        return;
                    }
                }
            }
            else
            {
                Console.WriteLine("ERROR");
            }
            Console.WriteLine("Exiting.");
        }
    }
}
