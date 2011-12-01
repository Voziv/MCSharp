using System;
using System.IO;

namespace MCSharp {
	public class CmdTp : Command {
        
        // Constructor
        public CmdTp(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }
        
        // Command Usage help
        public override void Help(Player p)
        {
            p.SendMessage("/tp <player> - Teleports you to a player, even if they are on a different map.");
        }

        // Code to run when used by a player
		public override void Use(Player p, string message)  {

            if (message != "")
            {
                if (Player.ValidName(message))
                {
                    Player target = Player.Find(message);
                    if (target != null)
                    {
                        // if the target is visible or your rank is higher than theirs, then teleport
                        if (!target.hidden || p.Rank > target.Rank)
                        {
                            if (p.level != target.level)
                            {
                                // Change levels if needed
                                p.ChangeLevel(target.level.name);
                            }
                            // Teleport to the player
                            unchecked
                            {
                                p.SendPos((byte)-1, target.pos[0], target.pos[1], target.pos[2], target.rot[0], 0);
                            }
                        }
                        else
                        {
                            // Player is offline
                            p.SendMessage("There is no player \"" + message + "\"!");
                        }
                    }
                    else
                    {
                        // Player is offline
                        p.SendMessage("There is no player \"" + message + "\"!");
                    }

                }
                else
                {
                    p.SendMessage("Invalid Name!");
                }
            }
            else
            {
                Help(p);
            }
		}
	}
}