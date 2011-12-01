using System;
using System.IO;

namespace MCSharp
{
    public class CmdActiveWater : Command
    {
        // Constructor
        public CmdActiveWater(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/activewater - Place a single block of active water.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (p.BlockAction == 5)
            {
                p.BlockAction = 0;
                p.SendMessage("Active water aborted.");
            }
            else
            {
                p.BlockAction = 5;
                p.SendMessage("Now placing &cactive_water&e!");
            }
            p.painting = false;
        }
    }
}