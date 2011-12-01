using System;

namespace MCSharp
{
    public class CmdLastSeen : Command
    {
        // Constructor
        public CmdLastSeen(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/lastseen <player> - Display last time player was connected");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                Player target = Player.Find(message);
                if (target == null)
                {
                    if (Player.lastSeen.ContainsKey(message.ToLower()))
                    {
                        string lastOnline = message + " was last seen on: " + Player.lastSeen[message.ToLower()].ToLongDateString() + " at: " + Player.lastSeen[message.ToLower()].ToShortTimeString() + TimeZone.CurrentTimeZone.ToString();
                        p.SendMessage(lastOnline);
                    }
                    else
                    {
                        p.SendMessage("Player not found!");
                    }
                }
                else
                {
                    if (target.hidden && p.Rank < target.Rank)
                    {
                        p.SendMessage("Player not found!");
                    }
                    else
                    {
                        p.SendMessage(target.name + " is currently playing!");
                    }
                }
            }
            else
            {
                Help(p);
            }
        }
    }
}
