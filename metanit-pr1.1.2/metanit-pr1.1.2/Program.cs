using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metanit_pr1._1._2
{
    class Program
    {
        static void Main(string[] args)
        {
            int a;
            a = Convert.ToInt32(Console.ReadLine());
            if(a > 5 && a < 10)
                Console.WriteLine("Число больше 5 и меньше 10");
            else
                Console.WriteLine("Неизвестное число");
            Console.ReadKey();
        }
    }
}
