using System;
using System.Collections.Generic;

namespace MCSharp
{
    public class CmdClones : Command
    {
        private Dictionary<string, List<string>> clones = new Dictionary<string, List<string>>();
        private char[] trimmings = { ' ', ',' };

        // Constructor
        public CmdClones(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/clones - Displays the players on the same IP.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "")
            {

                // Check for clones
                foreach (Player pl in Player.players)
                {
                    foreach (Player pl1 in Player.players)
                    {
                        if (pl.ip == pl1.ip && pl.name != pl1.name)     //If same IP found on another player
                        {
                            if (!clones.ContainsKey(pl.ip))     //If the key does not exist yet
                            {
                                clones.Add(pl.ip, new List<string>());
                                clones[pl.ip].Add(pl.name);
                                clones[pl.ip].Add(pl1.name);    //add playername to the list
                            }
                            else
                            {
                                if (!clones[pl.ip].Contains(pl1.name))
                                {
                                    clones[pl.ip].Add(pl1.name);
                                }
                            }
                        }
                    }
                }

                // List clones
                if (clones.Count > 0)
                {
                    p.SendMessage("Clones have been found!");
                    foreach (KeyValuePair<string, List<string>> kvp in clones)
                    {
                        string players = "";
                        foreach (string s in kvp.Value)
                        {
                            Player fplayer = Player.Find(s);
                            players += fplayer.color + s + "&e, ";
                        }
                        p.SendMessage(kvp.Key + ": " + players.Trim(trimmings));
                    }
                    clones.Clear();
                }
                else
                {
                    p.SendMessage("No Clones Found");
                }
            }
            else
            {
                Help(p); 
            }
        }
    }
}
