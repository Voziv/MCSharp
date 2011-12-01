using System;
using System.Collections.Generic;
using System.Text;

namespace MCSharp
{
    class CmdPlayers : Command
    {
        // Constructor
        public CmdPlayers(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/players - Shows name and general rank of all players");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            UInt16 playerCount = 0;
            string strPlayers = "";
            foreach (Player pl in Player.players)
            {
                if (!pl.hidden || p.Rank > pl.Rank)
                {
                    strPlayers += pl.color + pl.name + "&e, ";
                    playerCount++;
                }
            }
            
            if (strPlayers != "")
            {
                strPlayers = strPlayers.Remove(strPlayers.Length - 4);
            }
            else
            {
                strPlayers = "No players online";
            }
            p.SendMessage("There are " + playerCount + " players online");
            p.SendMessage("Playerlist: " + strPlayers);
        }
    }
}
