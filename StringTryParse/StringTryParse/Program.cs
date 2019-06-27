using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace StringTryParse
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "5";
            int a = int.Parse(str);

            NumberFormatInfo numberFormatInfo = new NumberFormatInfo
            {
                NumberDecimalSeparator = ","
            };

            string str1 = "5,9";
            double b = double.Parse(str1, numberFormatInfo);

            string str2 = "5.9afgks";
            try
            {
                double c = double.Parse(str2);
                Console.WriteLine("Успешная конвертация");
            }
            catch (Exception)
            {
                Console.WriteLine("Неупешная конвертация");
            }

            string str3 = "2";

            int d;

            string str4 = "2jlsfguhs";

            bool result = int.TryParse(str3, out d);
            if(result)
            {
                Console.WriteLine("Успешно, значение = " + d);
            }
            else
            {
                Console.WriteLine("Неуспешно");
            }
            result = int.TryParse(str4, out d);
            if (result)
            {
                Console.WriteLine("Успешно, значение = " + d);
            }
            else
            {
                Console.WriteLine("Неуспешно");
            }
        }
    }
}
