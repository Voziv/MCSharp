using System;
using System.Collections.Generic;

namespace MCSharp
{
    public class CmdCopy : Command
    {
        // Constructor
        public CmdCopy(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            if (!Properties.BetaMode)
            {
                p.SendMessage("/copy is not currently available in non-beta mode.");
            }
            p.SendMessage("/copy - Copies the selected area into the players buffer.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (!Properties.BetaMode)
            {
                p.SendMessage("/copy is not currently available in non-beta mode.");
            }
            else
            {
                p.SendMessage("Place two blocks to determine the edges.");
                p.ClearBlockchange();
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
            }
        }

        // Grab the first position
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos bp = new CatchPos();
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }

        // Get the second position
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            unchecked { if (cpos.type != (byte)-1) { type = cpos.type; } }

            byte[,,] blockBuffer;

            ushort lowX = Math.Min(cpos.x, x);
            ushort lowY = Math.Min(cpos.y, y);
            ushort lowZ = Math.Min(cpos.z, z);

            ushort highX = Math.Max(cpos.x, x);
            ushort highY = Math.Max(cpos.y, y);
            ushort highZ = Math.Max(cpos.z, z);


            ushort dimX = (ushort)Math.Abs(cpos.x - x);
            ushort dimY = (ushort)Math.Abs(cpos.y - y);
            ushort dimZ = (ushort)Math.Abs(cpos.z - z);

            dimX++;
            dimY++;
            dimZ++;

            Level copyLevel = p.level;

            // Now that we have the positions, check the size
            // If it's too big, cancel
            if (dimX * dimY * dimZ <= p.group.CuboidLimit || p.group.CuboidLimit == 0)
            {
                

                try
                {
                    // Resize the buffer
                    blockBuffer = new byte[highX - lowX + 1, highY - lowY + 1, highZ - lowZ + 1];

                    // Let the player know we're copying blocks now
                    p.SendMessage("Copying " + blockBuffer.Length + " blocks.");


                    // Add the tiles to a temporary buffer
                    for (ushort xx = lowX; xx <= highX; ++xx)
                    {
                        for (ushort yy = lowY; yy <= highY; ++yy)
                        {
                            for (ushort zz = lowZ; zz <= highZ; ++zz)
                            {
                                blockBuffer[xx - lowX, yy - lowY, zz - lowZ] = copyLevel.GetTile(xx, yy, zz);
                            }
                        }
                    }
                    // Replace the players buffer
                    p.copyBuffer.SetBuffer(blockBuffer, dimX, dimY, dimZ);
                    p.SendMessage("The block copy was successful!");
                }
                catch (Exception e)
                {
                    Logger.Log("Error copying blocks for " + p.name, LogType.Error);
                    Logger.Log(e.Message, LogType.ErrorMessage);
                    p.SendMessage("There was an error doing /copy!");
                }
            }
            else
            {
                p.SendMessage("You're trying to copy " + dimX * dimY * dimZ + " blocks.");
                p.SendMessage("Your block limit is " + p.group.CuboidLimit.ToString() + " blocks. Copy in stages.");
            }
        }

        void BufferAdd(List<BlockPos> list, ushort x, ushort y, ushort z)
        {
            BlockPos pos; pos.x = x; pos.y = y; pos.z = z; list.Add(pos);
        }
    }
}