using System;
using System.Collections.Generic;
using System.Text;

namespace MCSharp
{
    class CmdNewLvl : Command
    {
        // Constructor
        public CmdNewLvl(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/newlvl - creates a new level.");
            p.SendMessage("/newlvl mapname 128 64 128 type");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                string[] parameters = message.Split(' '); // Grab the parameters from the player's message
                if (parameters.Length == 5) // make sure there are 5 params
                {
                    switch (parameters[4])
                    {
                        case "flat":
                        case "pixel":
                        case "island":
                        case "mountains":
                        case "ocean":
                        case "forest":

                            break;

                        default:
                            p.SendMessage("Valid types: island, mountains, forest, ocean, flat, pixel"); return;
                    }

                    string name = parameters[0];
                    // create a new level...
                    try
                    {
                        Level lvl = new Level(name,
                                              Convert.ToUInt16(parameters[1]),
                                              Convert.ToUInt16(parameters[2]),
                                              Convert.ToUInt16(parameters[3]),
                                              parameters[4]);
                        lvl.changed = true;
                    }
                    finally
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                    }
                    Player.GlobalMessage("Level " + name + " created");
                }
                else
                {
                    p.SendMessage("Not enough parameters! <name> <x> <y> <z> <type>");
                }
            }
            else
            {
                Help(p);
            }
            if (Properties.ValidString(message, " "))
            {

            }
            else
            {
                p.SendMessage("Please use an alphanumerical characters only!");
            }
        }
    }
}
