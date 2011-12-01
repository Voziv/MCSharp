using System;
using System.IO;

namespace MCSharp
{
    public class CmdLava : Command
    {
        // Constructor
        public CmdLava(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }
        
        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/lava - Turns inactive lava mode on/off.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (p.BlockAction == 2)
            {
                p.BlockAction = 0;
                p.SendMessage("Lava mode: &cOFF&e.");
            }
            else
            {
                p.BlockAction = 2;
                p.SendMessage("Lava Mode: &aON&e.");
                p.painting = false;
            }
        }
    }
}