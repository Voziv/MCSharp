using System;
using System.IO;

namespace MCSharp
{
    public class CmdTree : Command
    {
        // Constructor
        public CmdTree(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/tree - Placed plants turn into trees");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                if (p.BlockAction == 7)
                {
                    p.BlockAction = 0;
                    p.SendMessage("Tree mode: &cOFF&e.");
                }
                else
                {
                    p.BlockAction = 7;
                    p.SendMessage("Tree Mode: &aON&e. Place a plant to grow trees");
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