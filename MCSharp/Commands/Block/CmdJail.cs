using System;
using System.Timers;
using System.Collections.Generic;

namespace MCSharp
{
    public class CmdJail : Command
    {
        Timer timer = new Timer(1000 * 1);  //Every second

        // Constructor
        public CmdJail(CommandGroup g, GroupEnum group, string name) : base(g, group, name)
        {
            blnConsoleSupported = false;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;
        }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/jail <cmd> <player> - Jails a banned user. (Commands: set, add, free)");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                Help(p);
                return;
            }

            string[] commands = message.Split(' ');

            switch (commands[0])
            {
                case "set":
                    {
                        p.level.jailedX = p.pos[0];
                        p.level.jailedY = p.pos[1];
                        p.level.jailedZ = p.pos[2];
                        p.level.jailedRotX = p.rot[0];
                        p.level.jailedRotY = p.rot[1];
                        p.SendMessage("Jail location successfully set at your position");
                        break;
                    }
                case "add":
                    {
                        if (commands.Length != 2)
                        {
                            Help(p);
                            return;
                        }
                        Player who = Player.Find(commands[1]);
                        if (who == null)                        //check for valid player
                        {
                            p.SendMessage("Player not found!");
                            return;
                        }
                        if (who.level.jailedX == 0 && who.level.jailedY == 0)       //Check for valid jail position
                        {
                            p.SendMessage("No jail position set for that level! Go set one!");
                            return;
                        }
                        if (who.isJailed)
                        {
                            p.SendMessage(who.name + " is already in jail!");
                        }
                        else
                        {
                            who.level.jailedPlayers.Add(who);
                            who.isJailed = true;        //This prevents the jailed user from using goto
                            Player.GlobalMessage("-" + who.color + who.name + "&e is now JAILED on &2" + who.level.name + "! &eEveryone point and laugh!");
                            IRCBot.Say(who.name + " was just JAILED on " + who.level.name);
                        }
                        break;
                    }
                case "free":
                    {
                        if (commands.Length != 2)
                        {
                            Help(p);
                            return;
                        }
                        Player who = Player.Find(commands[1]);
                        if (!who.level.jailedPlayers.Remove(who))   //check for success
                        {
                            p.SendMessage("Player not found!");
                            return;
                        }
                        who.isJailed = false;   //User can use goto again
                        who.SendMessage("You have been freed!");
                        Player.GlobalMessage("-" + who.color + who.name + "&e has been let out of jail!");
                        IRCBot.Say(who.name + " has been let out of jail!");
                        break;
                    }
                default:
                    Help(p);
                    break;
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (Level l in Server.levels)  //Check each level
            {
                if (l.jailedPlayers.Count > 0)  //Make sure level has jailed players
                {
                    foreach (Player p in l.jailedPlayers)   //Check each jailed player
                    {
                        if (!p.Loading && !p.disconnected && movedFromJail(p, l.jailedX, l.jailedY, l.jailedZ))  //If player moved outside of jailed zone
                        {
                            unchecked
                            {
                                p.SendPos((byte)-1, l.jailedX, l.jailedY, l.jailedZ,
                                            l.jailedRotX,
                                            l.jailedRotY);              //Send player back to jailed POS.
                                p.SendMessage("--You are in JAIL! Stay there!--");
                            }
                        }
                    }
                }
            }
        }

        private bool movedFromJail(Player jailedPlayer, ushort jailedX, ushort jailedY, ushort jailedZ)
        {
            int threshhold = 75;        //How far can player wander from jailed POS
            int diffX, diffY, diffZ;

            diffX = Math.Abs(jailedPlayer.pos[0]) - Math.Abs(jailedX);
            diffY = Math.Abs(jailedPlayer.pos[1]) - Math.Abs(jailedY);
            diffZ = Math.Abs(jailedPlayer.pos[2]) - Math.Abs(jailedZ);

            if (Math.Abs(diffX) >= threshhold || Math.Abs(diffY) >= threshhold || Math.Abs(diffZ) >= threshhold)
            {
                return true;
            }
            return false;

        }
    }
}