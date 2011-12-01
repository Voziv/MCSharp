using System;
using System.Text.RegularExpressions;

namespace MCSharp
{
    public class CmdUnbanip : Command
    {
        Regex regex = new Regex(@"^([0-9]{1,3}\.){3}[0-9]{1,3}$");
        
        // Constructor
        public CmdUnbanip(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }
        
        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/unbanip <ip> - Un-bans an ip.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            // Make sure the message isn't empty
            if (message != "")
            {
                // Validate IP Address
                if (regex.IsMatch(message))
                {
                    // We shouldn't be able to use this if our own ip is banned
                    if (p.ip != message)
                    {
                        // Make sure the IP is actually banned before unbanning
                        if (Server.bannedIP.Contains(message))
                        {
                            Player.GlobalMessage(message + " got &8unip-banned&e!");
                            Server.bannedIP.Remove(message);
                            Logger.Log("IP-UNBANNED: " + message.ToLower(), LogType.UserCommand);
                        }
                        else
                        {
                            p.SendMessage(message + " doesn't seem to be banned...");
                        }
                    }
                    else
                    {
                        p.SendMessage("This is your IP. You don't seemed to be banned....");
                    }
                }
                else
                {
                    p.SendMessage("Not a valid ip!");
                }
            }
            else
            {
                Help(p);
            }
        }
    }
}