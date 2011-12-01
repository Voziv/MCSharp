using System;
using System.IO;

namespace MCSharp {
	public class CmdBannedip : Command {
		
        // Constructor
        public CmdBannedip(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/bannedip - Lists all banned IPs.");
        }

        // Code to run when used by a player
		public override void Use(Player p,string message)  {
            if (message == "")
            {
                if (Server.bannedIP.All().Count > 0)
                {
                    Server.bannedIP.All().ForEach(delegate(string name) { message += ", " + name; });
                    p.SendMessage(Server.bannedIP.All().Count.ToString() + " IP" + ((Server.bannedIP.All().Count != 1) ? "s" : "") + "&8banned&e: " + message.Remove(0, 2) + ".");
                }
                else 
                { 
                    p.SendMessage("No IP is banned.");
                }    
            }
            else
            {
                Help(p);
            }
			
		} 
	}
}