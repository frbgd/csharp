using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metanit_pr1._2._2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите сумму вклада: ");
            decimal sum = Convert.ToDecimal(Console.ReadLine());

            Console.Write("Введите количество месяцев: ");
            int monthNum = Convert.ToInt32(Console.ReadLine());

            int i = 0;
            while(i < monthNum)
            {
                sum *= 1.07M;
                i++;
            }

            Console.WriteLine($"Сумма по истечению {monthNum} месяцев: {sum}");
            Console.ReadKey();
        }
    }
}
