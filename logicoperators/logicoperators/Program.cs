using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logicoperators
{
    class Program
    {
        public static bool GetTemperature()
        {
            return true;
        }
        public static bool GetCoolingStatus()
        {
            return true;
        }
        static void Main(string[] args)
        {
            bool isInfected = false;

            if (!isInfected)
            {
                Console.WriteLine("Персонаж здоров");
            }


            bool isHighTemperature = true;
            bool hasNoCooling = false;

            if (isHighTemperature && hasNoCooling)
            {
                Console.WriteLine("Угроза повреждения процессора!");
            }
            if (isHighTemperature || hasNoCooling)
            {
                Console.WriteLine("Небольшая угроза повреждения процессора");
            }

            //& и | проверяют все условия, даже если первые false
            if(GetTemperature()  | GetCoolingStatus())
            {
                Console.WriteLine("Угроза повреждения процессора!");
            }
            if (GetTemperature() || GetCoolingStatus())
            {
                Console.WriteLine("Угроза повреждения процессора!");
            }
        }
    }
}
