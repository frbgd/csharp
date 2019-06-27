using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace operators
{
    class Program
    {
        static void Main(string[] args)
        {
            int a = 2;
            int b = 5;
            double result = a / b;
            Console.WriteLine(result); //0

            double c = 2;
            result = c / b;
            Console.WriteLine(result); //0.4

            result = (double)a / b;
            Console.WriteLine(result); //0.4


            a = 10;
            b = 3;
            result = a % b;
            Console.WriteLine(result);
        }
    }
}
