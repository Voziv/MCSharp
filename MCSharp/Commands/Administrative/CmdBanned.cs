using System;
using System.IO;

namespace MCSharp {
	public class CmdBanned : Command {
		
        // Constructor
        public CmdBanned(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/banned - Lists all banned names.");
        }

        // Code to run when used by a player
		public override void Use(Player p,string message)  {
			if (message != "")
            {
                Help(p); return;
            }
			if (Server.banned.All().Count > 0) 
            {
				Server.banned.All().ForEach(delegate(string name) { message += ", "+name; } );
				p.SendMessage(Server.banned.All().Count+" player"+((Server.banned.All().Count!=1) ? "s" : "")+" &8banned&e: "+message.Remove(0,2)+".");
			} 
            else 
            {
                p.SendMessage("Nobody is banned."); 
            }
		}
	}
}