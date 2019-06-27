using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dz2
{
    class Program
    {
        static void Main(string[] args)
        {
            int a;
            Console.Write("Введите число: ");
            a = int.Parse(Console.ReadLine());

            if (a % 2 == 0)
            {
                Console.WriteLine(a + " - чётное");
            }
            else
            {
                Console.WriteLine(a + " - нечётное");
            }
        }
    }
}
