using System;
using System.IO;

namespace MCSharp
{
    public class CmdMods : Command
    {
        // Constructor
        public CmdMods (CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help (Player p)
        {
            p.SendMessage("/mods - Lists all online moderators.");
        }

        // Code to run when used by a player
        public override void Use (Player p, string message)
        {
            if (message == "")
            {
                string strModerators = "";
                foreach (var pl in Player.players)
                {
                    if (pl.Rank == GroupEnum.Operator && (!pl.hidden || p.Rank >= pl.Rank))
                    {
                        strModerators += pl.color + pl.name + ", ";
                    }
                }
                if (strModerators == "")
                {
                    strModerators = "No moderators currently online";
                }
                else
                {
                    strModerators = strModerators.Remove(strModerators.Length - 2);
                }

                p.SendMessage("Online Moderators: " + strModerators);
            }
            else
            {
                Help(p);
            }
        }
    }
}