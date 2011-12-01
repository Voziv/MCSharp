using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MCSharp
{
    public static class Logger
    {
        const string LogDirectory = "logs/";
        const string LogFileName = "mcsharp";
        const string ChatLogFileName = "chat";
        const string ErrorLogFileName = "error";
        const string TimeFormat = "hh:mm:ss tt";

        static string shortDateTime = "dd/MM/yyyy hh:mm:ss tt";
        static string longDateTime = "dddd, MMMM dd, yyyy - hh:mm tt (K)";

        static string SessionStart = DateTime.Now.ToString(longDateTime);

        static LogType chatTypes;
        static LogType errorTypes;

        public delegate void LogHandler(string message, LogType type, LogSource src);
        public static event LogHandler OnLog;

        static Logger()
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }

            // Get our log files ready
            PrepareLogFile(LogFileName);
            PrepareLogFile(ChatLogFileName);
            PrepareLogFile(ErrorLogFileName);
            
            // Prepare our Log Types
            chatTypes |= LogType.GlobalChat;
            chatTypes |= LogType.IRCChat;
            chatTypes |= LogType.PrivateChat;
            chatTypes |= LogType.WorldChat;
            chatTypes |= LogType.OpChat;

            errorTypes |= LogType.Error;
            errorTypes |= LogType.ErrorMessage;

            // Add our file writer to the log
            OnLog += HandleLog;
        }

        private static void HandleLog(string errorMessage, LogType type, LogSource src)
        {
            // Format our output string
            string message = "[" + DateTime.Now.ToString(shortDateTime) + "]"
                           + "[" + Enum.GetName(typeof(LogSource), src) + "]"
                           + "[" + Enum.GetName(typeof(LogType), type) + "]"
                           + " - " + errorMessage;

            if ((errorTypes & type) == type) 
            {
                WriteLog(message, ErrorLogFileName);
            }
            else if ((chatTypes & type) == type)
            {
                WriteLog(message, ChatLogFileName);
            }
            else if (((type & LogType.Debug) != type) || (Properties.DebugEnabled))
            {
                WriteLog(message, LogFileName);
            }
        }

        private static void WriteLog(string message, string filename)
        {
            bool appendHeader = false;
            string today = DateTime.Now.ToString("-MM-dd-yyyy");
            TextWriter logStream = null;

            try
            {
                // If we're on a new day, append a header to the file
                if (!File.Exists(LogDirectory + filename + today + ".log"))
                {
                    appendHeader = true;
                }

                // Open the log file
                logStream = new StreamWriter(LogDirectory + filename + today + ".log", true);

                // Append the header if its a new day
                if (appendHeader)
                {
                    // Add session header
                    logStream.WriteLine("==================== Session Continued ==================");
                    logStream.WriteLine(SessionStart);
                    logStream.WriteLine("=======================================================");
                }


                // Write contents
                logStream.WriteLine(message);
            }
            catch
            { }
            finally
            {
                if (logStream != null)
                {
                    logStream.Close();
                }
            }
        }

        internal static void PrepareLogFile(string filename)
        {
            string today = DateTime.Now.ToString("-MM-dd-yyyy");
            TextWriter logStream = null;

            try
            {
                // Open the log file
                logStream = new StreamWriter(LogDirectory + filename +  today + ".log", true);

                // Add session header
                logStream.WriteLine("==================== Session Start ====================");
                logStream.WriteLine(SessionStart);
                logStream.WriteLine("=======================================================");
            }
             finally
            {
                // Close the log file no matter what happens
                if (logStream != null)
                {
                    logStream.Close();
                }
            }
        }


        internal static void Log(string message)
        {
            Log(message, LogType.Information, LogSource.Server);
        }

        internal static void Log(string message, LogType type)
        {
            Log(message, type, LogSource.Server);
        }

        public static void Log(string message, LogType type, LogSource src)
        {
            if (OnLog != null)
            {
                OnLog(message, type, src);
            }
        }
    }
}
