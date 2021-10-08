using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using Notifications.OuterService.XMPP.Interfaces;

namespace Notifications.OuterService.XMPP.Realizations
{
    public class NLogger : Interfaces.ILogger
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private string logname;

        public NLogger(string _logname)
        {
            logname = _logname;
        }

        private void Config()
        {
            var filename = Path.Combine(Path.GetDirectoryName(logname), Path.GetFileNameWithoutExtension(logname) + "_" + DateTime.Now.Date.ToString("yyyyMMdd") + Path.GetExtension(logname));
            //config Nlog
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = filename };
            config.AddRule(LogLevel.Info, LogLevel.Error, logfile);
            LogManager.Configuration = config;
        }

        public void Error(string message)
        {
            Config();
            Logger.Error(message);
        }

        public void Info(string message)
        {
            Config();
            Logger.Info(message);
        }


        private IEnumerable<string> ReadLogFile(string filename, DateTime datebegin, DateTime dateend)
        {
            IList<string> result = new List<string>();

            using (StreamReader reader = new StreamReader(filename, Encoding.GetEncoding(1251)))
            {

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    DateTime date = DateTime.MinValue;

                    if (!String.IsNullOrEmpty(line) && line.IndexOf("|") > -1 && DateTime.TryParse(line.Substring(0, line.IndexOf("|")), out date))
                    {
                        if (date >= new DateTime(datebegin.Year, datebegin.Month, datebegin.Day, 0, 0, 0) && date <= new DateTime(dateend.Year, dateend.Month, dateend.Day, 23, 59, 59))
                        {

                            result.Add(line);

                        }
                    }
                    else
                    {
                        if (result.Count() > 0)
                        {
                            result[result.Count() - 1] = result.Last() + "\r\n" + line;
                        }
                    }


                }

                reader.Close();
            }

            return result;
        }

        public IEnumerable<string> ReadLog(DateTime datebegin, DateTime dateend)
        {
            var logDirectory = Path.GetDirectoryName(logname);
            var logExtension = Path.GetExtension(logname);
            IEnumerable<string> lines = Enumerable.Empty<string>();

            if (Directory.Exists(logDirectory))
            {

                foreach (string filename in GetFilesNames(logDirectory, logExtension, datebegin, dateend))
                {
                    lines = lines.Concat(ReadLogFile(filename, datebegin, dateend));
                }

            }

            return lines;
        }

        //Get files which names in period
        private IEnumerable<string> GetFilesNames(string directory, string extension, DateTime datebedin, DateTime dateend)
        {
            try
            {
                return Directory.GetFiles(directory, "*" + extension, SearchOption.AllDirectories).Where(f => InPeriod(f, datebedin, dateend));
            }
            catch
            {

                return Enumerable.Empty<string>();
            }
        }

        //Check filename in period
        private bool InPeriod(string fullfilename, DateTime datebedin, DateTime dateend)
        {
            try
            {
                string filename = Path.GetFileNameWithoutExtension(fullfilename);
                string filedate = filename.Substring(filename.Length - 8);
                DateTime datefile = DateTime.ParseExact(filedate, "yyyyMMdd", null);
                var begin = new DateTime(datebedin.Year, datebedin.Month, datebedin.Day);
                var end = new DateTime(dateend.Year, dateend.Month, dateend.Day);

                if (datefile >= begin && datefile <= end)
                    return true;

                return false;
            }
            catch
            {
                return false;
            }


        }

    }
}
