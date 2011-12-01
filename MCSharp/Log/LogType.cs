using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSharp
{
    [Flags]
    public enum LogType
    {
        Information = 1,
        Debug = 2,
        Warning = 4,
        Error = 8,
        FatalError = 16,
        ErrorMessage = 32,

        ConsoleOutput = 64,
        ConsoleInput = 64,

        PrivateChat = 256,
        GlobalChat = 512,
        IRCChat = 1024,
        WorldChat = 2048,
        OpChat = 4096,

        UserCommand = 8192,

        UserActivity = 16384,
        SuspiciousActivity = 32768,

        ScriptLog = 65536
    }
}
