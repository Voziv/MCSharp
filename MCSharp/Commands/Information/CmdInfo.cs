using System;

namespace MCSharp 
{
	public class CmdInfo : Command 
    {
		// Constructor
        public CmdInfo(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/info - Displays the server information.");
        }

        // Code to run when used by a player
		public override void Use(Player p,string message)  {
			if (message == "") 
            {
                p.SendMessage("MCsharp Revision " + Server.Version + ".");
                p.SendMessage("Developed and maintained by the MCSharp Development Team.");
                p.SendMessage("Official channel: &2#mcsharp @ irc.esper.net&e.");
                p.SendMessage("Visit our website at http://crafted.voziv.com");
            }
            else 
            {
                Help(p); 
			}
		} 
	}
}
