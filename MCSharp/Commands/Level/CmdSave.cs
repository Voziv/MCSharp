using System;

namespace MCSharp
{
    public class CmdSave : Command
    {
        // Constructor
        public CmdSave(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = true; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/save - Saves the level, not an actual backup.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
                if (message == "")
                {
                    p.level.Save();
                    p.SendMessage("Level \"" + p.level.name + "\" saved.");
                }
                else
                {
                    Help(p);
                }
        }

        // Code to run when used by the console
        public override void Use(string message)
        {
            Logger.Log("Forcing all loaded levels to save");
            foreach (Level l in Server.levels)
            {
                l.Save();
            }
        }
    }
}