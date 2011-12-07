using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MCSharp.World;
namespace MCSharp
{
    class CmdRestore : Command
    {
        // Constructor
        public CmdRestore(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/restore <number> - restores a previous backup of the current map");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                string strBackupLevel = "levels/backups/" + p.level.name + "/" + message + "/" + p.level.name + ".lvl";
                string strCurrentLevel = "levels/" + p.level.name + ".lvl";
                Logger.Log(strBackupLevel, LogType.Debug);
                if (File.Exists(strBackupLevel))
                {
                    try
                    {
                        File.Copy(strBackupLevel, strCurrentLevel, true);
                        Map temp = Map.Load(p.level.name);
                        if (temp != null)
                        {
                            p.level.spawnx = temp.spawnx;
                            p.level.spawny = temp.spawny;
                            p.level.spawnz = temp.spawnz;

                            p.level.height = temp.height;
                            p.level.width = temp.width;
                            p.level.depth = temp.depth;

                            p.level.blocks = temp.blocks;

                            // Disable physics before restoring
                            p.level.Physics = Physics.Off;
                            p.level.ClearPhysics();

                            ushort x, y, z;
                            
                            Player.players.ForEach(delegate(Player pl) 
                            {
                                if (pl.level == p.level) 
                                {
                                    pl.Loading = true;

                                    //Kills current player list for player
                                    foreach (Player pl2 in Player.players)
                                    {
                                        if (pl.level == pl2.level && pl != pl2)
                                        {
                                            pl.SendDie(pl2.id); 
                                        }
                                    }

                                    

                                    pl.ClearBlockchange();
                                    pl.BlockAction = 0;
                                    pl.painting = false;
                                    Player.GlobalDie(pl, true);
                                    pl.SendMotd();
                                    pl.SendMap();
                                    x = (ushort)((0.5 + pl.level.spawnx) * 32);
                                    y = (ushort)((1 + pl.level.spawny) * 32);
                                    z = (ushort)((0.5 + pl.level.spawnz) * 32);
                                    if (!pl.hidden)
                                    {
                                        Player.GlobalSpawn(pl, x, y, z, pl.level.rotx, pl.level.roty, true);
                                    }
                                    else unchecked
                                        {
                                            pl.SendPos((byte)-1, x, y, z, pl.level.rotx, pl.level.roty);
                                        }
                                    foreach (Player pl2 in Player.players)
                                    {
                                        if (pl2.level == pl.level && pl != pl2 && !pl2.hidden)
                                        {
                                            pl.SendSpawn(pl2.id, pl2.color + pl2.name, pl2.pos[0], pl2.pos[1], pl2.pos[2], pl2.rot[0], pl2.rot[1]);
                                        }
                                    }
                                    
                                    pl.Loading = false;
                                }
                                GC.Collect();
                                GC.WaitForPendingFinalizers();
                            });
                        }
                        else
                        {
                            Logger.Log("Restore didn't work, attempting last backup", LogType.Debug);
                            File.Copy(strCurrentLevel + ".backup", strCurrentLevel, true);
                        }

                    }
                    catch
                    {
                        Logger.Log("Restore fail", LogType.Error);
                    }
                }
                else
                {
                    p.SendMessage("Backup " + message + " does not exist.");
                }
            }
            else
            {
                if (Directory.Exists("levels/backups/" + p.level.name))
                {
                    int backupNumber = Directory.GetDirectories("levels/backups/" + p.level.name).Length;
                    p.SendMessage(p.level.name + " has " + backupNumber.ToString() + " backups .");
                }
                else
                {
                    p.SendMessage(p.level.name + " has no backups yet.");
                }
            }

        }
    }
}
