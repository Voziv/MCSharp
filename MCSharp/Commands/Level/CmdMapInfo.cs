using System;

namespace MCSharp
{
    public class CmdMapInfo : Command
    {
        // Constructor
        public CmdMapInfo(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/mapinfo - Display details of the current map.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                p.SendMessage("Currently on &b" + p.level.name + "&e X:" + p.level.width.ToString() + " Y:" + p.level.depth.ToString() + " Z:" + p.level.height.ToString());
                switch (p.level.physics)
                {
                    case 0:
                        p.SendMessage("Physics is &cOFF&e.");
                        break;

                    case 1:
                        p.SendMessage("Physics is &aNormal&e.");
                        break;

                    case 2:
                        p.SendMessage("Physics is &aAdvanced&e.");
                        break;
                }

                p.SendMessage("Build rank = " + Level.PermissionToName(p.level.permissionbuild) + " : Visit rank = " + Level.PermissionToName(p.level.permissionvisit) + ".");
            }
            else
            { 
                Help(p);
            }
        }
    }
}