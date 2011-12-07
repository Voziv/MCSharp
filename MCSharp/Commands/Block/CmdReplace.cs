using System;
using System.Collections.Generic;
using MCSharp.World;
namespace MCSharp
{
    public class CmdReplace : Command
    {
        // Constructor
        public CmdReplace(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/replace [type] [type2] - replace type with type2 inside a selected cuboid");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            int number = message.Split(' ').Length;
            if (number != 2) { Help(p); return; }
            
            int pos = message.IndexOf(' ');
            string t = message.Substring(0, pos).ToLower();
            string t2 = message.Substring(pos + 1).ToLower();
            byte type = Block.Byte(t);
            if (type == 255) { p.SendMessage("There is no block \"" + t + "\"."); return; }
            byte type2 = Block.Byte(t2);
            if (type2 == 255) { p.SendMessage("There is no block \"" + t2 + "\"."); return; }

            CatchPos cpos = new CatchPos(); cpos.type2 = type2; cpos.type = type;
            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            p.SendMessage("Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
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

            for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx) 
            {           
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)    
                {
                    for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)     
                    {
                        if (p.level.GetTile(xx, yy, zz) == type)
                        {
                            BufferAdd(buffer, xx, yy, zz);
                        }
                    }       
                }   
            }
                                


                if (buffer.Count > p.group.CuboidLimit && p.group.CuboidLimit != 0)
                {
                    p.SendMessage("You're trying to replace " + buffer.Count.ToString() + " blocks.");
                    p.SendMessage("Your block limit is " + p.group.CuboidLimit.ToString() + " blocks. Replace in stages.");
                    return;
                }


            p.SendMessage(buffer.Count.ToString() + " blocks.");
            buffer.ForEach(delegate(Pos pos)
            {
                p.level.Blockchange(p, pos.x, pos.y, pos.z, cpos.type2);                  //update block for everyone
            });


        }
        void BufferAdd(List<Pos> list, ushort x, ushort y, ushort z)
        {
            Pos pos; pos.x = x; pos.y = y; pos.z = z; list.Add(pos);
        }
    }
}
