using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metanit_pr1._1._5
{
    class Program
    {
        static void Main(string[] args)
        {
            double sum;
            sum = Convert.ToDouble(Console.ReadLine());
            if (sum < 100)
                sum *= 1.05;
            else if (sum < 200)
                sum *= 1.07;
            else
                sum *= 1.1;
            sum += 15;
            Console.WriteLine(sum);
            Console.ReadKey();
        }
    }
}
