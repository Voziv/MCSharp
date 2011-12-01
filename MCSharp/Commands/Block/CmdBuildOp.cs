using System;
using System.IO;

namespace MCSharp
{
    public class CmdBuildOp : Command
    {
        // Constructor
        public CmdBuildOp(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/buildop - All blocks turn to op_material when placed");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                // Toggle OFF
                if (p.BlockAction == 8)
                {
                    p.BlockAction = 0;
                    p.SendMessage("BuildOp mode: &cOFF&e.");
                }
                else // Toggle ON
                {
                    // Disable painting
                    p.painting = false;

                    // Clear all bindings, creates faulty blocks otherwise
                    int bindCount = p.ClearBindings();

                    // Set the block action to buildop
                    p.BlockAction = 8;

                    if (bindCount > 0)
                    {
                        p.SendMessage("Cleared " + bindCount + " block bind(s).");
                    }

                    p.SendMessage("BuildOp Mode: &aON&e. Now Placing op_materials");
                }
            }
            else
            {
                Help(p);
            }
        }
    }
}