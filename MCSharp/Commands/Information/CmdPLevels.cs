using System;
using System.Collections.Generic;
using System.Text;

namespace MCSharp
{
    class CmdPLevels : Command
    {
        
        // Constructor
        public CmdPLevels(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/plevels - Shows name and level of all players");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            string strPlayers;
            foreach (Level level in Server.levels)
            {
                strPlayers = "";
                foreach (Player pl in Player.players)
                {
                    if (!p.hidden || p.Rank > pl.Rank)
                    {
                        if (pl.level == level)
                        {
                            strPlayers += pl.color + pl.name + "&e, ";
                        }
                    }
                }
                if (strPlayers != "")
                {
                    strPlayers = strPlayers.Remove(strPlayers.Length - 4);
                }
                else
                {
                    strPlayers = "No players on this level";
                }
                p.SendMessage(player.level.name + ": " + strPlayers);
            }
        }
    }
}
