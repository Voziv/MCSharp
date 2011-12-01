using System;
using System.Collections.Generic;
using System.Text;

namespace MCSharp
{
    class CmdResetBot : Command
    {
        // Constructor
        public CmdResetBot(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = true; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/resetbot - reloads the IRCBot.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            IRCBot.Reset();
            p.SendMessage("IRC Bot reset");
            Logger.Log("IRC Bot Reset by " + p.name);   
        }

        // Code to run when used by the console
        public override void Use(string message)
        {
            IRCBot.Reset();
            Logger.Log("IRC Bot Reset", LogType.ConsoleOutput);
        }
    }
}
