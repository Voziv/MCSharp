using System;

namespace MCSharp 
{
	public class CmdMe : Command 
    {
		// Constructor
        public CmdMe(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/me <action> - Roleplay-like action message.");
        }

        // Code to run when used by a player
		public override void Use(Player p,string message)  
        {
			if (message != "")
            {
                if (Properties.AllowWorldChat)
                {
                    Player.GlobalChat(p, p.color + "*" + p.name + " " + message, false);
                }
                else
                {
                    Player.GlobalChatLevel(p, p.color + "*" + p.name + " " + message, false);
                }
            }
            else
            {
                Help(p);
            } 
		}
	}
}