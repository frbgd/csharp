using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace argstest_intcm
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Getting arguments...");
            if (args.ElementAtOrDefault<string>(0) != null)
            {
                string str = string.Join(" ", args);
                string[] split = str.Split(new char[] { ',' });
                Console.WriteLine("Done");
                int i = 0;
                foreach(string arg in split)
                {
                    Console.WriteLine($"{i++}\t{arg}");
                }
                Console.WriteLine("Exiting");
            }
        }
    }
}
