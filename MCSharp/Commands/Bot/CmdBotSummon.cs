using System;
using System.IO;

namespace MCSharp
{
    public class CmdBotSummon : Command
    {
        // Constructor
        public CmdBotSummon(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/botsummon <name> - Summons a bot to your position.");
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
                        target.SetPos(p.pos[0], p.pos[1], p.pos[2], p.rot[0], 0);
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