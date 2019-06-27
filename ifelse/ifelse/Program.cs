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
            int a;

            a = int.Parse(Console.ReadLine());

            if (a == 5)
            {
                Console.WriteLine("a равна 5");
            }
            else
            {
                Console.WriteLine("a не равна 5");
            }
        }
    }
}
