using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metanit_pr1._2._1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите сумму вклада: ");
            decimal sum = Convert.ToDecimal(Console.ReadLine());

            Console.Write("Введите количество месяцев: ");
            int monthNum = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < monthNum; i++)
            {
                sum = sum * 1.07M;
            }

            Console.WriteLine($"Сумма по истечению {monthNum} месяцев: {sum}");
            Console.ReadKey();
        }
    }
}
