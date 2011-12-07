using System;

namespace MCSharp
{
    public class CmdHidden : Command
    {
        private string playerList;
        private char[] trimmings = { ' ', ',' };
        
        // Constructor
        public CmdHidden(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/hidden - Displays the hidden players.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                playerList = "";
                foreach (var pl in Player.players)
                {
                    if (pl.hidden && p.Rank >= pl.Rank)
                    {
                        playerList += pl.color + pl.name + "&e, ";
                    }
                }

                if (playerList == "")
                {
                    p.SendMessage("No one is currently hidden.");
                }
                else
                {
                    p.SendMessage("Hidden Players: " + playerList.Trim(trimmings));
                }
            }
            else
            {
                Help(p);
            }
        }
    }
}
