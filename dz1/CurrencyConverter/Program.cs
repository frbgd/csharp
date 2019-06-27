using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            uint currency;
            Console.WriteLine("Перед Вами простейший конвертер валют (руб <=> $) на 28.06.2019");
            Console.Write("Выберите валюту для конвертации (0 - руб, 1 - $): ");
            if(!uint.TryParse(Console.ReadLine(), out currency) || (currency > 1))
            {
                Console.WriteLine("Неверный формат ввода");
            }
            else
            {
                if (currency == 0)
                {
                    try
                    {
                        Console.Write("Введите количество рублей: ");
                        double n = double.Parse(Console.ReadLine());
                        Console.WriteLine("Эквивалентная сумма долларов: " + n * 0.016);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Неверный формат ввода");
                    }
                }
                else
                {
                    try
                    {
                        Console.Write("Введите количество долларов: ");
                        double n = double.Parse(Console.ReadLine());
                        Console.WriteLine("Эквивалентная сумма рублей: " + n * 63.09);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Неверный формат ввода");
                    }
                }
            }
            Console.WriteLine("Закрытие программы!");
        }
    }
}
