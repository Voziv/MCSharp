using System;
using System.Collections.Generic;
using System.Text;

namespace MCSharp
{
    class CmdAfk : Command
    {
        // Constructor
        public CmdAfk(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/afk <reason> - mark yourself as AFK. Use again to mark yourself as back");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "list")
            {
                if (Server.afkset.Contains(p.name))
                {
                    Server.afkset.Remove(p.name);
                    Player.GlobalMessage("-" + p.group.Color + p.name + "&e- is no longer AFK");
                    IRCBot.Say(p.name + " is no longer AFK");
                }
                else
                {
                    Server.afkset.Add(p.name);
                    Player.GlobalMessage("-" + p.group.Color + p.name + "&e- is AFK " + message);
                    IRCBot.Say(p.name + " is AFK " + message);
                }
            }
            else
            {
                // Send list of afk players
                foreach (string s in Server.afkset)
                {
                    p.SendMessage(s);
                }
            }
        }
    }
}
