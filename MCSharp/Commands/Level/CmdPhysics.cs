using System;

namespace MCSharp
{
    public class CmdPhysics : Command
    {
        // Constructor
        public CmdPhysics(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/physics <0/1/2> - Set the levels physics, 0-Off 1-On 2-Advanced");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                try
                {
                    int temp = int.Parse(message);
                    if (temp >= 0 && temp <= 2)
                    {
                        p.level.physics = temp;
                        switch (temp)
                        {
                            case 0:
                                p.level.ClearPhysics();
                                Player.GlobalMessageLevel(p.level, "Physics is now &cOFF&e on &b" + p.level.name + "&e.");
                                Logger.Log("Physics is now OFF on " + p.level.name + ".");
                                IRCBot.Say("Physics is now OFF on " + p.level.name + ".");
                                break;

                            case 1:
                                Player.GlobalMessageLevel(p.level, "Physics is now &aNormal&e on &b" + p.level.name + "&e.");
                                Logger.Log("Physics is now ON on " + p.level.name + ".");
                                IRCBot.Say("Physics is now ON on " + p.level.name + ".");
                                break;

                            case 2:
                                Player.GlobalMessageLevel(p.level, "Physics is now &aAdvanced&e on &b" + p.level.name + "&e.");
                                Logger.Log("Physics is now ADVANCED on " + p.level.name + ".");
                                IRCBot.Say("Physics is now ADVANCED on " + p.level.name + ".");
                                break;
                        }

                    }
                    else
                    {
                        p.SendMessage("Not a valid setting");
                    }
                }
                catch
                {
                    p.SendMessage("INVALID INPUT");
                }
            }
            else
            {
                Help(p); 
            }
        }
    }
}