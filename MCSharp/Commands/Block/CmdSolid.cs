using System;
using System.IO;

namespace MCSharp
{
    public class CmdSolid : Command
    {
        // Constructor
        public CmdSolid(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/solid - Turns solid mode on/off.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                if (p.BlockAction == 1)
                {
                    p.BlockAction = 0;
                    p.SendMessage("Solid mode: &cOFF&e.");
                }
                else
                {
                    p.BlockAction = 1;
                    p.SendMessage("Solid Mode: &aON&e.");
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