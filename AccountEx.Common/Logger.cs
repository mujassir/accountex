using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public static class Logger
    {
        public static string ApplicationName { get; set; }
        public static void Log(string logMessage)
        {
            try
            {
                using (StreamWriter txtWriter = File.AppendText(LogFileName))
                {
                    txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    txtWriter.WriteLine("\n ");
                    txtWriter.WriteLine("\nInformation: {0}", logMessage);
                    txtWriter.WriteLine("\n ");
                    txtWriter.WriteLine("-------------------------------");
                }
            }
            catch (Exception ex)
            {
                EventLog.CreateEventSource(ApplicationName, logMessage + Environment.NewLine + ex.ToString());
            }
        }

        public static void Log(Exception ex)
        {
            try
            {
                using (StreamWriter txtWriter = File.AppendText(LogFileName))
                {
                    txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                    txtWriter.WriteLine("\n ");
                    txtWriter.WriteLine("\nError: {0}", ex.Message);
                    txtWriter.WriteLine("\n ");
                    txtWriter.WriteLine("\nStackTrace: {0}", ex.StackTrace);
                    txtWriter.WriteLine("\n ");
                    txtWriter.WriteLine("-------------------------------");
                }
            }
            catch (Exception ex2)
            {
                EventLog.CreateEventSource(ApplicationName, ex.Message + Environment.NewLine + ex2.ToString());
            }
        }
        private static string LogFileName
        {
            get
            {
                var m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var dir = Path.Combine(m_exePath, "Logs");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                dir = Path.Combine(dir, DateTime.Now.ToString("MMMM"));
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var fileName = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                return fileName;
            }
        }
    }
}
