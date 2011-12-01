using System;
using System.IO;

namespace MCSharp
{
    public class CmdBuildDoor : Command
    {
        // Constructor
        public CmdBuildDoor(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/buildDoor - Toggles build door mode. When activated, all blocks will turn into doors when placed.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                // Toggle OFF
                if (p.BlockAction == 9)
                {
                    p.BlockAction = 0;
                    p.SendMessage("BuildDoor mode: &cOFF&e.");
                }
                else // Toggle ON
                {
                    // Disable painting
                    p.painting = false;

                    // Clear all bindings, creates faulty blocks otherwise
                    int bindCount = p.ClearBindings();

                    // Set the block action to buildop
                    p.BlockAction = 9;

                    if (bindCount > 0)
                    {
                        p.SendMessage("Cleared " + bindCount + " block bind(s).");
                    }

                    p.SendMessage("BuildDoor Mode: &aON&e. Now Placing doors.");
                }
            }
            else
            {
                Help(p);    
            }
        }
    }
}