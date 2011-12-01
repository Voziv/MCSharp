using System;
using System.IO;

namespace MCSharp
{
    public class CmdWater : Command
    {
        // Constructor
        public CmdWater(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/water - Turns inactive water mode on/off.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                if (p.BlockAction == 3)
                {
                    p.BlockAction = 0;
                    p.SendMessage("Water mode: &cOFF&e.");
                }
                else
                {
                    p.BlockAction = 3;
                    p.SendMessage("Water Mode: &aON&e.");
                    p.painting = false;
                }
            }
            else
            {
                Help(p);
            }
        }
    }
}