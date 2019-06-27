using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3numbers
{
    class Program
    {
        static void Main(string[] args)
        {
            double a, b, c;
            try
            {
                Console.Write("Введите первое число: ");
                a = double.Parse(Console.ReadLine());
                Console.Write("Введите второе число: ");
                b = double.Parse(Console.ReadLine());
                Console.Write("Введите третье число: ");
                c = double.Parse(Console.ReadLine());
                Console.WriteLine("Сумма " + a + ", " + b + " и " + c + " = " + (a + b + c));
                Console.WriteLine("Произведение " + a + ", " + b + " и " + c + " = " + a * b * c);
            }
            catch (Exception)
            {
                Console.WriteLine("Неверный формат ввода");
            }
            Console.WriteLine("Закрытие программы!");
        }
    }
}
