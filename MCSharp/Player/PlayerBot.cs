using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace MCSharp
{
    public sealed class PlayerBot
    {
        public static List<PlayerBot> playerbots = new List<PlayerBot>(64);
        public static byte number { get { return (byte)playerbots.Count; } }

        public string name;
        public byte id;
        public string color;
        public Level level;

        public ushort[] pos = new ushort[3] { 0, 0, 0 };
        ushort[] oldpos = new ushort[3] { 0, 0, 0 };
        ushort[] basepos = new ushort[3] { 0, 0, 0 };
        public byte[] rot = new byte[2] { 0, 0 };
        byte[] oldrot = new byte[2] { 0, 0 };

        #region == constructors ==
        public PlayerBot(string n, Level l)
        {
            Logger.Log("Adding " + n + " as a bot");
            name = n;
            color = "&1";
            id = FreeId();

            level = l;
            ushort x = (ushort)((0.5 + level.spawnx) * 32);
            ushort y = (ushort)((1 + level.spawny) * 32);
            ushort z = (ushort)((0.5 + level.spawnz) * 32);
            pos = new ushort[3] { x, y, z }; rot = new byte[2] { level.rotx, level.roty };
            GlobalSpawn();
        }
        public PlayerBot(string n, Level l, ushort x, ushort y, ushort z, byte rotx, byte roty)
        {
            Logger.Log("Adding " + n + " as a bot");
            name = n;
            color = "&1";
            id = FreeId();

            level = l;
            pos = new ushort[3] { x, y, z }; rot = new byte[2] { rotx, roty };
            GlobalSpawn();
        }
        #endregion
        #region ==Input ==
        public void SetPos(ushort x, ushort y, ushort z, byte rotx, byte roty)
        {
            pos = new ushort[3] { x, y, z };
            rot = new byte[2] { rotx, roty };
        }
        #endregion

        public void GlobalSpawn()
        {
            Player.players.ForEach(delegate(Player p)   //bots dont need to be informed of other bots here
            {
                if (p.level != level) { return; }
                p.SendSpawn(id, color + name, pos[0], pos[1], pos[2], rot[0], rot[1]);
            });
        }

        public void GlobalDie()
        {
            Logger.Log("Removing the bot named " + name);
            Player.players.ForEach(delegate(Player p)
            {
                if (p.level != level) { return; }
                p.SendDie(id);
            });
            playerbots.Remove(this);        //dont know if this is allowed really calling itself to kind of die
        }

        public void Update()
        {
            //pos[0] += 1;
        }

        void UpdatePosition()   //Im going to avoid touching this unless nessesary
        {

            //pingDelayTimer.Stop();

            // Shameless copy from JTE's Server
            byte changed = 0;
            if (oldpos[0] != pos[0] || oldpos[1] != pos[1] || oldpos[2] != pos[2]) { changed |= 1; }
            if (oldrot[0] != rot[0] || oldrot[1] != rot[1]) { changed |= 2; }
            if (Math.Abs(pos[0] - basepos[0]) > 32 || Math.Abs(pos[1] - basepos[1]) > 32 ||
                Math.Abs(pos[2] - basepos[2]) > 32) { changed |= 4; }
            if ((oldpos[0] == pos[0] && oldpos[1] == pos[1] && oldpos[2] == pos[2]) &&
                (basepos[0] != pos[0] || basepos[1] != pos[1] || basepos[2] != pos[2])) { changed |= 4; }
            byte[] buffer = new byte[0]; byte msg = 0;
            if ((changed & 4) != 0)
            {
                msg = 8; buffer = new byte[9]; buffer[0] = id;
                HTNO(pos[0]).CopyTo(buffer, 1);
                HTNO(pos[1]).CopyTo(buffer, 3);
                HTNO(pos[2]).CopyTo(buffer, 5);
                buffer[7] = rot[0]; buffer[8] = rot[1];
            }
            else if (changed == 1)
            {
                msg = 10; buffer = new byte[4]; buffer[0] = id;
                buffer[1] = (byte)(pos[0] - oldpos[0]);
                buffer[2] = (byte)(pos[1] - oldpos[1]);
                buffer[3] = (byte)(pos[2] - oldpos[2]);
            }
            else if (changed == 2)
            {
                msg = 11; buffer = new byte[3]; buffer[0] = id;
                buffer[1] = rot[0]; buffer[2] = rot[1];
            }
            else if (changed == 3)
            {
                msg = 9; buffer = new byte[6]; buffer[0] = id;
                buffer[1] = (byte)(pos[0] - oldpos[0]);
                buffer[2] = (byte)(pos[1] - oldpos[1]);
                buffer[3] = (byte)(pos[2] - oldpos[2]);
                buffer[4] = rot[0]; buffer[5] = rot[1];
            } 
            if (changed != 0) foreach (Player p in Player.players)
            {
                if (p.level == level)
                {
                    p.SendRaw(msg, buffer);
                }
            } 
            oldpos = pos; 
            oldrot = rot;
        }

        #region == Misc ==
        static byte FreeId()
        {
            for (byte i = 64; i < 128; ++i)
            {
                foreach (PlayerBot b in playerbots)
                {
                    if (b.id == i) { goto Next; }
                } return i;
            Next: continue;
            } unchecked { return (byte)-1; }
        }
        public static PlayerBot Find(string name)
        {
            foreach (PlayerBot b in playerbots)
            { if (b.name.ToLower() == name.ToLower()) { return b; } } return null;
        }
        public static bool ValidName(string name)
        {
            string allowedchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890_";
            foreach (char ch in name) { if (allowedchars.IndexOf(ch) == -1) { return false; } } return true;
        }
        #endregion

        #region == True Global ==
        public static void GlobalUpdatePosition()
        {
            playerbots.ForEach(delegate(PlayerBot b) { b.UpdatePosition(); });
        }
        public static void GlobalUpdate()
        {
            while (true)
            {
                Thread.Sleep(100);
                playerbots.ForEach(delegate(PlayerBot b) { b.Update(); });
            }
        }
        #endregion
        #region == Host <> Network ==
        byte[] HTNO(ushort x)       //Is used currently, the rest are not and may not be needed at all
        {
            byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
        }
        ushort NTHO(byte[] x, int offset)
        {
            byte[] y = new byte[2];
            Buffer.BlockCopy(x, offset, y, 0, 2); Array.Reverse(y);
            return BitConverter.ToUInt16(y, 0);
        }
        byte[] HTNO(short x)
        {
            byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
        }
        #endregion
    }
}