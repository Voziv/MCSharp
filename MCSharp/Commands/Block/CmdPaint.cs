using System;
using System.IO;

namespace MCSharp
{
    public class CmdPaint : Command
    {
        // Constructor
        public CmdPaint(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }
        
        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/paint - Turns painting mode on/off.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                p.painting = !p.painting; 
                if (p.painting) 
                { 
                    p.SendMessage("Painting mode: &aON&e.");
                    p.BlockAction = 0;
                }
                else 
                {
                    p.SendMessage("Painting mode: &cOFF&e.");
                }
            }
            else
            {
                Help(p);
            }
        }
    }
}