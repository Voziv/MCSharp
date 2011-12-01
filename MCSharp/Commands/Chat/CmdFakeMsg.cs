using System;

namespace MCSharp
{
    public class CmdFakeMsg : Command
    {
        // Constructor
        public CmdFakeMsg(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/fakemsg <player> <message> - Fake a player message");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "") 
            {
                string[] split = message.Split(' ');
                string newmsg = "";

                Player target = Player.Find(split[0]);

                if (target != null)
                {
                    // Make sure we have a message to send
                    foreach (string s in split)
                    {
                        if (s != split[0] && s.Trim() != "")
                        {
                            newmsg = newmsg + s + " ";
                        }
                    }
                    newmsg.Trim();
                    if (newmsg.Length > 0)
                    {
                        Player.GlobalChat(target, newmsg);
                        Logger.Log("<" + target.name + "> " + newmsg, LogType.UserCommand);
                        IRCBot.Say(target.name + ": " + newmsg);
                    }
                    else
                    {
                        p.SendMessage("Error: You need a message to send!");
                    }

                }
                else
                {
                    p.SendMessage(LanguageString.NoSuchPlayer);
                }
            }
            else
            {
                Help(p); 
            }
        }
    }
}
