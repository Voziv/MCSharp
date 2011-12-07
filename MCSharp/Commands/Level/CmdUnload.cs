using System;
using System.IO;
using System.Collections.Generic;
using MCSharp.World;
namespace MCSharp
{
    public class CmdUnload : Command
    {
        // Constructor 
        public CmdUnload (CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = true; /* By default no console support*/ }

        // Command usage help
        public override void Help (Player p)
        {
            p.SendMessage("/unload [level] - Unloads a level.");
        }

        // Code to run when used by a player
        public override void Use (Player p, string message)
        {
            bool blnLevelFound = false;
            Map targetLevel = null;

			foreach (Map level in Server.levels) 
            {
                if (level.name.ToLower() == message.ToLower())
                {
                    targetLevel = level;
                    blnLevelFound = true;
                }
            }


            if (blnLevelFound)
            {
                if (targetLevel != Server.mainLevel)
                {
                    foreach (var pl in Player.players)
                    {
                        if (pl.level == targetLevel)
                        {
                            Player.GlobalDie(pl, true);
                        }
                    }



                    foreach (var pl in Player.players) { if (pl.level == targetLevel) { pl.SendMotd(); } }
                    ushort x = (ushort) ((0.5 + Server.mainLevel.spawnx) * 32);
                    ushort y = (ushort) ((1 + Server.mainLevel.spawny) * 32);
                    ushort z = (ushort) ((0.5 + Server.mainLevel.spawnz) * 32);

                    List<Player> userList = new List<Player>();

                    foreach (Player pl in Player.players)
                    {
                        if (pl.level == targetLevel)
                        {
                            userList.Add(pl);
                        }
                    }

                    foreach (Player pl in userList)
                    {
                        Command.all.Find("goto").Use(pl, "main");
                        pl.SendMessage("Map unloaded, you were sent back to the main level.");
                    }
                    targetLevel.Save();
                    Server.levels.Remove(targetLevel);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    p.SendMessage("Map \"" + targetLevel.name + "\" unloaded.");
                }
                else
                {
                    p.SendMessage("You can't unload the main level.");
                }
            }
            else
            {
                p.SendMessage("There is no level \"" + message + "\" loaded.");
            }
        }

        // Code to run when used by console
        public override void Use (string message)
        {
            bool blnLevelFound = false;
            Map targetLevel = null;

            foreach (Map level in Server.levels)
            {
                if (level.name.ToLower() == message.ToLower())
                {
                    targetLevel = level;
                    blnLevelFound = true;
                }
            }


            if (blnLevelFound)
            {
                if (targetLevel != Server.mainLevel)
                {
                    foreach (var pl in Player.players)
                    {
                        if (pl.level == targetLevel)
                        {
                            Player.GlobalDie(pl, true);
                        }
                    }

                    foreach (var pl in Player.players)
                    {
                        if (pl.level == targetLevel)
                        {
                            pl.SendMotd();
                        }
                    }
                    ushort x = (ushort) ((0.5 + Server.mainLevel.spawnx) * 32);
                    ushort y = (ushort) ((1 + Server.mainLevel.spawny) * 32);
                    ushort z = (ushort) ((0.5 + Server.mainLevel.spawnz) * 32);

                    List<Player> userList = new List<Player>();

                    foreach (Player pl in Player.players)
                    {
                        if (pl.level == targetLevel)
                        {
                            userList.Add(pl);
                        }
                    }

                    foreach (Player pl in userList)
                    {
                        Command.all.Find("goto").Use(pl, "main");
                        pl.SendMessage("Map unloaded, you were sent back to the main level.");
                    }
                    targetLevel.Save();
                    Server.levels.Remove(targetLevel);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    Logger.Log("Map \"" + targetLevel.name + "\" unloaded.", LogType.ConsoleOutput);
                }
                else
                {
                    Logger.Log("You can't unload the main level.", LogType.ConsoleOutput);
                }
            }
            else
            {
                Logger.Log("There is no level \"" + message + "\" loaded.", LogType.ConsoleOutput);
            }
        }
    }
}