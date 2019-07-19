using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace metanit_pr1._3._1
{
    class Program
    {
        static void Main(string[] args)
        {
            int[,,] mas = { { { 1, 2 },{ 3, 4 } },
                { { 4, 5 }, { 6, 7 } },
                { { 7, 8 }, { 9, 10 } },
                { { 10, 11 }, { 12, 13 } }
              };
            Console.Write("{");
            for(int i = 0; i <= mas.GetUpperBound(0); i++)
            {
                Console.Write("{");
                for(int j = 0; j <= mas.GetUpperBound(1); j++)
                {
                    Console.Write("{");
                    for (int k = 0; k <= mas.GetUpperBound(2); k++)
                    {
                        Console.Write(k == mas.GetUpperBound(2) ? $"{mas[i,j,k]}" : $"{mas[i, j, k]},");
                    }
                    Console.Write(j == mas.GetUpperBound(1) ? "}" : "},");
                }
                Console.Write(i == mas.GetUpperBound(0) ? "}" : "},");
            }
            Console.WriteLine("}");
            Console.ReadKey();
        }
    }
}
