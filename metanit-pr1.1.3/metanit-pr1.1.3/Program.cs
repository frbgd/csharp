using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metanit_pr1._1._3
{
    class Program
    {
        static void Main(string[] args)
        {
            int num;
            num = Convert.ToInt32(Console.ReadLine());
            if(num == 5 || num == 10)
                Console.WriteLine("Число либо равно 5, либо равно 10");
            else
                Console.WriteLine("Неизвестное число");
            Console.ReadKey();
        }
    }
}
