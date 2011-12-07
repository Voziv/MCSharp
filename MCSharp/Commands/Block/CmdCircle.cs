using System;
using System.Collections.Generic;
using MCSharp.World;
namespace MCSharp
{
    public class CmdCircle : Command
    {
        // Constructor
        public CmdCircle(CommandGroup g, GroupEnum group, string name) : base(g, group, name) { blnConsoleSupported = false; /* By default no console support*/ }

        // Command usage help
        public override void Help(Player p)
        {
            p.SendMessage("/circle [type] [solid/hollow] - Its not a finished command yet.");
        }

        // Code to run when used by a player
        public override void Use(Player p, string message)
        {
            int number = message.Split(' ').Length;
            if (number > 2) { Help(p); return; }
            if (number == 2)
            {
                int pos = message.IndexOf(' ');
                string t = message.Substring(0, pos).ToLower();
                string s = message.Substring(pos + 1).ToLower();
                byte type = Block.Byte(t);
                if (type == 255) { p.SendMessage("There is no block \"" + t + "\"."); return; }
                SolidType solid;
                if (s == "solid") { solid = SolidType.solid; }
                else if (s == "hollow") { solid = SolidType.hollow; }
                else { Help(p); return; }
                CatchPos cpos = new CatchPos(); cpos.solid = solid; cpos.type = type;
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            }
            else if (message != "")
            {
                SolidType solid = SolidType.solid;
                message = message.ToLower();
                byte type; unchecked { type = (byte)-1; }
                if (message == "solid") { solid = SolidType.solid; }
                else if (message == "hollow") { solid = SolidType.hollow; }
                else
                {
                    byte t = Block.Byte(message);
                    if (t == 255) { p.SendMessage("There is no block \"" + t + "\"."); return; }
                    type = t;
                } CatchPos cpos = new CatchPos(); cpos.solid = solid; cpos.type = type;
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            }
            else
            {
                CatchPos cpos = new CatchPos(); cpos.solid = SolidType.solid; unchecked { cpos.type = (byte)-1; }
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            }
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

            if (Math.Abs(x - cpos.x) != Math.Abs(z - cpos.z)) { p.SendMessage("No good, make it a circle."); return; }

            float CenX = Middle(x , cpos.x);
            float CenZ = Middle(z , cpos.z);
            float Rad = Math.Abs(CenX - (float)x);

            if (Rad != (int)Rad) { p.SendMessage("No good, try a diferent radius."); return; }
            switch (cpos.solid)
            {
                case SolidType.solid:
                    //for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                    //    for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                    //        for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    //            if (p.level.GetTile(xx, yy, zz).type != type) { BufferAdd(buffer, xx, yy, zz); }


                    float error = -Rad;
                    float varx = Rad;
                    float varz = 0;

                    p.SendMessage("Radius " + Rad.ToString());

                    while (varx >= varz)
                    {
                        for (ushort yy = (ushort)(Math.Min(cpos.y, y)); yy <= Math.Max(cpos.y, y); ++yy)
                        {
                            BufferAdd(buffer, (ushort)(CenX + varx), yy, (ushort)(CenZ + varz));
                            BufferAdd(buffer, (ushort)(CenX - varx), yy, (ushort)(CenZ + varz));
                            BufferAdd(buffer, (ushort)(CenX + varx), yy, (ushort)(CenZ - varz));
                            BufferAdd(buffer, (ushort)(CenX - varx), yy, (ushort)(CenZ - varz));

                            BufferAdd(buffer, (ushort)(CenX + varz), yy, (ushort)(CenZ + varx));
                            BufferAdd(buffer, (ushort)(CenX - varz), yy, (ushort)(CenZ + varx));
                            BufferAdd(buffer, (ushort)(CenX + varz), yy, (ushort)(CenZ - varx));
                            BufferAdd(buffer, (ushort)(CenX - varz), yy, (ushort)(CenZ - varx));
                        }

                        error += varz;
                        ++varz;
                        error += varz;

                        // The following test may be implemented in assembly language in
                        // most machines by testing the carry flag after adding 'y' to
                        // the value of 'error' in the previous step, since 'error'
                        // nominally has a negative value.
                        if (error >= 0)
                        {
                            --varx;
                            error -= varx;
                            error -= varx;
                        }
                    }


                    break;

                case SolidType.hollow:
                    p.SendMessage("Not implemented yet.");
                    return;
            }

            if (!Server.operators.Contains(p.name))
            {
                if (buffer.Count > 800)
                {
                    p.SendMessage("Too many blocks, build in stages.");
                    return;
                }
            }
            else if (buffer.Count > 20000)
            {
                p.SendMessage("That is a bad idea.");
                return;
            }

            p.SendMessage(buffer.Count.ToString() + " blocks.");
            buffer.ForEach(delegate(Pos pos)
            {
                p.level.Blockchange(p, pos.x, pos.y, pos.z, type);                  //update block for everyone
            });


        }
        float Middle(ushort A, ushort B)
        {
            return (float)B + ((float)A - (float)B) / 2;
        }

        void BufferAdd(List<Pos> list, ushort x, ushort y, ushort z)
        {
            Pos pos; pos.x = x; pos.y = y; pos.z = z; list.Add(pos);
        }
    }
}