using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metanit_pr1._2._3
{
    class Program
    {
        static void Main(string[] args)
        {
            for(int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                    Console.Write($"{i * j}\t");
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
