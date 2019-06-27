using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inc_dec
{
    class Program
    {
        static void Main(string[] args)
        {
            int a = 0;
            Console.WriteLine(a++); //0
            int b = 0;
            Console.WriteLine(++b); //1

            int c = 1;
            c = ++c * c;
            Console.WriteLine(c); //4

            int d = 1;
            d = d++ * d;
            Console.WriteLine(d);
        }
    }
}
