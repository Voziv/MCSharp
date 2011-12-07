using System;
using MCSharp.World;
namespace MCSharp
{
    public class CmdPermissionVisit : Command
    {
        // Constructor
        public CmdPermissionVisit(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/PerVisit <rank> - Sets visiting permission for a map.");
        }

        // Code to be run when used
        public override void Use(Player p, string message)
        {
            int number = message.Split(' ').Length;
            string strLevelName;
            string strPermission = message;
            bool blnLevelLoaded = false;
            Map targetLevel;
            LevelPermission lvlPermission;

            if (message != "" || number <= 2)
            {
                targetLevel = p.level;
                strLevelName = targetLevel.name;
                if (number == 1)
                {
                    lvlPermission = Map.PermissionFromName(message);
                    if (lvlPermission != LevelPermission.Null)
                    {
                        targetLevel.permissionvisit = lvlPermission;

                        Logger.Log(strLevelName + " Visit permission changed to " + strPermission + ".");
                        Player.GlobalMessageLevel(targetLevel, "Visit permission changed to " + strPermission + ".");
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
                    lvlPermission = Map.PermissionFromName(strPermission);

                    if (lvlPermission != LevelPermission.Null)
                    {
                        foreach (Map level in Server.levels)
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
                            targetLevel.permissionvisit = lvlPermission;
                            Logger.Log(strLevelName + " Visit permission changed to " + strPermission + ".");
                            Player.GlobalMessageLevel(targetLevel, "visit permission changed to " + strPermission + ".");
                            if (p.level != targetLevel)
                            {
                                p.SendMessage("Visit permission changed to " + strPermission + " on " + strLevelName + ".");
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