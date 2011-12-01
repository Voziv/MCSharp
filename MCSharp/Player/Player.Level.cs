using System;
using System.Collections.Generic;

namespace MCSharp
{
    public sealed partial class Player
    {
        public BlockBuffer copyBuffer = new BlockBuffer();
        public UndoBuffer undoPasteBuffer = new UndoBuffer();

        public delegate void BlockchangeEventHandler (Player p, ushort x, ushort y, ushort z, byte type);
        public event BlockchangeEventHandler Blockchange = null;
        public void ClearBlockchange () { Blockchange = null; }
        public bool HasBlockchange () { return (Blockchange == null); }
        public object blockchangeObject = null;


        public int deletedBlocks = 0;
        public int placedBlocks = 0;

        // Player Coordinates
        public ushort[] pos = new ushort[3] { 0, 0, 0 };
        ushort[] oldpos = new ushort[3] { 0, 0, 0 };
        ushort[] basepos = new ushort[3] { 0, 0, 0 };
        public byte[] rot = new byte[2] { 0, 0 };
        byte[] oldrot = new byte[2] { 0, 0 };

        /// <summary>
        /// 0 - Nothing
        /// 1 - Solid
        /// 2 - Lava
        /// 3 - Water
        /// 4 - Active_lava
        /// 5 - Active_water
        /// 6 OpGlass
        /// </summary>
        public byte BlockAction = 0;

        // Player States
        public bool painting = false;

        // grief/spam detection
        public static int spamBlockCount = 55;
        public static int spamBlockTimer = 5;
        Queue<DateTime> spamBlockLog = new Queue<DateTime>(spamBlockCount);
        public Level level = Server.mainLevel; // Do we need a reference here? Maybe.

        public Block doors = new Block();

        public void ChangeLevel (string strLevel)
        {
            foreach (Level level in Server.levels)
            {
                if (level.name.ToLower() == strLevel.ToLower())
                {
                    ChangeLevel(level);
                    break;
                }
            }
        }

        public void ChangeLevel (Level lvlLevel)
        {
            Loading = true;

            // Clear the players player list
            foreach (Player pl in Player.players)
            {
                if (this.level == pl.level && this != pl)
                {
                    this.SendDie(pl.id);
                }
            }

            this.ClearBlockchange();
            this.BlockAction = 0;
            this.painting = false;
            Player.GlobalDie(this, true);
            this.level = lvlLevel;
            this.SendMotd();
            this.SendMap();
            ushort x = (ushort) ((0.5 + level.spawnx) * 32);
            ushort y = (ushort) ((1 + level.spawny) * 32);
            ushort z = (ushort) ((0.5 + level.spawnz) * 32);
            if (!this.hidden)
            {
                Player.GlobalSpawn(this, x, y, z, level.rotx, level.roty, true);
            }
            else unchecked
                {
                    this.SendPos((byte) -1, x, y, z, level.rotx, level.roty);
                }
            foreach (Player pl in Player.players)
            {
                if (this.level == pl.level && this != pl && !pl.hidden)
                {
                    this.SendSpawn(pl.id, pl.color + pl.name, pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1]);
                }
            }
            this.Loading = false;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Resets any actions that the user has selected and returns to normal block placing mode.
        /// </summary>
        public void ClearActions ()
        {
            ClearBlockchange();
            ClearBindings();
            painting = false;
        }

        /// <summary>
        /// Resets all the bindings to different block types
        /// </summary>
        /// <returns>Returns how many bindings were set</returns>
        public int ClearBindings ()
        {
            int bindCount = 0;
            for (byte i = 0; i < 128; ++i)
            {
                if (Block.Placable(i) && bindings[i] != i)
                {
                    bindings[i] = i;
                    bindCount += 1;
                }
            }
            return bindCount;
        }

        #region == GLOBAL MESSAGES ==
        public static void GlobalBlockchange (Level level, ushort x, ushort y, ushort z, byte type)
        {
            players.ForEach(delegate(Player p) { if (p.level == level) { p.SendBlockchange(x, y, z, type); } });
        }

        public static void GlobalRespawn (Player respawnTarget)
        {
            GlobalSpawn(respawnTarget, respawnTarget.pos[0], respawnTarget.pos[1], respawnTarget.pos[2], respawnTarget.rot[0], respawnTarget.rot[1], false);
        }


        public static void GlobalSpawn (Player from, ushort x, ushort y, ushort z, byte rotx, byte roty, bool self)
        {
            players.ForEach(delegate(Player p)
            {
                if (p.Loading && p != from) { return; }
                if (p.level != from.level || (from.hidden && !self)) { return; }
                if (p != from) { p.SendSpawn(from.id, from.color + from.name, x, y, z, rotx, roty); }
                else if (self)
                {
                    p.pos = new ushort[3] { x, y, z }; p.rot = new byte[2] { rotx, roty };
                    p.oldpos = p.pos; p.basepos = p.pos; p.oldrot = p.rot;
                    unchecked { p.SendSpawn((byte) -1, from.color + from.name, x, y, z, rotx, roty); }
                }
            });
        }
        public static void GlobalDie (Player from, bool self)
        {
            players.ForEach(delegate(Player p)
            {
                if (p.level != from.level || (from.hidden && !self)) { return; }
                if (p != from) { p.SendDie(from.id); }
                else if (self) { unchecked { p.SendDie((byte) -1); } }
            });
        }
        public static void GlobalUpdate () { players.ForEach(delegate(Player p) { if (!p.hidden) { p.UpdatePosition(); } }); }
        #endregion
    }
}
