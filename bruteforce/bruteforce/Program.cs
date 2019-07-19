using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace bruteforce
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

                        string targetHostName;
                        if (String.IsNullOrEmpty(split[1]))
                            targetHostName = split[7];
                        else targetHostName = split[1];
                        string targetAddress;
                        if (targetHostName == split[7] || String.IsNullOrEmpty(split[2]))
                            targetAddress = split[8];
                        else targetAddress = split[2];

                        string target;
                        if (!String.IsNullOrEmpty(targetHostName) && !String.IsNullOrEmpty(targetAddress))
                            target = $"{targetHostName} - {targetAddress}";
                        else
                        {
                            if (String.IsNullOrEmpty(targetHostName))
                                target = targetAddress;
                            else
                                target = targetHostName;
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

                        string fullUsername;
                        if (String.IsNullOrEmpty(split[10]))
                            fullUsername = split[9];
                        else
                        {
                            if(split[10].Contains("\\"))
                                fullUsername = $"{split[10]}{split[9]}";
                            else
                                fullUsername = $"{split[10]}\\{split[9]}";
                        }

                        mailItem.Subject = $"Множественные неуспешные попытки входа({target}|{fullUsername})";
                        mailItem.CC = "RTSOC@rt.ru;soc2line@rt.ru;";
                        mailItem.Display(mailItem);
                        MailItem variable = mailItem;
                        string[] hTMLBody = new string[] { $@"<font size ='4'><p>Добрый день!</p><br><p>Зарегистрирован инцидент: Множественные неуспешные попытки входа <font color=""red"">под критичной УЗ</font>({target}|{fullUsername})</p>", null, null, null, null, null, null, null };
                        hTMLBody[1] = $"<p>=== Ключевая информация:===</p><p>Время детектирования: {split[0]}</p><p>Хост-инициатор: {atacker}</p><p>Целевой хост(источник событий): {target}</p><p>Целевая УЗ: {fullUsername}</p>";
                        if(split[14].Contains("Kerberos"))
                            hTMLBody[2] = $@"<p>Протокол аутентификации: Kerberos</p><br><p>===Подробная информация===</p><p>На хосте {atacker}{atackerZone} зафиксированы множественные попытки аутентификаций по протоколу Kerberos под <font color=""red"">критичной</font> УЗ {fullUsername}, которые были зарегистрированы на контроллере домена {target}.</p>";
                        else
                            hTMLBody[2] = $@"<p>Протокол аутентификации: {split[13]}</p><br><p>===Подробная информация===</p><p>С хоста {atacker}{atackerZone} зафиксированы множественные попытки аутентификаций по протоколу {split[13]} на хосте {target}{targetZone} под <font color=""red"">критичной</font> УЗ {fullUsername}</p>";
                        hTMLBody[3] = @"<p>На текущий момент активность <font color=""red"">продолжается\закончилась</font>.</p><p>Успешные аутентификации <font color=""red"">зарегистрированы в [время]\не зарегистрированы</font>.</p>";
                        hTMLBody[4] = @"<p>Начало активности: <font color=""red"">[постараться определить каналом по Target user name в пределах 1 суток. Если активность фиксируется на протяжении суток, то так и указать с пометкой что старт активности выявить не удалось]</font>.</p>";
                        hTMLBody[5] = $@"<p>Причина неуспешных аутентификаций: {split[15]}.</p><p>Количество неуспешных попыток: <font color=""red"">[определяем в среднем по каналу]</font>.</p>";
                        hTMLBody[6] = "<p>===Рекомендации в случае подтверждения инцидента===</p><p>Актуализировать учетные данные в сервисах запущенных на хосте инициаторе. Если попытки подбора инициированы не сервисами, изолировать хост-инициатор до выяснения причин инцидента.</p><p>Просьба проверить легитимность активности и, по возможности, сообщить о результатах (легитимно/нелегитимно/оповещать ли по подобным событиям в дальнейшем).</p>";
                        hTMLBody[7] = mailItem.HTMLBody;
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
