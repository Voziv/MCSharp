using System;
using System.IO;

namespace MCSharp
{
    public class CmdOps : Command
    {
        // Constructor
        public CmdOps (CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help (Player p)
        {
            p.SendMessage("/ops - Lists all online operators.");
        }

        // Code to run when used by a player
        public override void Use (Player p, string message)
        {
            if (message == "")
            {
                string strOperators = "";
                foreach (var pl in Player.players)
                {
                    if (pl.Rank == GroupEnum.Operator && (!pl.hidden || p.Rank >= pl.Rank))
                    {
                        strOperators += pl.color + pl.name + ", ";
                    }
                }
                if (strOperators == "")
                {
                    strOperators = "No operators currently online";
                }
                else
                {
                    strOperators = strOperators.Remove(strOperators.Length - 2);
                }

                p.SendMessage("Online Operators: " + strOperators);
            }
            else
            {
                Help(p);
            }
        }
    }
}