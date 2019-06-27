using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace @while
{
    class Program
    {
        static void Main(string[] args)
        {
            uint limit;
            limit = uint.Parse(Console.ReadLine());
            uint count = 0;
            while (count < limit)
            {
                count++;
                Console.WriteLine(count);
            }
            Console.ReadLine();
        }
    }
}
