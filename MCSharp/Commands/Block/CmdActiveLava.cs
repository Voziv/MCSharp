using System;
using System.IO;

namespace MCSharp
{
    public class CmdActiveLava : Command
    {
        // Constructor
        public CmdActiveLava(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/activelava - Place a single block of active lava.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (p.BlockAction == 4)
            {
                p.BlockAction = 0;
                p.SendMessage("Active lava aborted.");
            }
            else
            {
                p.BlockAction = 4;
                p.SendMessage("Now placing &cactive_lava&e!");
            }
            p.painting = false;
        }
    }
}