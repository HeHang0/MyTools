using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileHelper
{
    public class LogHelper
    {
        public static object ob = new object();
        public static string LogPath = "Log";
        public static void WriteLog(string input)
        {
            lock (ob)
            {
                try
                {
                    File.AppendAllText($"{GetLogPath()}.txt", input);
                }
                catch { }
            }
        }

        private static string GetLogPath()
        {
            return Path.Combine(Path.GetDirectoryName(LogPath), DateTime.Now.ToString("yyyy-MM-dd"));
        }
    }
}
