using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MCSharp.World;
namespace MCSharp
{
    class CmdDeleteLvl : Command
    {
        // Constructor
        public CmdDeleteLvl(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/deletelvl - permanently deletes a level.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                bool levelfound = false;
                string levelName = "";

                // Try and find the level in the levels directory.
                DirectoryInfo di = new DirectoryInfo("levels/");
                FileInfo[] fi = di.GetFiles("*.lvl");
                foreach (FileInfo file in fi)
                {
                    levelName = file.Name.Replace(".lvl", "").ToLower();
                    if (levelName == message && levelName != "main")
                    {
                        levelfound = true;
                        break;
                    }
                }

                // If the level is found, unload if necessary, and then delete all related files
                if (levelfound)
                {
                    // Is the level loaded?
                    foreach (Map l in Server.levels)
                    {
                        if (l.name.ToLower() == levelName)
                        {
                            // Unload the level
                            Command.all.Find("unload").Use(p, levelName);
                            break;
                        }
                    }
                    try
                    {
                        // Check and delete backups
                        if (File.Exists("levels/" + levelName + ".lvl.backup"))
                        {
                            if (!Directory.Exists("levels/deleted"))
                            {
                                Directory.CreateDirectory("levels/deleted/");
                            }
                            File.Move("levels/" + levelName + ".lvl.backup", "levels/deleted/" + levelName + ".lvl.backup");
                        }

                        // Delete Map file
                        File.Delete("levels/" + levelName + ".lvl");

                        // And backup folders
                        if (Directory.Exists("levels/backups/" + levelName))
                            Directory.Delete("levels/backups/" + levelName, true);
                        p.SendMessage(message + " deleted");
                    }
                    catch (Exception e)
                    {
                        Logger.Log("Error while deleting " + levelName + ".", LogType.Error);
                        Logger.Log(e.Message, LogType.ErrorMessage);
                        p.SendMessage("Error while deleting " + message + ". Please check the server log.");
                    }
                }
                else
                {
                    p.SendMessage(message + " could not be found");
                }
            }
            else
            { 
                Help(p); 
            }
        }
    }
}
