using System;
using MCSharp.World;
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
                switch (p.level.Physics)
                {
                    case Physics.Off:
                        p.SendMessage("Physics is &cOFF&e.");
                        break;

                    case Physics.Normal:
                        p.SendMessage("Physics is &aNormal&e.");
                        break;

                    case Physics.Advanced:
                        p.SendMessage("Physics is &aAdvanced&e.");
                        break;
                }

                p.SendMessage("Build rank = " + Map.PermissionToName(p.level.permissionbuild) + " : Visit rank = " + Map.PermissionToName(p.level.permissionvisit) + ".");
            }
            else
            { 
                Help(p);
            }
        }
    }
}