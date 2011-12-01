using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSharp
{
    public class CmdPaste : Command
    {
          // Constructor
        public CmdPaste(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            if (!Properties.BetaMode)
            {
                p.SendMessage("/paste is not currently available in non-beta mode.");
            }
            p.SendMessage("/paste - Pastes the buffer in at the first position selected.");
            p.SendMessage("/paste ToDo: Change default to paste without air, and a switch to paste with air");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            if (!Properties.BetaMode)
            {
                p.SendMessage("/paste is not currently available in non-beta mode.");
            }
            else
            {
                p.SendMessage("Place a block to determine the paste start");
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

            byte[,,] blockBuffer;

            int tempX = Math.Abs(p.copyBuffer.DimX + x - 1);
            int tempY = Math.Abs(p.copyBuffer.DimY + y - 1);
            int tempZ = Math.Abs(p.copyBuffer.DimZ + z - 1); ;



            ushort x2 = (tempX > p.level.width - 1) ? (ushort)(p.level.width - 1) : (ushort)tempX;
            ushort y2 = (tempY > p.level.depth - 1) ? (ushort)(p.level.depth - 1) : (ushort)tempY;
            ushort z2 = (tempZ > p.level.height - 1) ? (ushort)(p.level.height - 1) : (ushort)tempZ;
            ushort dimX = (ushort)Math.Abs(x - x2);
            ushort dimY = (ushort)Math.Abs(y - y2);
            ushort dimZ = (ushort)Math.Abs(z - z2);

            dimX++;
            dimY++;
            dimZ++;

            BlockPos pos1, pos2;

            pos1.x = x;
            pos1.y = y;
            pos1.z = z;
            pos2.x = x2;
            pos2.y = y2;
            pos2.z = z2;

            try
            {
                // Resize the buffer for undo
                blockBuffer = new byte[Math.Abs(x - x2) + 1, Math.Abs(y - y2) + 1, Math.Abs(z - z2) + 1];

                // Store where we're copying to the undo buffer
                for (ushort xx = Math.Min(x, x2); xx <= Math.Max(x, x2); ++xx)
                {
                    for (ushort yy = Math.Min(y, y2); yy <= Math.Max(y, y2); ++yy)
                    {
                        for (ushort zz = Math.Min(z, z2); zz <= Math.Max(z, z2); ++zz)
                        {
                            blockBuffer[xx - x, yy - y, zz - z] = p.level.GetTile(xx, yy, zz);
                        }
                    }
                }

                // Move the blocks to the undo buffer
                p.undoPasteBuffer.SetBuffer(blockBuffer, dimX, dimY, dimZ, pos1, pos2, p.level.name);

                
                // Do the block changes
                for (ushort xx = Math.Min(x, x2); xx <= Math.Max(x, x2); ++xx)
                {
                    for (ushort yy = Math.Min(y, y2); yy <= Math.Max(y, y2); ++yy)
                    {
                        for (ushort zz = Math.Min(z, z2); zz <= Math.Max(z, z2); ++zz)
                        {
                            p.level.Blockchange(p, xx, yy, zz, p.copyBuffer.GetTile((ushort)(xx - x), (ushort)(yy - y), (ushort)(zz - z))); // Make the change for everyone
                        }
                    }
                }
            }
            catch
            {
                p.SendMessage("There was an error during /paste! Use /undoLastPaste to undo the damage.");
            }

        }

        void BufferAdd(List<BlockPos> list, ushort x, ushort y, ushort z)
        {
            BlockPos pos; pos.x = x; pos.y = y; pos.z = z; list.Add(pos);
        }
    }
}
