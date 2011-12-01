using System;
using System.IO;

namespace MCSharp
{
    public class CmdBotRemove : Command
    {
        
        // Constructor
        public CmdBotRemove(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/botremove <name> - Remove a bot on the same level as you");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                PlayerBot target = PlayerBot.Find(message);
                if (target != null)
                {
                    if (p.level == target.level)
                    {
                        target.GlobalDie();
                    }
                    else
                    {
                        p.SendMessage(target.name + " is in a different level.");
                    }
                }
                else
                {
                    p.SendMessage("There is no bot called " + message + "!");
                }
            }
            else
            { 
                Help(p);
            }
        }
    }
}