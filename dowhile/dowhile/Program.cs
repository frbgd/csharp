using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dowhile
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 0;
            do
            {
                count++;
                Console.WriteLine(count);
            } while (count < 5);
            Console.ReadLine();
        }
    }
}
