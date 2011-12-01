using System;

namespace MCSharp
{
    public class CmdKick : Command
    {
        // Constructor
        public CmdKick(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = true; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/kick <player> [message] - Kicks a player.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {

            if (message != "")
            {
                string who = message;
                int index = message.IndexOf(' ');

                if (index != -1)
                {
                    who = message.Substring(0, index);
                    message = message.Substring(index + 1);
                }
                
                // Find the player and ensure they are online
                Player target = Player.Find(who);
                if (target != null)
                {
                    if (p.Rank > target.Rank)
                    {
                        if (target != p)
                        {
                            if (index == -1)
                            {
                                target.Kick("You were kicked by " + p.name + "!");
                                IRCBot.Say(who + " was kicked by " + p.name);
                            }
                            else
                            {
                                target.Kick(message);
                                IRCBot.Say(who + " was kicked by " + p.name + "(" + message + ")");
                            }
                        }
                        else
                        {
                            p.SendMessage("You can't kick yourself!");
                        }
                    }
                    else
                    {
                        p.SendMessage("You can't kick someone of equal or higher rank!");
                    }
                }
                else { p.SendMessage("There is no player called \"" + who + "\" currently online!"); } 
            }
            else
            {
                Help(p);
            }
        }

        // Code to run when used by the console
        public override void Use(string message)
        {
            if (message != "") 
            {
                string who = message;
                int index = message.IndexOf(' ');
                if (index != -1)
                {
                    who = message.Substring(0, index);
                    message = message.Substring(index + 1);
                }
                if (Player.Exists(who))
                {
                    Player target = Player.Find(who);
                    if (index == -1)
                    {
                        target.Kick("You were kicked by [console]!"); 
                        IRCBot.Say(who + " was kicked by [console]");
                    }
                    else
                    {
                        target.Kick(message);
                        IRCBot.Say(who + " was kicked (" + message + ")");
                    }
                }
            }
        }
       
    }
}