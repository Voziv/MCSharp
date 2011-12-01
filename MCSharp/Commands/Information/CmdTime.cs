using System;

namespace MCSharp
{
    public class CmdTime : Command
    {
        // Constructor
        public CmdTime(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/time - Shows the server time.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            string time = DateTime.Now.ToString("hh:mm:ss tt");
            message = "Server time is " + time + "(" + TimeZoneInfo.Local.DisplayName + ")";
            p.SendMessage(message);
        }
    }
}