using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace @switch
{
    class Program
    {
        static void Main(string[] args)
        {
            double a, b;
            Console.Write("Введите первый операнд: ");
            a = double.Parse(Console.ReadLine());
            Console.Write("Введите второй операнд: ");
            b = double.Parse(Console.ReadLine());

            string op;
            Console.Write("Введите операцию: ");
            op = Console.ReadLine();
            switch (op)
            {
                case "+":
                    Console.WriteLine("Результат сложения: " + (a + b));
                    break;
                case "-":
                    Console.WriteLine("Результат вычитания: " + (a - b));
                    break;
                case "*":
                    Console.WriteLine("Результат умножения: " + (a * b));
                    break;
                case "/":
                    Console.WriteLine("Результат деления: " + (a / b));
                    break;
                default:
                    Console.WriteLine("Вы ввели неизвестный символ");
                    break;
            }
        }
    }
}
