using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;


[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace MCCMLogger 
{

    public static class MccMLogger
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("MainLog");
        public static bool DebugMode = false;

        public static void SetDebugMode()
        {
            bool temp;
            string Mode = ConfigurationManager.AppSettings["DebugMode"].ToLower();
            if (bool.TryParse(Mode, out temp))
            {
                DebugMode = temp;
            }
        }

        public static void Info(string buffer)
        {
            log.Info(buffer);
        }

        public static void Error(string buffer)
        {
            log.Error(buffer);
        }

        public static void Error(string buffer, Exception ex)
        {
            log.Error(buffer);
            log.Error(ex.ToString());
        }

        public static void Debug(string buffer)
        {
            if (DebugMode)
                log.Debug(buffer);
        }

       
        public static void WriteEventLog(string Message, System.Diagnostics.EventLogEntryType Type = System.Diagnostics.EventLogEntryType.Error)
        {
            try
            {
                System.Diagnostics.EventLog EL = new System.Diagnostics.EventLog() { Source = "MccM Manager" };
                EL.WriteEntry(Message, Type);
            }
            catch 
            { };
        }
    }
}




