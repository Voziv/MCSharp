using System;

namespace MCSharp
{
    public class CmdUnban : Command
    {
        
        // Constructor
        public CmdUnban(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/unban <player> - Unbans a player.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                if (Player.ValidName(message))
                {
                    if (Server.banned.Contains(message))
                    {
                        Player target = Player.Find(message);
                        if (target == null)
                        {
                            Player.GlobalMessage(message + " &8(banned)&e is now " + Group.standard.Color + Group.standard.Name + "&e!");
                        }
                        else
                        {
                            target.group = Group.standard;
                            Player.GlobalChat(target, target.color + target.name + "&e is now " + Group.standard.Color + Group.standard.Name + "&e!", false);
                            Player.GlobalDie(target, false);
                            Player.GlobalSpawn(target, target.pos[0], target.pos[1], target.pos[2], target.rot[0], target.rot[1], false);
                        }
                        Server.banned.Remove(message);
                        Logger.Log("UNBANNED: " + message.ToLower());
                    }
                    else
                    {
                        p.SendMessage(message + " isn't banned.");
                    }
                }
                else
                {
                    p.SendMessage("Invalid name \"" + message + "\".");
                }
            }
            else
            {
                Help(p);
            }
        }

    }
}