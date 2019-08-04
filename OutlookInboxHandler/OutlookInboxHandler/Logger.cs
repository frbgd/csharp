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
        string _logFileName;
        string _progName;
        string _windowsFolderPath;

        private static Logger _logger;
        public static Logger SetGetLogger(string progName, string windowsFolderPath)
        {
            if (_logger != null)
                return _logger;
            else
            {
                _logger = new Logger(progName, windowsFolderPath);
                return _logger;
            }
        }
        protected Logger(string progName, string windowsFolderPath)
        {
            _windowsFolderPath = windowsFolderPath;
            _progName = progName;
            _logFileName = $"{_progName}-{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff")}.log";
            if (!Directory.Exists(_windowsFolderPath))
            {
                Directory.CreateDirectory($"{_windowsFolderPath}\\Logs");
            }
        }

        public void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}\t{message}");
            using (var sw = new StreamWriter($"{_windowsFolderPath}\\Logs\\{_logFileName}", true, System.Text.Encoding.Default))
            {
                sw.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}\t{message}");
            }
        }
    }
}
