using System;
using System.IO;
using System.Collections.Generic;

namespace MCSharp
{
	public class CmdLevels : Command 
    {
		// Constructor
        public CmdLevels(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/levels - Lists all levels.");
        }

        // Code to run when used by a player
		public override void Use(Player p,string message)  
        { // TODO
            if (message == "")
            {
                // Loaded Levels
                List<string> levels = new List<string>(Server.levels.Count);
                message = Server.mainLevel.name;
                string message2 = "";
                levels.Add(Server.mainLevel.name.ToLower());
                bool Once = false;
                Server.levels.ForEach(delegate(Level level)
                {
                    if (level != Server.mainLevel)
                    {
                        if (level.permissionvisit <= p.group.Permission)
                        {
                            message += ", " + level.name;
                            levels.Add(level.name.ToLower());
                        }
                        else
                        {
                            if (!Once)
                            {
                                Once = true;
                                message2 += level.name;
                            }
                            else
                            {
                                message2 += ", " + level.name;
                            }
                        }
                    }
                });
                p.SendMessage("Loaded: &2" + message);
                p.SendMessage("Can't Goto: &c" + message2);

                // Unloaded levels
                message = "";
                DirectoryInfo di = new DirectoryInfo("levels/");
                FileInfo[] fi = di.GetFiles("*.lvl");
                Once = false;
                foreach (FileInfo file in fi)
                {
                    if (!levels.Contains(file.Name.Replace(".lvl", "").ToLower()))
                    {
                        if (!Once)
                        {
                            Once = true;
                            message += file.Name.Replace(".lvl", "");
                        }
                        else
                        {
                            message += ", " + file.Name.Replace(".lvl", "");
                        }
                    }
                }
                p.SendMessage("Unloaded: &4" + message);

            }
            else
            {
                Help(p);
            }	
		}
	}
}