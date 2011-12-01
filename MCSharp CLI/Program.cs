using System;


namespace MCSharp.CLI
{
    class Program
    {
        static Server server;
        static LogType consoleLogTypes;

        static void Main(string[] args)
        {
            consoleLogTypes |= LogType.Information;
            consoleLogTypes |= LogType.Warning;
            consoleLogTypes |= LogType.Error;
            consoleLogTypes |= LogType.FatalError;
            consoleLogTypes |= LogType.UserCommand;
            consoleLogTypes |= LogType.UserActivity;
            consoleLogTypes |= LogType.SuspiciousActivity;
            consoleLogTypes |= LogType.ConsoleOutput;
            consoleLogTypes |= LogType.IRCChat;
            consoleLogTypes |= LogType.PrivateChat;
            consoleLogTypes |= LogType.WorldChat;
            consoleLogTypes |= LogType.OpChat;
            consoleLogTypes |= LogType.GlobalChat;
            
            try
            {
                server = new Server();
                Logger.OnLog += Log;
                server.OnSettingsUpdate += SettingsUpdate;
                server.Start();
                server.ParseInput();
            }
            catch (Exception e)
            {
                Console.WriteLine("Console: " + e.Message);
            }

        }

        static void SettingsUpdate()
        {
            Console.Title = Properties.ServerName;
        }

        static void Log(string message, LogType type, LogSource src)
        {
            if ((LogType.Warning & type) == type)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if ((LogType.OpChat & type) == type || (LogType.GlobalChat & type) == type || (LogType.IRCChat & type) == type || (LogType.GlobalChat & type) == type)
                Console.ForegroundColor = ConsoleColor.White;
            else if ((LogType.Error & type) == type || (LogType.FatalError & type) == type || (LogType.ErrorMessage & type) == type)
                Console.ForegroundColor = ConsoleColor.Red;

            message = "[" + DateTime.Now.ToString("hh:mm:ss tt") + "]"
                           + "[" + Enum.GetName(typeof(LogType), type).Substring(0, 1) + "]"
                           + " - " + message;

            if ((consoleLogTypes & type) == type)
                Console.WriteLine(message);
            else if ((LogType.Debug & type) == type && Properties.DebugEnabled)
                Console.WriteLine(message);

            // Reset the console color afterwards to make sure the informational messages are always there
            Console.ResetColor();
        }
    }
}
