using System;
using System.Collections.Generic;
using MCSharp.World;
namespace MCSharp
{
    public class CmdCuboid : Command
    {
        // Constructor
        public CmdCuboid (CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help (Player p)
        {
            p.SendMessage("/cuboid [type] <solid/hollow/walls> - create a cuboid of blocks.");
        }

        // Code to run when used by a player
        public override void Use (Player p, string message)
        {
            int number = message.Split(' ').Length;
            if (number > 2) { Help(p); return; }
            // example, /cuboid op_white walls
            if (number == 2)
            {
                int pos = message.IndexOf(' ');
                string t = message.Substring(0, pos).ToLower();
                string s = message.Substring(pos + 1).ToLower();
                byte type = Block.Byte(t);
                if (type == 255) { p.SendMessage("There is no block \"" + t + "\"."); return; }
                if (Server.advbuilders.Contains(p.name))
                {
                    if (!Block.Placable(type) && !Block.AdvPlacable(type)) { p.SendMessage("Your not allowed to place that."); return; }
                }
                SolidType solid;
                if (s == "solid") { solid = SolidType.solid; }
                else if (s == "hollow") { solid = SolidType.hollow; }
                else if (s == "walls") { solid = SolidType.walls; }
                else { Help(p); return; }
                CatchPos cpos = new CatchPos(); cpos.solid = solid; cpos.type = type;
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            }
            // Example, /cuboid op_white
            // Example, /cuboid walls
            else if (message != "")
            {
                SolidType solid = SolidType.solid;
                message = message.ToLower();
                byte type; unchecked { type = (byte) -1; }
                if (message == "solid") { solid = SolidType.solid; }
                else if (message == "hollow") { solid = SolidType.hollow; }
                else if (message == "walls") { solid = SolidType.walls; }
                else
                {
                    byte t = Block.Byte(message);
                    if (t == 255) { p.SendMessage("There is no block \"" + message + "\"."); return; }
                    if (p.Rank == GroupEnum.AdvBuilder)
                    {
                        if (!Block.Placable(t) && !Block.AdvPlacable(t)) { p.SendMessage("Your not allowed to place that."); return; }
                    }
                    type = t;
                } CatchPos cpos = new CatchPos(); cpos.solid = solid; cpos.type = type;
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            }
            // Example, /cuboid
            // Take currently held block
            else
            {
                CatchPos cpos = new CatchPos(); cpos.solid = SolidType.solid; unchecked { cpos.type = (byte) -1; }
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            }
            p.SendMessage("Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        // Grab First Pos
        // First block change (defining first corner)
        // Figure out what kind of cuboid
        // Process Changes to the world
        public void Blockchange1 (Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos bp = (CatchPos) p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }

        // Second block change (defining second corner)
        public void Blockchange2 (Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos cpos = (CatchPos) p.blockchangeObject;
            unchecked { if (cpos.type != (byte) -1) { type = cpos.type; } }
            List<Pos> buffer = new List<Pos>();

            // Solid is default
            switch (cpos.solid)
            {
                case SolidType.solid:

                    // redundant?
                    if (Math.Abs(cpos.x - x) * Math.Abs(cpos.y - y) * Math.Abs(cpos.z - z) > p.group.CuboidLimit && p.group.CuboidLimit != 0)
                    {
                        p.SendMessage("You're trying to place " + buffer.Count.ToString() + " blocks.");
                        p.SendMessage("Your block limit is " + p.group.CuboidLimit.ToString() + " blocks. Build in stages.");
                        return;
                    }
                    // end redundant?

                    buffer.Capacity = Math.Abs(cpos.x - x) * Math.Abs(cpos.y - y) * Math.Abs(cpos.z - z);

                    // Nested for loops to cover a solid cube
                    for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                        for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                            for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                                if (p.level.GetTile(xx, yy, zz) != type) { BufferAdd(buffer, xx, yy, zz); }
                    break;

                case SolidType.hollow:
                    // TODO: Work out if theres 800 blocks used before making the buffer

                    // Hollow will build only the outer shell of a cube leaving the center alone
                    for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                        for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                        {
                            if (p.level.GetTile(cpos.x, yy, zz) != type) { BufferAdd(buffer, cpos.x, yy, zz); }
                            if (cpos.x != x) { if (p.level.GetTile(x, yy, zz) != type) { BufferAdd(buffer, x, yy, zz); } }
                        }
                    if (Math.Abs(cpos.x - x) >= 2)
                    {
                        for (ushort xx = (ushort) (Math.Min(cpos.x, x) + 1); xx <= Math.Max(cpos.x, x) - 1; ++xx)
                            for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                            {
                                if (p.level.GetTile(xx, cpos.y, zz) != type) { BufferAdd(buffer, xx, cpos.y, zz); }
                                if (cpos.y != y) { if (p.level.GetTile(xx, y, zz) != type) { BufferAdd(buffer, xx, y, zz); } }
                            }
                        if (Math.Abs(cpos.y - y) >= 2)
                        {
                            for (ushort xx = (ushort) (Math.Min(cpos.x, x) + 1); xx <= Math.Max(cpos.x, x) - 1; ++xx)
                                for (ushort yy = (ushort) (Math.Min(cpos.y, y) + 1); yy <= Math.Max(cpos.y, y) - 1; ++yy)
                                {
                                    if (p.level.GetTile(xx, yy, cpos.z) != type) { BufferAdd(buffer, xx, yy, cpos.z); }
                                    if (cpos.z != z) { if (p.level.GetTile(xx, yy, z) != type) { BufferAdd(buffer, xx, yy, z); } }
                                }
                        }
                    }
                    break;

                // Walls builds only the surrounding vertical borders of a cube
                case SolidType.walls:
                    for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                        for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                        {
                            if (p.level.GetTile(cpos.x, yy, zz) != type) { BufferAdd(buffer, cpos.x, yy, zz); }
                            if (cpos.x != x) { if (p.level.GetTile(x, yy, zz) != type) { BufferAdd(buffer, x, yy, zz); } }
                        }
                    if (Math.Abs(cpos.x - x) >= 2)
                    {
                        if (Math.Abs(cpos.z - z) >= 2)
                        {
                            for (ushort xx = (ushort) (Math.Min(cpos.x, x) + 1); xx <= Math.Max(cpos.x, x) - 1; ++xx)
                                for (ushort yy = (ushort) (Math.Min(cpos.y, y)); yy <= Math.Max(cpos.y, y); ++yy)
                                {
                                    if (p.level.GetTile(xx, yy, cpos.z) != type) { BufferAdd(buffer, xx, yy, cpos.z); }
                                    if (cpos.z != z) { if (p.level.GetTile(xx, yy, z) != type) { BufferAdd(buffer, xx, yy, z); } }
                                }
                        }
                    }
                    break;
            }

            // Why are we running this in the solid case statement as well?
            if (buffer.Count > p.group.CuboidLimit && p.group.CuboidLimit != 0)
            {
                p.SendMessage("You're trying to place " + buffer.Count.ToString() + " blocks.");
                p.SendMessage("Your block limit is " + p.group.CuboidLimit.ToString() + " blocks. Build in stages.");
                return;
            }


            p.SendMessage(buffer.Count.ToString() + " blocks.");


            // This code may not be needed. We already check whether the player can place the block near the top of this class
            if (!Server.advbuilders.Contains(p.name))
            {
                buffer.ForEach(delegate(Pos pos)
                {
                    p.level.Blockchange(p, pos.x, pos.y, pos.z, type);                  //update block for everyone
                });
            }
            else
            {
                buffer.ForEach(delegate(Pos pos)
                {
                    byte bl = p.level.GetTile(pos.x, pos.y, pos.z);
                    if (Block.Placable(bl) || Block.AdvPlacable(bl)) { p.level.Blockchange(p, pos.x, pos.y, pos.z, type); }                  //update block for everyone
                });
            }
            // end possibly not needed code

        }

        void BufferAdd (List<Pos> list, ushort x, ushort y, ushort z)
        {
            Pos pos; pos.x = x; pos.y = y; pos.z = z; list.Add(pos);
        }
    }
}