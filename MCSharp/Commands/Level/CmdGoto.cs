using System;

namespace MCSharp 
{
	public class CmdGoto : Command 
    {
		// Constructor
        public CmdGoto(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/goto <mapname> - Teleports yourself to a different level.");
        }

        // Code to run when used by a player
        public override void Use(Player p,string message)  
        {
            bool blnLevelFound = false;
            Level targetLevel = null;
            if (message != "")
            {
                message = message.ToLower();
                if (p.level.name.ToLower() != message)
                {
                    foreach (Level level in Server.levels)
                    {
                        if (level.name.ToLower() == message)
                        {
                            targetLevel = level;
                            blnLevelFound = true;
                            break;
                        }
                    }
                    if (blnLevelFound)
                    {
                        if (targetLevel.PlayerCanVisit(p))
                        {
                            p.ChangeLevel(targetLevel);
                            if (!p.hidden)
                            {
                                Player.GlobalChat(p, p.color + "*" + p.name + "&e went to \"" + p.level.name + "\".", false);
                            }
                        }
                        else
                        {
                            p.SendMessage("You do not have perimssion to visit this level");
                        }
                    }
                    else
                    {
                        // Level either not found or non-existant
                        
                        p.SendMessage("There is no level \"" + message + "\" loaded.");
                    }
                    
                }
                else
                {
                    p.SendMessage("You are already on that level!");
                }
            }
            else
            {
                Help(p); 
            }
		} 
	}
}