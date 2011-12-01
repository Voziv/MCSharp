using System;
using System.Collections.Generic;
using System.Text;

namespace MCSharp
{
    class CmdSay : Command
    {
        // Constructor
        public CmdSay(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/say - Brodcasts a global message to everyone in the server.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                message = "&e" + message; // defaults to yellow
                message = message.Replace("%", "&"); // Allow colors in global messages
                Player.GlobalChat(p, message, false);
                message = message.Replace("&", ""); // converts the MC color codes to IRC. Doesn't seem to work with multiple colors
                IRCBot.Say("Global: " + message);
            }
            else
            {
                Help(p);
            }
            
        }
    }
}
