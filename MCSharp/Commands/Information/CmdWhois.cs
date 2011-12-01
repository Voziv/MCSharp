using System;

namespace MCSharp
{
    public class CmdWhois : Command
    {
        // Constructor
        public CmdWhois(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/whois [player] - Displays information about someone.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            Player target;
            
            if (message == "") 
            { 
                target = p; 
            } 
            else 
            { 
                target = Player.Find(message);
            }

            if (target != null && (!target.hidden || p.Rank >= target.Rank))
            {
                if (target == p)
                {
                    message = "&eYou are a(n) " + target.group.Color + target.group.Name + "&e.";
                }
                else
                {
                    message = target.color + target.name + "&e is a(n) " +
                        target.group.Color + target.group.Name + "&e on &b" + target.level.name + "&e.";
                }

                if (Server.afkset.Contains(target.name)) { message += "-AFK-"; }

                if (p.Rank > GroupEnum.Moderator)
                {
                    message += " IP: " + target.ip + ".";
                }

                p.SendMessage(message);

                if (Player.checkDev(target))
                {
                    p.SendMessage(target.name + " is a developer of MCSharp.");
                }
                else if (Player.checkSupporter(target))
                {
                    p.SendMessage(target.name + " is a supporter of MCSharp.");
                }
            }
            else
            {
                p.SendMessage(LanguageString.NoSuchPlayer);
            }
        }
    }
}