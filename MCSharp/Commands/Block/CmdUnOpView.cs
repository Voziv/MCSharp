using System;
using System.Collections.Generic;
using System.Linq;

namespace MCSharp
{
    public class CmdUnOpView : Command
    {
        public byte[] ignoreList = { 255, 255, 255, 255, 255, 255, 255 };
        public byte type = (byte)0;
        public Block opMats = new Block();

        // Constructor
        public CmdUnOpView(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }
        
        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/unopview - Reverts pink showing op_material to original texture");
            p.SendMessage("/unopview <block> ignores that block, max 7");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            // Clear previously ignored blocks
            for (int i = 0; i < ignoreList.Length - 1; i++)
            {
                ignoreList[i] = 255;
            }
            CatchPos cpos = new CatchPos(); cpos.solid = SolidType.solid; unchecked { cpos.type = (byte)-1; }
            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            int counter = 0;

            if (message != "")
            {
                message = message.ToLower();
                message = " " + message;
                for (int i = 0; i < opMats.opBlocks.Length; i++)
                {
                    if ((message.Contains(" " + Block.Name(opMats.opBlocks[i]))) || (message.Contains(" " + Block.Name(opMats.regBlocks[i]))))
                    {
                        ignoreList[counter] = opMats.opBlocks[i];
                        counter += 1;
                        if (counter > ignoreList.Length - 1)
                        {
                            break;
                        }
                    }
                }
                if (message.Contains("all"))
                {   // User used "all"
                    Blockchange2(p, p.level.width, p.level.depth, p.level.height, type);
                    return;
                }
            }
            // Code will only be reached if message doesn't contain "all"
            p.SendMessage("Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z); //get the tile type of first set and set that to byte b
            p.SendBlockchange(x, y, z, b);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            unchecked { if (cpos.type != (byte)-1) { type = cpos.type; } }
            List<Pos> buffer = new List<Pos>();
            switch (cpos.solid)
            {
                case SolidType.solid:
                    if (!Server.operators.Contains(p.name))
                    {
                        int attemptedLimit = (Math.Abs(cpos.x - x) * Math.Abs(cpos.y - y) * Math.Abs(cpos.z - z));
                        if (attemptedLimit > p.group.CuboidLimit && p.group.CuboidLimit != 0) //OPERATOR Lock LIMIT, same as cuboid
                        {
                            p.SendMessage("You're trying to unview " + attemptedLimit + " blocks.");
                            p.SendMessage("Your block limit is " + p.group.CuboidLimit.ToString() + " blocks. Build in stages.");
                            return;
                        }
                    }
                    buffer.Capacity = Math.Abs(cpos.x - x) * Math.Abs(cpos.y - y) * Math.Abs(cpos.z - z);
                    for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                        for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                            for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                                /*if (p.level.GetTile(xx, yy, zz) != type) {*/
                                BufferAdd(buffer, xx, yy, zz); //}
                    break;
            }

            p.SendMessage("Processing " + buffer.Count.ToString() + " blocks.");

            buffer.ForEach(delegate(Pos pos)
            {
                byte bl = p.level.GetTile(pos.x, pos.y, pos.z); //Get the block that is there at the moment
                if (!ignoreList.Contains(bl))// if the block is not being ignored
                {
                    if (opMats.opBlocks.Contains(bl))   // if the block is an op_material// Saves bandwidth not sending reg_material updates
                    {
                        p.SendBlockchange(pos.x, pos.y, pos.z, bl); // Send original texture back to client
                    }
                }
            }
        );
            p.SendMessage("Unopview complete.");
        }


        void BufferAdd(List<Pos> list, ushort x, ushort y, ushort z)
        {
            Pos pos; pos.x = x; pos.y = y; pos.z = z; list.Add(pos);
        }
    }
}