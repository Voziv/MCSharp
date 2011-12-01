using System;

namespace MCSharp
{
    public class CmdKickban : Command
    {
        // Constructor
        public CmdKickban(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/kickban <player> [message] - Kicks and bans a player with an optional message.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                string who = message;
                int index = message.IndexOf(' ');
                string kickmessage = "kicked and banned by " + p.name + "! (";
                if (index != -1)
                {
                    who = message.Substring(0, index);
                    kickmessage += message.Substring(index + 1) + ")";
                }
                else
                {
                    kickmessage += "Kicked + Banned!)";
                }

                Player target = Player.Find(who);

                if (!Server.banned.Contains(who))
                {
                    if (p.Rank > Player.GetRank(who))
                    {

                        if (target != null)
                        {
                            if (target != p)
                            {
                                target.Kick(kickmessage);
                                Player.Ban(who);
                            }
                            else
                            {
                                p.SendMessage("You can't kickban yourself!");
                            }
                        }
                        else
                        {
                            Player.Ban(who);
                            Player.GlobalMessage(target.name + " was " + kickmessage);
                            IRCBot.Say(target.name + " was " + kickmessage);
                        }
                    }
                    else
                    {
                        p.SendMessage("You can't kickban someone of equal or higher rank!");
                    }
                }
                else
                {
                    p.SendMessage(who + " is already banned.");
                    if (target != null)
                    {
                        target.Kick(kickmessage);
                    }
                }
            }
            else
            {
                Help(p);    
            }
            
   
        }
    }
}