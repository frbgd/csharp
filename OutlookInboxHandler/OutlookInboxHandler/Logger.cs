using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OutlookInboxHandler
{
    public class Logger
    {
        string logFileName;

        private static Logger _logger;
        public static Logger SetGetLogger()
        {
            if (_logger != null)
                return _logger;
            else
            {
                _logger = new Logger();
                return _logger;
            }
        }
        protected Logger()
        {
            if (!Directory.Exists("C:\\ELKAddressAdder\\Logs"))
            {
                Directory.CreateDirectory("C:\\ELKAddressAdder\\Logs");
            }
            logFileName = $"ELKAddressAdder-{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-fff")}.log";
        }

        public void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff")}\t{message}");
            using (var sw = new StreamWriter($"C:\\ELKAddressAdder\\Logs\\{logFileName}", true, System.Text.Encoding.Default))
            {
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff")}\t{message}");
            }
        }
    }
}
