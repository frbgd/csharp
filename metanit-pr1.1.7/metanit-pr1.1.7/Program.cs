using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metanit_pr1._1._7
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите номер операции: 1.Сложение  2.Вычитание  3.Умножение");
            int operation = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Введите первое число: ");
            int num1 = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Введите второе число: ");
            int num2 = Convert.ToInt32(Console.ReadLine());

            switch (operation)
            {
                case 1:
                    Console.WriteLine("Сложение");
                    Console.WriteLine($"Результат: {num1 + num2}");
                    break;
                case 2:
                    Console.WriteLine("Вычитание");
                    Console.WriteLine($"Результат: {num1 - num2}");
                    break;
                case 3:
                    Console.WriteLine("Умножение");
                    Console.WriteLine($"Результат: {num1 * num2}");
                    break;
                default:
                    Console.WriteLine("Неизвестная операция");
                    break;
            }
            Console.ReadKey();
        }
    }
}
