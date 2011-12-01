using System;

namespace MCSharp 
{
	public class CmdAbort : Command 
    {
		// Constructor
        public CmdAbort(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/abort - Cancels an action.");
        }

        // Code to run when used by a player
		public override void Use(Player p,string message) 
        {
			if (p.HasBlockchange()) 
            { 
                p.SendMessage("There is no action to abort."); 
            }
			else 
            {
                p.ClearBlockchange();
                p.SendMessage("Action aborted."); 
            }
		}
	}
}