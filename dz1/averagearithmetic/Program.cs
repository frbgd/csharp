using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace averagearithmetic
{
    class Program
    {
        static void Main(string[] args)
        {
            double a, b;
            try
            {
                Console.Write("Введите первое число: ");
                a = double.Parse(Console.ReadLine());
                Console.Write("Введите второе число: ");
                b = double.Parse(Console.ReadLine());
                Console.WriteLine("Среднее арифметическое " + a + " и " + b + " = " + (a + b) / 2);
            }
            catch (Exception)
            {
                Console.WriteLine("Неверный формат ввода");
            }
            Console.WriteLine("Закрытие программы!");
        }
    }
}
