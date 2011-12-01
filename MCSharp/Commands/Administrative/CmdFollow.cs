using System;
using System.Timers;
using System.Collections.Generic;

namespace MCSharp
{
    public class CmdFollow : Command
    {
        private Timer fTimer = new Timer(50);
        private Player adminPlayer = null;
        private Player targetPlayer = null;
        //private ushort[] lastPos = new ushort[3] { 0, 0, 0 };
        //private byte[] lastRot = new byte[2] { 0, 0 };

        private Dictionary<string, string> dList = new Dictionary<string, string>();

        // Constructor
        public CmdFollow(CommandGroup g, GroupEnum group, string name) : base(g, group, name)
        {
            fTimer.Elapsed += new ElapsedEventHandler(fTimer_Elapsed);
            fTimer.Enabled = true;
        }
        
        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/follow <player> - Follow someone.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                if (dList.ContainsKey(p.name))
                {
                    p.SendMessage("Stopped following: " + dList[p.name]);
                    Player oldPlayer = Player.Find(dList[p.name]);
                    if (oldPlayer != null)
                    {
                        p.SendSpawn(oldPlayer.id, oldPlayer.name, oldPlayer.pos[0], oldPlayer.pos[1], oldPlayer.pos[2], oldPlayer.rot[0], oldPlayer.rot[1]);
                    }
                    dList.Remove(p.name);
                    return;
                }
                else
                {
                    Help(p);
                    return;
                }
            }

            Player who = Player.Find(message);

            if (who == null)
            {
                p.SendMessage("No such player!");
                return;
            }
            else
            {
                if (who.hidden)
                {
                    p.SendMessage("No such player!");
                    return;
                }
                else if (who.name == p.name)
                {
                    p.SendMessage("You have a better chance at chasing your tail than following yourself!");
                    return;
                }
            }

            if (!dList.ContainsKey(p.name))
            {
                if (!p.hidden)
                {
                    Command.all.Find("hide").Use(p, "");
                }
                p.SendDie(who.id);
                dList.Add(p.name, who.name);
                p.SendMessage("Now following: " + who.color + who.name);
                p.SendMessage("--Make sure you have flight turned ON!--");
                p.SendMessage("--Otherwise expect jittery movement!--");
            }
            else
            {
                Player oldPlayer = Player.Find(dList[p.name]);
                if (oldPlayer != null)
                {
                    p.SendSpawn(oldPlayer.id, oldPlayer.name, oldPlayer.pos[0], oldPlayer.pos[1], oldPlayer.pos[2], oldPlayer.rot[0], oldPlayer.rot[1]);
                }
                dList[p.name] = who.name;
                p.SendDie(who.id);
            }

        }

        void fTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (string s in dList.Keys)
            {
                adminPlayer = Player.Find(s);
                targetPlayer = Player.Find(dList[s]);

                if (targetPlayer != null)
                {
                    if (targetPlayer.level != adminPlayer.level)
                    {
                        dList.Remove(adminPlayer.name);
                        adminPlayer.SendMessage("Stopped following: " + targetPlayer.color + targetPlayer.name);
                        adminPlayer.SendMessage("Player moved to: " + targetPlayer.level.name);
                        return;
                    }
                    //if (lastPos != targetPlayer.pos || lastRot != targetPlayer.rot)
                    //{
                    //lastPos = targetPlayer.pos;
                    //lastRot = targetPlayer.rot;
                    unchecked { adminPlayer.SendPos((byte)-1, targetPlayer.pos[0], targetPlayer.pos[1], targetPlayer.pos[2], targetPlayer.rot[0], targetPlayer.rot[1]); }
                    //}
                }
                else
                {
                    adminPlayer.SendMessage("Following stopped!");
                    dList.Remove(adminPlayer.name);
                    break;
                }
            }
        }
    }
}
