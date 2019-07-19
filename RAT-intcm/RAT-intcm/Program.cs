using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RAT_intcm
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
                        if(!soc)
                            Console.WriteLine("ERROR");

                        Console.Write("Creating messasge...");
                        string rat;
                        if (split[9] == "22")
                            rat = "SSH";
                        else if (split[9] == "23")
                            rat = "Telnet";
                        else if (split[9] == "137")
                            rat = "NetBIOS Name Service";
                        else if (split[9] == "138")
                            rat = "NetBIOS Session Service";
                        else if (split[9] == "445")
                            rat = "SMB";
                        else if (split[9] == "3389" || split[9] == "13389")
                            rat = "RDP";
                        else if (split[9] == "2654" || split[9] == "5900" || split[9] == "5901" || split[9] == "5902" || split[9] == "5903" || split[9] == "5904")
                            rat = "VNC";
                        else if (split[9] == "4899")
                            rat = "RAdmin";
                        else if (split[9] == "5938")
                            rat = "Teamviewer";
                        else
                            rat = "Не определено";

                        string target;
                        if (!String.IsNullOrEmpty(split[8]) && !String.IsNullOrEmpty(split[9]))
                            target = $"{split[8]} - {split[9]}";
                        else
                        {
                            if (String.IsNullOrEmpty(split[8]))
                                target = split[9];
                            else
                                target = split[8];
                        }
                        string atacker;
                        if (!String.IsNullOrEmpty(split[2]) && !String.IsNullOrEmpty(split[3]))
                            atacker = $"{split[2]} - {split[3]}";
                        else
                        {
                            if (String.IsNullOrEmpty(split[2]))
                                atacker = split[3];
                            else
                                atacker = split[2];
                        }
                        string device;
                        if (!String.IsNullOrEmpty(split[12]) && !String.IsNullOrEmpty(split[4]))
                            device = $"{split[12]} - {split[4]}";
                        else
                        {
                            if (String.IsNullOrEmpty(split[12]))
                                device = split[4];
                            else
                                device = split[12];
                        }

                        string atackerZone;
                        if (split[6].Contains("RFC1918"))
                            atackerZone = null;
                        else
                            atackerZone = $" сетевой зоны {split[6]}";
                        string targetZone;
                        if (split[11].Contains("RFC1918"))
                            targetZone = null;
                        else
                            targetZone = $" сетевой зоны {split[11]}";

                        string user;
                        if (String.IsNullOrEmpty(split[7]))
                            user = null;
                        else
                            user = $" (активная УЗ«{split[7]}»)";

                        mailItem.Subject = $"Использование программного обеспечения для удалённого доступа({atacker} | {rat})";
                        mailItem.CC = "RTSOC@rt.ru;soc2line@rt.ru;";
                        mailItem.Display(mailItem);
                        MailItem variable = mailItem;
                        string[] hTMLBody = new string[] { $"<p>Добрый день!</p><br><p>Зафиксирован инцидент: Использование программного обеспечения для удалённого доступа({atacker} | {rat})</p>=== Ключевая информация:===<p>Время детектирования: {split[0]}</p>", null, null, null, null, null, null, null, null, null, null };
                        hTMLBody[1] = $"<p>Протокол: {split[1]}</p>";
                        hTMLBody[2] = $"<p>Инициатор активности: {atacker}";
                        hTMLBody[3] = $"<p>Активность детектирована: {device}</p>";
                        hTMLBody[4] = "<p>===Подробная информация===</p>";
                        hTMLBody[5] = $"С хоста {atacker}:{split[5]}{atackerZone}";
                        hTMLBody[6] = $"{user} зафиксирована успешная попытка подключения по протоколу {split[1]} к хосту {target}:{split[10]}";
                        hTMLBody[7] = $"{targetZone}, что косвенно может свидетельствовать об использовании средств RAT {rat}.</p><br>";
                        hTMLBody[8] = @"<p style=""color: red"">***Проверяем активность на Source и Dest-хостах по каналу Device Address, пытаемся найти признаки запуска предполагаемого RAT и описываем активность на них***</p><br>";
                        hTMLBody[9] = "<p>===Рекомендации в случае подтверждения инцидента===</p><p>Проверить хост на наличие нерегламентированного ПО и сервисов. Стороннее ПО удалить. Провести внеплановую проверку хоста средствами АВПО. Пользователю донести риски использования средств RAT на хосте.</p>";
                        hTMLBody[10] = mailItem.HTMLBody;
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
