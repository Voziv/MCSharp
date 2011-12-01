using System;

namespace MCSharp
{
    public class CmdAid : Command
    {
        // Constructor
        public CmdAid(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/aid <block> - Builds a block underneath the player");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            byte type = Block.yellow; // Default value
            message = message.ToLower().Trim();
            int number = message.Split(' ').Length;
            
            //p.SendMessage("DEBUG /AID: Number = " + number);
            if (number < 2) 
            {
                if (number == 1) // Set the block type to what the user wanted
                {
                    type = Block.Byte(message);
                    if (type == 255)
                    {
                        type = Block.yellow;
                    }
                }
                if (p.Rank >= GroupEnum.AdvBuilder && !Block.Placable(type) && !Block.AdvPlacable(type))
                {
                    p.SendMessage("You're not allowed to place that block type.");
                }
                else
                {
                    double x = p.pos[0];
                    double y = p.pos[1];
                    double z = p.pos[2];

                    x = Math.Round((x / 32) - 0.4, MidpointRounding.AwayFromZero);
                    y = Math.Round((y / 32), MidpointRounding.AwayFromZero) - 3;
                    z = Math.Round((z / 32) - 0.4, MidpointRounding.AwayFromZero);

                    Logger.Log("Command /aid debug info. Used by " + p.name, LogType.Debug);
                    Logger.Log("Player X: " + x, LogType.Debug);
                    Logger.Log("Player Y: " + y, LogType.Debug);
                    Logger.Log("Player Z: " + z, LogType.Debug);
                    p.SendBlockchange((ushort)x, (ushort)y, (ushort)z, (byte)(type)); // take currently held block?
                }
            }
            else
            {
                Help(p);
            }
        }
    }
}