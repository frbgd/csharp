using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metanit_pr1._1._6
{
    class Program
    {
        static void Main(string[] args)
        {
            int num;
            Console.Write("Введите номер операции: 1.Сложение 2.Вычитание 3.Умножение: ");
            num = Convert.ToInt32(Console.ReadLine());
            switch (num)
            {
                case 1:
                    Console.WriteLine("Сложение");
                    break;
                case 2:
                    Console.WriteLine("Вычитание");
                    break;
                case 3:
                    Console.WriteLine("Умножение");
                    break;
                default:
                    Console.WriteLine("Операция не определена");
                    break;
            }
            Console.ReadKey();
        }
    }
}
