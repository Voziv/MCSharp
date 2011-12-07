using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using MCSharp.World;
namespace MCSharp
{
    public class CmdLoad : Command
    {
        // Constructor
        public CmdLoad(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = true; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/load <level> [<physics level>] - Loads a level. You can optionally set the physics to 0|1|2");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            try
            {
                if (message != "" || message.Split(' ').Length <= 2)
                {
                    int pos = message.IndexOf(' ');
                    string phys = "0";
                    bool blnAlreadyLoaded = false;

                    if (pos != -1)
                    {
                        phys = message.Substring(pos + 1);
                        message = message.Substring(0, pos).ToLower();
                    }
                    else
                    {
                        message = message.ToLower();
                    }

                    // Make sure the level isn't already loaded
                    foreach (Map l in Server.levels)
                    {
                        if (l.name == message)
                        {
                            blnAlreadyLoaded = true;
                            break;
                        }
                    }

                    if (!blnAlreadyLoaded)
                    {
                        if (Server.levels.Capacity > 1)
                        {
                            if (Server.levels.Count < Server.levels.Capacity)
                            {
                                if (File.Exists("levels/" + message + ".lvl"))
                                {
                                    // Attempt to load the level
                                    Map level = Map.Load(message);
                                    if (level == null)
                                    {
                                        if (File.Exists("levels/" + message + ".lvl.backup"))
                                        {
                                            Logger.Log("Atempting to load backup.");
                                            File.Copy("levels/" + message + ".lvl.backup", "levels/" + message + ".lvl", true);
                                            level = Map.Load(message);

                                        }
                                        else
                                        {
                                            p.SendMessage("Backup of " + message + " does not exist.");
                                        }

                                    }

                                    // Make sure we loaded something before adding it the level list
                                    if (level != null)
                                    {
                                        lock (Server.levels)
                                        {
                                            Server.levels.Add(level);
                                        }
                                        Player.GlobalMessage("Map \"" + level.name + "\" loaded.");
                                        try
                                        {
                                            int temp = int.Parse(phys);
                                            if (temp >= 0 && temp <= 2)
                                            {
                                                level.Physics = (Physics)temp;
                                            }
                                        }
                                        catch
                                        {
                                            if (!p.disconnected) p.SendMessage("Physics variable invalid");
                                        }
                                    }
                                    else
                                    {
                                        p.SendMessage("Failed to load the backup of " + message);
                                    }
                                }
                                else
                                {
                                    p.SendMessage("Map \"" + message + "\" doesn't exist!");
                                }
                            }
                            else
                            {
                                p.SendMessage("You can't load more than " + Server.levels.Capacity + " levels!");
                            }
                        }
                        else
                        {
                            p.SendMessage("Map capacity is 1 or lower, you can't load any levels!");
                        }

                    }
                    else
                    {
                        p.SendMessage(message + " is already loaded!");
                    }

                }
                else
                {
                    Help(p);
                }       
            }
            catch (Exception e)
            {
                Player.GlobalMessage("An error occured with /load");
                Logger.Log("An error occured with /load", LogType.Error);
                Logger.Log(e.Message, LogType.ErrorMessage);
            }
        }

        // Code to run when used by the console
        public override void Use(string message)
        {
            try
            {
                if (message != "" || message.Split(' ').Length <= 2)
                {
                    int pos = message.IndexOf(' ');
                    string phys = "0";

                    if (pos != -1)
                    {
                        phys = message.Substring(pos + 1);
                        message = message.Substring(0, pos).ToLower();
                    }
                    else
                    {
                        message = message.ToLower();
                    }

                    if (!Map.Loaded(message))
                    {
                        if (Server.levels.Capacity > 1)
                        {
                            if (Server.levels.Count < Server.levels.Capacity)
                            {
                                if (Map.Exists(message))
                                {
                                    // Attempt to load the level
                                    Map level = Map.Load(message);
                                    if (level == null)
                                    {
                                        if (File.Exists("levels/" + message + ".lvl.backup"))
                                        {
                                            Logger.Log("Atempting to load backup.", LogType.ConsoleOutput);
                                            File.Copy("levels/" + message + ".lvl.backup", "levels/" + message + ".lvl", true);
                                            level = Map.Load(message);

                                        }
                                        else
                                        {
                                            Logger.Log("Backup of " + message + " does not exist.", LogType.ConsoleOutput);
                                        }

                                    }

                                    // Make sure we loaded something before adding it the level list
                                    if (level != null)
                                    {
                                        lock (Server.levels)
                                        {
                                            Server.levels.Add(level);
                                        }
                                        Player.GlobalMessage("Map \"" + level.name + "\" loaded.");
                                        try
                                        {
                                            int temp = int.Parse(phys);
                                            if (temp >= 0 && temp <= 2)
                                            {
                                                level.Physics = (Physics)temp;
                                            }
                                        }
                                        catch
                                        {
                                            Logger.Log("Physics variable invalid", LogType.ConsoleOutput);
                                        }
                                    }
                                    else
                                    {
                                        Logger.Log("Failed to load the backup of " + message, LogType.ConsoleOutput);
                                    }
                                }
                                else
                                {
                                    Logger.Log("Map \"" + message + "\" doesn't exist!", LogType.ConsoleOutput);
                                }
                            }
                            else
                            {
                                Logger.Log("You can't load more than " + Server.levels.Capacity + " levels!", LogType.ConsoleOutput);
                            }
                        }
                        else
                        {
                            Logger.Log("Map capacity is 1 or lower, you can't load any levels!", LogType.ConsoleOutput);
                        }
                    }
                    else
                    {
                        Logger.Log(message + " is already loaded!", LogType.ConsoleOutput);
                    }
                }
                else
                {
                    // We need to print help to the console somehow
                    Logger.Log("Someday the console will support help messages", LogType.ConsoleOutput);
                }
            }
            catch (Exception e)
            {
                Logger.Log("An error occured with /load", LogType.Error);
                Logger.Log(e.Message, LogType.ErrorMessage);
            }
        }
    }
}