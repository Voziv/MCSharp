using System;

namespace MCSharp
{
    public class CmdReveal : Command
    {
        
        // Constructor
        public CmdReveal(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/reveal <player> - Reveals map to banned user.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                Player target = Player.Find(message);
                if (target != null)
                {
                    if (target.Rank == GroupEnum.Banned)
                    {
                        target.Loading = true;
                        ushort x = p.pos[0];
                        ushort y = p.pos[1];
                        ushort z = p.pos[2];
                        byte rotX = p.rot[0];
                        byte rotY = p.rot[1];

                        //who.SendMotd(); who.SendMap();

                        target.ClearBlockchange();
                        target.BlockAction = 0;
                        target.SendMotd();
                        target.SendMap();

                        unchecked
                        {
                            target.SendPos((byte)-1, x, y, z,
                                rotX,
                                rotY);
                        }

                        target.SendMessage("Oops, looks like all your greifing was for nothing!");
                        p.SendMessage("Map revealed for: " + target.color + target.name);
                        Player.GlobalMessageOps("-" + p.color + p.name + "&e just revealed the map to: " + target.color + target.name);
                        target.Loading = false;
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                    else
                    {
                        p.SendMessage("Player is not banned!");
                    }
                }
                else
                {
                    p.SendMessage("There is no player \"" + message + "\"!");
                }
            }
            else
            {
                Help(p);
            }
        }
    }
}