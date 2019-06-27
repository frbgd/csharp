using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ifelse
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
            if (op == "+")
            {
                Console.WriteLine("Результат сложения: " + (a + b));
            }
            else if (op == "-")
            {
                Console.WriteLine("Результат вычитания: " + (a - b));
            }
            else if (op == "*")
            {
                Console.WriteLine("Результат умножения: " + (a * b));
            }
            else if (op == "/")
            {
                Console.WriteLine("Результат деления: " + (a / b));
            }
            else
            {
                Console.WriteLine("Вы ввели неизвестный символ");
            }
        }
    }
}
