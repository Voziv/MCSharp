using System;

namespace MCSharp
{
    public class CmdPermissionBuild : Command
    {
        // Constructor
        public CmdPermissionBuild(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/PerBuild <rank> - Sets build permission for a map.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            int number = message.Split(' ').Length;
            string strLevelName;
            string strPermission = message;
            bool blnLevelLoaded = false;
            Level targetLevel;
            LevelPermission lvlPermission;

            if (message != "" || number <= 2)
            {
                targetLevel = p.level;
                strLevelName = targetLevel.name;

                if (number == 1)
                {
                    lvlPermission = Level.PermissionFromName(message);
                    if (lvlPermission != LevelPermission.Null)
                    {
                        targetLevel.permissionbuild = lvlPermission;

                        Logger.Log(strLevelName + " Build permission changed to " + strPermission + ".");
                        Player.GlobalMessageLevel(targetLevel, "Build permission changed to " + strPermission + ".");
                    }
                    else
                    {
                        p.SendMessage("\"" + strPermission + "\" is not a valid rank");
                    }
                }
                else
                {
                    int pos = message.IndexOf(' ');
                    strLevelName = message.Substring(0, pos).ToLower();
                    strPermission = message.Substring(pos + 1).ToLower();
                    lvlPermission = Level.PermissionFromName(strPermission);

                    if (lvlPermission != LevelPermission.Null)
                    {
                        foreach (Level level in Server.levels)
                        {
                            if (level.name.ToLower() == strLevelName.ToLower())
                            {
                                blnLevelLoaded = true;
                                targetLevel = level;
                                break;
                            }
                        }

                        if (blnLevelLoaded)
                        {
                            targetLevel.permissionbuild = lvlPermission;
                            Logger.Log(strLevelName + " Build permission changed to " + strPermission + ".");
                            Player.GlobalMessageLevel(targetLevel, "build permission changed to " + strPermission + ".");
                            if (p.level != targetLevel)
                            {
                                p.SendMessage("Build permission changed to " + strPermission + " on " + strLevelName + ".");
                            }
                        }
                        else
                        {
                            p.SendMessage("There is no level \"" + strPermission + "\" loaded.");
                        }
                    }
                    else
                    {
                        p.SendMessage("\"" + strPermission + "\" is not a valid rank");
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