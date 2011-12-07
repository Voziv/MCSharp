using System;
using System.IO;
using MCSharp.World;
namespace MCSharp
{
    public class CmdSendLevel : Command
    {
        // Constructor
        public CmdSendLevel(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/sendlevel <player> <level> - Forces a player to another level");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "" || message.Split(' ').Length != 2)
            {
                Player target = Player.Find(message.Split(' ')[0]);
                string levelName = message.Split(' ')[1];
                bool blnLevelLoaded = false;

                if (target == null)
                {
                    p.SendMessage("FAILED - No such player!");
                }
                else if (target.Rank >= p.Rank)
                {
                    p.SendMessage("You cannnot send a player who is an equal or greater rank than yourself");
                }
                else
                {
                    foreach (Map l in Server.levels)
                    {
                        if (l.name == levelName)
                        {
                            if (target.level.name != levelName)
                            {
                                target.SendMessage(p.name + " has sent you to " + levelName);
                                Command.all.Find("goto").Use(target, levelName);
                                blnLevelLoaded = true;
                                break;
                            }
                            else
                            {
                                p.SendMessage("Player is already on that level!");
                                blnLevelLoaded = true;
                                break;
                            }
                        }
                    }
                    if (!blnLevelLoaded)
                    {
                        DirectoryInfo di = new DirectoryInfo("levels/");
                        FileInfo[] fi = di.GetFiles("*.lvl");
                        foreach (FileInfo file in fi)
                        {
                            if (file.Name.Replace(".lvl", "").ToLower() == levelName.ToLower())
                            {
                                p.SendMessage("That Map is not loaded yet!");
                            }
                        }
                        p.SendMessage("No such level!");
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