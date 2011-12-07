using System;
using MCSharp.World;
namespace MCSharp 
{
	public class CmdAbout : Command 
    {
		// Constructor
        public CmdAbout(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/about - Displays information about a block.");
        }

        // Code to run when used by a player
		public override void Use(Player p,string message)  
        {
            if (message == "")
            {
                p.SendMessage("Break/build a block to display information.");
                p.ClearBlockchange();
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange);
            }
            else
            {
                Help(p);
            }
		} 
        
        public void Blockchange(Player p,ushort x,ushort y,ushort z,byte type)
        {
			p.ClearBlockchange();
			byte b = p.level.GetTile(x,y,z);
            if (b != Block.Zero)
            {
                p.SendBlockchange(x, y, z, b);
                string message = "Block (" + x + "," + y + "," + z + "): ";
                message += "&f" + b + " = " + Block.Name(b);
                p.SendMessage(message + "&e.");
            }
            else
            {
                p.SendMessage("Invalid Block(" + x + "," + y + "," + z + ")!"); 
            }
		}
	}
}