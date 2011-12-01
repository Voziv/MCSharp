
namespace MCSharp {
	public class CmdHide : Command {
		
        // Constructor
        public CmdHide(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }
        
        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/hide - Makes yourself (in)visible to other players.");
        }

        // Code to run when used by a player
		public override void Use(Player p,string message)
        {
            if (message == "")
            {
                p.hidden = !p.hidden;
                if (p.hidden)
                {
                    Player.GlobalDie(p, true);
                    Player.GlobalMessageOps("To Ops &f-" + p.color + p.name + "&f-&e is now &finvisible&e.");
                    Player.GlobalChat(p, "&c- " + p.color + p.name + "&e disconnected.", false);
                    //p.SendMessage("You're now &finvisible&e.");
                }
                else
                {
                    Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                    Player.GlobalMessageOps("To Ops &f-" + p.color + p.name + "&f-&e is now &8visible&e.");
                    Player.GlobalChat(p, "&a+ " + p.color + p.name + "&e joined the game.", false);
                    //p.SendMessage("You're now &8visible&e.");
                }
            }
            else
            {
                Help(p);
            }
			
		} 
	}
}