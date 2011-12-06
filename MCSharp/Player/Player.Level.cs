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

        /// <summary>
        /// Sends the player to a new level
        /// </summary>
        /// <param name="strLevel">The name of the level</param>
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

        /// <summary>
        /// Sends the player to a new level
        /// </summary>
        /// <param name="lvlLevel">The level object to send the player to</param>
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

        /// <summary>
        /// Processes deleting a block from the world
        /// </summary>
        /// <param name="targetBlockType">The type of block we will be replacing</param>
        /// <param name="newBlockType">The type of block we're trying to place</param>
        /// <param name="x">The x coordinate of the block</param>
        /// <param name="y">The t coordinate of the block</param>
        /// <param name="z">The z coordinate of the block</param>
        private void deleteBlock (byte targetBlockType, byte newBlockType, ushort x, ushort y, ushort z)
        {
            // Don't bother with buildop here yet, deleted op_material should not turn into op_air.
            // That would be annoying. 

            /*switch (b)
            {
                case Block.door_tree: //Door
                    if (level.physics != 0)
                    { level.Blockchange(this, x, y, z, (byte)(Block.doorair_tree)); }
                    else
                    { SendBlockchange(x, y, z, b); }
                    break;
                case Block.door_obsidian:   //Door2
                    if (level.physics != 0)
                    { level.Blockchange(this, x, y, z, (byte)(Block.doorair_obsidian)); }
                    else
                    { SendBlockchange(x, y, z, b); }
                    break;
                case Block.door_glass:   //Door3
                    if (level.physics != 0)
                    { level.Blockchange(this, x, y, z, (byte)(Block.doorair_glass)); }
                    else
                    { SendBlockchange(x, y, z, b); }
                    break;
                case Block.door_white:
                    if (level.physics != 0)
                    { level.Blockchange(this, x, y, z, (byte)(Block.doorair_white)); }
                    else
                    { SendBlockchange(x, y, z, b); }
                    break;
                case Block.doorair_tree:   //Door_air
                case Block.doorair_obsidian:
                case Block.doorair_glass:
                case Block.doorair_white:
                    break;
                default:
                    level.Blockchange(this, x, y, z, (byte)(Block.air));
                    deletedBlocks += 1;
                    break;
            }*/
            bool doorCheck = false;
            for (int i = 0; i < doors.doorBlocks.Length; i++)
            {
                if (targetBlockType.Equals(doors.doorBlocks[i]))
                {
                    doorCheck = true;
                    if (level.Physics != Physics.Off)
                    { 
                        level.Blockchange(this, x, y, z, (doors.doorAirBlocks[i]));
                    }
                    else
                    {
                        SendBlockchange(x, y, z, targetBlockType);
                    }
                }
                else if (targetBlockType.Equals(doors.doorAirBlocks[i]))
                {
                    doorCheck = true;
                    break;
                }

            }

            // If the block hasn't been changed, add air
            if (!doorCheck) 
            {
                //this.SendMessage("loop failed, regular delete");
                level.Blockchange(this, x, y, z, (byte) (Block.air));
                deletedBlocks += 1;
            }
        }

        /// <summary>
        /// Handles a player adding a block to the world
        /// </summary>
        /// <param name="targetBlockType">The type of block we will be replacing</param>
        /// <param name="newBlockType">The type of block we're trying to place</param>
        /// <param name="x">The x coordinate of the block</param>
        /// <param name="y">The t coordinate of the block</param>
        /// <param name="z">The z coordinate of the block</param>
        private void placeBlock (byte targetBlockType, byte newBlockType, ushort x, ushort y, ushort z)
        {
            switch (BlockAction)
            {
                case 0:     //normal
                    if (level.Physics == Physics.Off)
                    {
                        switch (newBlockType)
                        {
                            case Block.dirt: //instant dirt to grass
                                level.Blockchange(this, x, y, z, (byte) (Block.grass));
                                break;
                            case Block.staircasestep:    //stair handler
                                if (level.GetTile(x, (ushort) (y - 1), z) == Block.staircasestep)
                                {
                                    SendBlockchange(x, y, z, Block.air);    //send the air block back only to the user.
                                    //level.Blockchange(this, x, y, z, (byte)(Block.air));
                                    level.Blockchange(this, x, (ushort) (y - 1), z, (byte) (Block.staircasefull));
                                    break;
                                }
                                //else
                                level.Blockchange(this, x, y, z, newBlockType);
                                break;
                            default:
                                level.Blockchange(this, x, y, z, newBlockType);
                                break;
                        }
                    }
                    else
                    {
                        level.Blockchange(this, x, y, z, newBlockType);
                    }
                    if (!Block.LightPass(newBlockType))
                    {
                        if (level.GetTile(x, (ushort) (y - 1), z) == Block.grass)
                        {
                            level.Blockchange(x, (ushort) (y - 1), z, Block.dirt);
                        }
                    }

                    break;
                case 1:     //solid
                    if (targetBlockType == Block.blackrock) { SendBlockchange(x, y, z, targetBlockType); return; }
                    level.Blockchange(this, x, y, z, (byte) (Block.blackrock));
                    break;
                case 2:     //lava
                    if (targetBlockType == Block.lavastill) { SendBlockchange(x, y, z, targetBlockType); return; }
                    level.Blockchange(this, x, y, z, (byte) (Block.lavastill));
                    break;
                case 3:     //water
                    if (targetBlockType == Block.waterstill) { SendBlockchange(x, y, z, targetBlockType); return; }
                    level.Blockchange(this, x, y, z, (byte) (Block.waterstill));
                    break;
                case 4:     //ACTIVE lava
                    if (targetBlockType == Block.lava) { SendBlockchange(x, y, z, targetBlockType); return; }
                    level.Blockchange(this, x, y, z, (byte) (Block.lava));
                    BlockAction = 0;
                    break;
                case 5:     //ACTIVE water
                    if (targetBlockType == Block.water) { SendBlockchange(x, y, z, targetBlockType); return; }
                    level.Blockchange(this, x, y, z, (byte) (Block.water));
                    BlockAction = 0;
                    break;
                case 6:     //OpGlass
                    if (targetBlockType == Block.op_glass) { SendBlockchange(x, y, z, targetBlockType); return; }
                    level.Blockchange(this, x, y, z, (byte) (Block.op_glass));
                    break;
                case 7:    // sapling >> tree
                    if (newBlockType == Block.shrub)
                    {
                        AddTree(x, y, z);
                    }
                    else
                    {
                        goto case 0;
                    }
                    break;
                case 8:
                    // BuildOP
                    break;
                case 9:
                    // BuildDoor
                    break;
                default:
                    if (BlockAction != 8 && BlockAction != 9) // Yea it's ugly, I know.
                    {
                        Logger.Log(name + " is breaking something", LogType.Debug);   // Should fix annoying log spam with buildop + builddoor
                        BlockAction = 0;
                    }
                    break;
            }
            #region === Buildop + Builddoor ===
            if (BlockAction == 8) //buildop
            {
                switch (newBlockType)
                {
                    case Block.air:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_air));
                        break;
                    case Block.rock:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_stone));
                        break;
                    case Block.dirt:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_dirt));
                        break;
                    case Block.stone:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_cobblestone));
                        break;
                    case Block.wood:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_wood));
                        break;
                    case Block.shrub:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_shrub));
                        break;
                    case Block.sand:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_sand));
                        break;
                    case Block.gravel:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_gravel));
                        break;
                    case Block.goldrock:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_goldrock));
                        break;
                    case Block.ironrock:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_ironrock));
                        break;
                    case Block.coal:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_coal));
                        break;
                    case Block.trunk:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_trunk));
                        break;
                    case Block.leaf:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_leaf));
                        break;
                    case Block.sponge:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_sponge));
                        break;
                    case Block.glass:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_glass));
                        break;
                    case Block.red:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_red));
                        break;
                    case Block.orange:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_orange));
                        break;
                    case Block.yellow:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_yellow));
                        break;
                    case Block.lightgreen:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_lightgreen));
                        break;
                    case Block.green:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_green));
                        break;
                    case Block.aquagreen:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_aquagreen));
                        break;
                    case Block.cyan:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_cyan));
                        break;
                    case Block.lightblue:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_lightblue));
                        break;
                    case Block.blue:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_blue));
                        break;
                    case Block.purple:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_purple));
                        break;
                    case Block.lightpurple:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_lightpurple));
                        break;
                    case Block.pink:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_pink));
                        break;
                    case Block.darkpink:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_darkpink));
                        break;
                    case Block.darkgrey:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_darkgrey));
                        break;
                    case Block.lightgrey:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_lightgrey));
                        break;
                    case Block.white:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_white));
                        break;
                    case Block.yellowflower:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_yellowflower));
                        break;
                    case Block.redflower:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_redflower));
                        break;
                    case Block.mushroom:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_mushroom));
                        break;
                    case Block.redmushroom:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_redmushroom));
                        break;
                    case Block.goldsolid:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_goldsolid));
                        break;
                    case Block.iron:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_iron));
                        break;
                    case Block.staircasefull:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_staircasefull));
                        break;
                    case Block.staircasestep:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_staircasestep));
                        break;
                    case Block.brick:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_brick));
                        break;
                    case Block.tnt:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_tnt));
                        break;
                    case Block.bookcase:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_bookcase));
                        break;
                    case Block.stonevine:
                        level.Blockchange(this, x, y, z, (byte) (Block.op_stonevine));
                        break;
                    case Block.obsidian:
                        level.Blockchange(this, x, y, z, (byte) (Block.opsidian));
                        break;
                    default:
                        break;
                }
            }
            if (BlockAction == 9) //builddoor
            {
                switch (newBlockType)
                {
                    case Block.air:
                        break;
                    case Block.rock:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_stone));
                        break;
                    case Block.dirt:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_dirt));
                        break;
                    case Block.stone:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_cobblestone));
                        break;
                    case Block.wood:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_wood));
                        break;
                    case Block.shrub:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_shrub));
                        break;
                    case Block.sand:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_sand));
                        break;
                    case Block.gravel:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_gravel));
                        break;
                    case Block.goldrock:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_goldrock));
                        break;
                    case Block.ironrock:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_ironrock));
                        break;
                    case Block.coal:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_coal));
                        break;
                    case Block.trunk:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_tree));
                        break;
                    case Block.leaf:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_leaf));
                        break;
                    case Block.sponge:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_sponge));
                        break;
                    case Block.glass:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_glass));
                        break;
                    case Block.red:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_red));
                        break;
                    case Block.orange:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_orange));
                        break;
                    case Block.yellow:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_yellow));
                        break;
                    case Block.lightgreen:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_lightgreen));
                        break;
                    case Block.green:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_green));
                        break;
                    case Block.aquagreen:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_aquagreen));
                        break;
                    case Block.cyan:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_cyan));
                        break;
                    case Block.lightblue:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_lightblue));
                        break;
                    case Block.blue:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_blue));
                        break;
                    case Block.purple:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_purple));
                        break;
                    case Block.lightpurple:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_lightpurple));
                        break;
                    case Block.pink:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_pink));
                        break;
                    case Block.darkpink:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_darkpink));
                        break;
                    case Block.darkgrey:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_darkgrey));
                        break;
                    case Block.lightgrey:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_lightgrey));
                        break;
                    case Block.white:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_white));
                        break;
                    case Block.yellowflower:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_yellowflower));
                        break;
                    case Block.redflower:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_redflower));
                        break;
                    case Block.mushroom:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_mushroom));
                        break;
                    case Block.redmushroom:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_redmushroom));
                        break;
                    case Block.goldsolid:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_goldsolid));
                        break;
                    case Block.iron:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_iron));
                        break;
                    case Block.staircasefull:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_staircasefull));
                        break;
                    case Block.staircasestep:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_staircasestep));
                        break;
                    case Block.brick:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_brick));
                        break;
                    case Block.tnt:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_tnt));
                        break;
                    case Block.bookcase:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_bookcase));
                        break;
                    case Block.stonevine:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_stonevine));
                        break;
                    case Block.obsidian:
                        level.Blockchange(this, x, y, z, (byte) (Block.door_obsidian));
                        break;
                    default:
                        break;
                }
            }
            #endregion === Buildop + Builddoor ===  // (blockaction 8 and 9)

            placedBlocks += 1;
        }

        /// <summary>
        /// Handles adding a tree to the world when using the tree command
        /// </summary>
        /// <param name="x">The x coordinate of the block</param>
        /// <param name="y">The t coordinate of the block</param>
        /// <param name="z">The z coordinate of the block</param>
        void AddTree (ushort x, ushort z, ushort y)
        {
            byte height = (byte) rand.Next(4, 7);
            for (ushort zz = 0; zz < height; zz++)
            {
                if (level.GetTile(x, (ushort) (z + zz), y) == Block.air)   //Not likly to trigger anyway
                {
                    level.Blockchange(x, (ushort) (z + zz), y, Block.trunk);
                }
                else
                {
                    height = (byte) zz;
                }
            }

            short top = (short) (height - 3);

            for (short xx = (short) -top; xx <= top; ++xx)
            {
                for (short yy = (short) -top; yy <= top; ++yy)
                {
                    for (short zz = (short) -top; zz <= top; ++zz)
                    {
                        if (level.GetTile((ushort) (x + xx), (ushort) (z + zz + height), (ushort) (y + yy)) == Block.air)   //Not likly to trigger anyway
                        {
                            //short Dist = (short)(Math.Abs(xx) + Math.Abs(yy) + Math.Abs(zz));
                            short Dist = (short) (Math.Sqrt(xx * xx + yy * yy + zz * zz));
                            if (Dist < top + 1)
                            {
                                if (rand.Next((int) (Dist)) < 2)
                                {
                                    level.Blockchange((ushort) (x + xx), (ushort) (z + zz + height), (ushort) (y + yy), Block.leaf);
                                }
                            }
                        }
                    }
                }
            }
        } // taken from map generator


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
