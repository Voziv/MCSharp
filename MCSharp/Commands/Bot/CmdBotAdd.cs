using System;
using System.IO;

namespace MCSharp
{
    public class CmdBotAdd : Command
    {
        // Constructor
        public CmdBotAdd(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }
        
        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/botadd <name> - Add a  new bot at your position.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            // Make sure the message isn't blank
            if (message != "")
            {
                // Make sure the name is valid
                if (PlayerBot.ValidName(message))
                {
                    // Check to see if there is a bot by this name already
                    PlayerBot target = PlayerBot.Find(message);
                    if (target == null)
                    {
                        // Add a bot
                        PlayerBot.playerbots.Add(new PlayerBot(message, p.level, p.pos[0], p.pos[1], p.pos[2], p.rot[0], 0));
                    }
                    else
                    {
                        p.SendMessage("bot " + target.name + " already exists!");
                    }
                }
                else
                {
                    p.SendMessage("bot name " + message + " not valid!");
                }
            }
            else
            { 
                Help(p); 
            }
        }
    }
}