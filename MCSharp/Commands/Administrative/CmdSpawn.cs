using System;

namespace MCSharp 
{
	public class CmdSpawn : Command 
    {
		
        // Constructor
        public CmdSpawn(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/spawn - Teleports yourself to the spawn location.");
        }

        // Code to run when used by a player
		public override void Use(Player p,string message)
        {
            
			if (message == "") 
            {
                // Send the player to the spawnpoint of the level
                ushort x = (ushort)((0.5 + p.level.spawnx) * 32);
                ushort y = (ushort)((1 + p.level.spawny) * 32);
                ushort z = (ushort)((0.5 + p.level.spawnz) * 32);
                unchecked
                {
                    p.SendPos((byte)-1, x, y, z, p.level.rotx, p.level.roty);
                }
            }
            else
            { 
                Help(p);
            }
		}
	}
}