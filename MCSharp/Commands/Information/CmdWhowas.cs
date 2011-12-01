using System;
using System.Collections.Generic;
namespace MCSharp
{
    public class CmdWhowas : Command
    {
        // Constructor
        public CmdWhowas(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                Player target = Player.Find(message);
                if (target == null || (target.hidden && p.Rank < target.Rank))
                {
                    if (Player.left.ContainsKey(message.ToLower()))
                    {
                        string playerName = message.ToLower();
                        string ip = Player.left[playerName];
                        message = "&e" + playerName + " is " + Player.GetColor(playerName) + Player.GetGroup(playerName).Name + "&e.";
                        if (p.Rank >= GroupEnum.Operator)
                        {
                            message += " IP: " + ip + ".";
                        }
                        p.SendChat(p, message);
                    }
                    else
                    {
                        p.SendMessage("No entry found for \"" + message + "\".");
                    }
                }
                else
                {
                    p.SendMessage(target.color + target.name + "&e is online, use /whois instead.");
                }
            }
            else
            { 
                Help(p);
            }
        }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/whowas <name> - Displays information about someone who left.");
        }
    }
}