using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metanit_pr1._2._4
{
    class Program
    {
        static void Main(string[] args)
        {
            int num1;
            int num2;
            while (true)
            {
                Console.Write("Введите первое число от 0 до 10: ");
                num1 = Convert.ToInt32(Console.ReadLine());

                Console.Write("Введите второе число от 0 до 10: ");
                num2 = Convert.ToInt32(Console.ReadLine());

                if (num1 >= 0 && num1 <= 10 && num2 >= 0 && num2 <= 10)
                    break;
            }

            Console.WriteLine(num1*num2);
            Console.ReadKey();
        }
    }
}
