using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metanit_pr1._1._1
{
    class Program
    {
        static void Main(string[] args)
        {
            int a, b;
            a = Convert.ToInt32(Console.ReadLine());
            b = Convert.ToInt32(Console.ReadLine());
            if(a > b)
                Console.WriteLine("a > b");
            else if(a < b)
                Console.WriteLine("a < b");
            else
                Console.WriteLine("a == b");
            Console.ReadKey();
        }
    }
}
