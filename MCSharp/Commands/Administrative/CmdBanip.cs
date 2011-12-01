using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MCSharp
{
    public class CmdBanip : Command
    {
        // Constructor
        public CmdBanip(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }
        
        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/banip <ip/name> - Bans an ip, can also use the name of an online player.");
            p.SendMessage(" -Kicks players with matching ip as well.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            Regex regex = new Regex(@"^([0-9]{1,3}\.){3}[0-9]{1,3}$");
            if (message == "") { if (p != null)Help(p); return; }
            Player who = null;
            who = Player.Find(message);
            if (who != null) { message = who.ip; }
            if (message.Equals("127.0.0.1")) { if (p != null) { p.SendMessage("You can't ip-ban the server!"); } return; }
            if (!regex.IsMatch(message)) { if (p != null)p.SendMessage("Not a valid ip!"); return; }
            if (p != null) { if (p.ip == message) { p.SendMessage("You can't ip-ban yourself.!"); return; } }
            if (Server.bannedIP.Contains(message)) { if (p != null)p.SendMessage(message + " is already ip-banned."); return; }
            Player.GlobalMessage(message + " got &8ip-banned&e!");
            if (p != null)
            { IRCBot.Say("IP-BANNED: " + message.ToLower() + " by " + p.name); }
            else
            { IRCBot.Say("IP-BANNED: " + message.ToLower() + " by console"); }
            Server.bannedIP.Add(message);
            Logger.Log("IP-BANNED: " + message.ToLower());

            List<Player> kickList = new List<Player>();
            foreach (Player pl in Player.players)
            {
                if (message.Equals(pl.ip)) { kickList.Add(pl); }       //Kicks anyone off with matching ip for convinience
            }
            foreach (Player pl in kickList)
            {
                pl.Kick("Kicked by ipban");
            }
        }
    }
}