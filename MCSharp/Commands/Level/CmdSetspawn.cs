using System;
using System.IO;

namespace MCSharp 
{
	public class CmdSetspawn : Command 
    {
		// Constructor
        public CmdSetspawn(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/setspawn - Set the default spawn location.");
        }

        // Code to run when used by a player
		public override void Use(Player p,string message)  
        {
            if (message == "")
            {
                p.SendMessage("Spawn location changed.");
                p.level.spawnx = (ushort)(p.pos[0] / 32);
                p.level.spawny = (ushort)(p.pos[1] / 32);
                p.level.spawnz = (ushort)(p.pos[2] / 32);
                p.level.rotx = p.rot[0];
                p.level.roty = 0;
            }
            else
            { 
                Help(p);
            }	
		}
	}
}