using System;
using System.IO;

namespace MCSharp
{
    public class CmdSummon : Command
    {

        // Constructor
        public CmdSummon(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/summon <player> - Summons a player to your position.");
        }

        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                Player target = Player.Find(message);
                if (target != null)
                {
                    if (!target.hidden)
                    {
                        if (p.level != target.level)
                        {
                            target.ChangeLevel(p.level.name);
                        }
                        unchecked 
                        { 
                            target.SendPos((byte)-1, p.pos[0], p.pos[1], p.pos[2], p.rot[0], 0); 
                        }
                        target.SendMessage("You were summoned by " + p.color + p.name + "&e.");
                    }
                    else
                    {
                        p.SendMessage("There is no player \"" + message + "\"!");
                    }
                }
                else
                {
                    p.SendMessage("There is no player \"" + message + "\"!");
                }
            }
            else
            {
                Help(p);
            }
        }
    }
}