﻿using Microsoft.Office.Interop.Outlook;
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
            if (args.ElementAtOrDefault<string>(0) != null)
            {
                string str = string.Join(" ", args);
                string[] split = str.Split(new char[] { ',' });
                if (Process.GetProcessesByName("OUTLOOK").Count<Process>() <= 0)
                {
                    Console.WriteLine("Невозможно создать письмо, так как Outlook не запущен.");
                    Console.WriteLine("Для завершения нажмите Enter");
                    Console.Read();
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
                        foreach (Account item in NS.Accounts)
                        {
                            if (item.DisplayName == "soc@rt.ru")
                            {
                                mailItem.SendUsingAccount = item;
                                break;
                            }
                        }

                        string rat;
                        if (split[4] == "22")
                            rat = "SSH";
                        else if (split[4] == "23")
                            rat = "Telnet";
                        else if (split[4] == "137")
                            rat = "NetBIOS Name Service";
                        else if (split[4] == "138")
                            rat = "NetBIOS Session Service";
                        else if (split[4] == "445")
                            rat = "SMB";
                        else if (split[4] == "3389" || split[4] == "13389")
                            rat = "RDP";
                        else if (split[4] == "2654" || split[4] == "5900" || split[4] == "5901" || split[4] == "5902" || split[4] == "5903" || split[4] == "5904")
                            rat = "VNC";
                        else if (split[4] == "4899")
                            rat = "RAdmin";
                        else if (split[4] == "5938")
                            rat = "Teamviewer";
                        else
                            rat = "Не определено";

                        mailItem.Subject = $"Использование программного обеспечения для удалённого доступа({split[2]} | {rat})";
                        mailItem.CC = "RTSOC@rt.ru;soc2line@rt.ru;";
                        mailItem.Display(mailItem);
                        MailItem variable = mailItem;
                        string[] hTMLBody = new string[] { $"<font size ='4'><p>Добрый день!</p><p>Зафиксирован инцидент: Использование программного обеспечения для удалённого доступа({split[2]} | {rat})</p>=== Ключевая информация:===<p>Время детектирования: {split[0]}</p>", null, null, null, null, null, null, null, null, null, null };
                        hTMLBody[1] = string.Format("<p>Протокол: {0}</p>", (split.ElementAtOrDefault<string>(1) != null ? split[1] : "Null"));
                        hTMLBody[2] = string.Format("<p>Инициатор активности: {0}", (split.ElementAtOrDefault<string>(2) != null ? string.Format("{0}", split[2]) : "Null"));
                        hTMLBody[3] = string.Format("<p>Активность детектирована: {0}</p>", (split.ElementAtOrDefault<string>(3) != null ? string.Format("{0}", split[3]) : "Null"));
                        hTMLBody[4] = "<p>===Подробная информация===</p>";
                        hTMLBody[5] = string.Format("С хоста {0}:{1} подсети {2}", (split.ElementAtOrDefault<string>(2) != null ? string.Format("{0}", split[2]) : "Null"), (split.ElementAtOrDefault<string>(4) != null ? string.Format("{0}", split[4]) : "Null"), (split.ElementAtOrDefault<string>(5) != null ? string.Format("{0}", split[5]) : "Null"));
                        hTMLBody[6] = string.Format(" (активная УЗ «{0}») зафиксирована успешная попытка подключения по протоколу {1} к хосту {2} - {3}:{4} ",(split.ElementAtOrDefault<string>(6) != null ? string.Format("{0}", split[6]) : "Null"), (split.ElementAtOrDefault<string>(1) != null ? string.Format("{0}", split[1]) : "Null"), (split.ElementAtOrDefault<string>(7) != null ? string.Format("{0}", split[7]) : "Null"), (split.ElementAtOrDefault<string>(8) != null ? string.Format("{0}", split[8]) : "Null"), (split.ElementAtOrDefault<string>(9) != null ? string.Format("{0}", split[9]) : "Null"));
                        hTMLBody[7] = string.Format("подсети {0}, что косвенно может свидетельствовать о использовании средств RAT {1}.", (split.ElementAtOrDefault<string>(10) != null ? string.Format("{0}", split[10]) : "Null"), rat);
                        hTMLBody[8] = @"<p style=""color: red""=>***Проверяем активность на Source и Dest-хостах по каналу Device Address, пытаемся найти признаки запуска предполагаемого RAT и описываем активность на них***</p>";
                        hTMLBody[9] = "<p>===Рекомендации в случае подтверждения инцидента===</p><p>Проверить хост на наличие нерегламентированного ПО и сервисов. Стороннее ПО удалить. Провести внеплановую проверку хоста средствами АВПО. Пользователю донести риски использования средств RAT на хосте.</p></font>";
                        hTMLBody[10] = mailItem.HTMLBody;
                        variable.HTMLBody = string.Concat(hTMLBody);
                    }
                    catch (System.Exception exception)
                    {
                        Console.WriteLine("Произошла ошибка при формировании письма.");
                        Console.WriteLine("Для завершения нажмите Enter");
                        Console.Read();
                        return;
                    }
                }
            }
            else
            {
                Console.WriteLine("Для завершения нажмите Enter");
                Console.Read();
            }
        }
    }
}
