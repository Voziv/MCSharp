using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MCSharp.World
{
    public sealed partial class Map
    {
        public Physics Physics { get; set; }

        public int physics = 0;

        #region ==Physics==

        public void CalcPhysics ()
        {
            try
            {
                if (Physics > 0)
                {
                    bool extraPhysicsCheck = false;
                    ushort x, y, z;

                    ListCheck.ForEach(delegate(Check C)    //checks though each block to be updated
                    {
                        try
                        {
                            IntToPos(C.b, out x, out y, out z);
                            //Player.GlobalMessage("Found Block! = " + Block.Name(blocks[C.b]) + " : Ctime = " + C.time.ToString());
                            switch (blocks[C.b])
                            {
                                case Block.air:         //Placed air
                                    //initialy checks if block is valid
                                    PhysAir(PosToInt((ushort) (x + 1), y, z));
                                    PhysAir(PosToInt((ushort) (x - 1), y, z));
                                    PhysAir(PosToInt(x, y, (ushort) (z + 1)));
                                    PhysAir(PosToInt(x, y, (ushort) (z - 1)));
                                    PhysAir(PosToInt(x, (ushort) (y + 1), z));  //Check block above the air

                                    //Edge of map water
                                    if (y < depth / 2 && y >= (depth / 2) - 2)
                                    {
                                        if (x == 0 || x == width - 1 || z == 0 || z == height - 1)
                                        {
                                            AddUpdate(C.b, Block.water);
                                        }
                                    }

                                    C.time = 255;
                                    break;


                                case Block.dirt:     //Dirt
                                    if (C.time > 80)   //To grass
                                    {
                                        if (Block.LightPass(GetTile(x, (ushort) (y + 1), z)))
                                        {
                                            AddUpdate(C.b, Block.grass);
                                        }
                                        C.time = 255;
                                    }
                                    else
                                    {
                                        C.time++;
                                    }
                                    break;

                                case Block.water:         //Active_water
                                    //initialy checks if block is valid
                                    if (!PhysSpongeCheck(C.b))
                                    {
                                        if (GetTile(x, (ushort) (y + 1), z) != Block.Zero) { PhysSandCheck(PosToInt(x, (ushort) (y + 1), z)); }
                                        PhysWater(PosToInt((ushort) (x + 1), y, z));
                                        PhysWater(PosToInt((ushort) (x - 1), y, z));
                                        PhysWater(PosToInt(x, y, (ushort) (z + 1)));
                                        PhysWater(PosToInt(x, y, (ushort) (z - 1)));
                                        PhysWater(PosToInt(x, (ushort) (y - 1), z));
                                    }
                                    else
                                    {
                                        AddUpdate(C.b, Block.air);  //was placed near sponge
                                    }
                                    C.time = 255;
                                    break;

                                case Block.lava:         //Active_lava
                                    //initialy checks if block is valid
                                    if (C.time >= 4)
                                    {
                                        PhysLava(PosToInt((ushort) (x + 1), y, z), Block.lava);
                                        PhysLava(PosToInt((ushort) (x - 1), y, z), Block.lava);
                                        PhysLava(PosToInt(x, y, (ushort) (z + 1)), Block.lava);
                                        PhysLava(PosToInt(x, y, (ushort) (z - 1)), Block.lava);
                                        PhysLava(PosToInt(x, (ushort) (y - 1), z), Block.lava);
                                        C.time = 255;
                                    }
                                    else
                                    {
                                        C.time++;
                                    }
                                    break;

                                case Block.sand:    //Sand
                                    if (PhysSand(C.b, Block.sand))
                                    {
                                        PhysAir(PosToInt((ushort) (x + 1), y, z));
                                        PhysAir(PosToInt((ushort) (x - 1), y, z));
                                        PhysAir(PosToInt(x, y, (ushort) (z + 1)));
                                        PhysAir(PosToInt(x, y, (ushort) (z - 1)));
                                        PhysAir(PosToInt(x, (ushort) (y + 1), z));   //Check block above
                                    }
                                    C.time = 255;
                                    break;

                                case Block.gravel:    //Gravel
                                    if (PhysSand(C.b, Block.gravel))
                                    {
                                        PhysAir(PosToInt((ushort) (x + 1), y, z));
                                        PhysAir(PosToInt((ushort) (x - 1), y, z));
                                        PhysAir(PosToInt(x, y, (ushort) (z + 1)));
                                        PhysAir(PosToInt(x, y, (ushort) (z - 1)));
                                        PhysAir(PosToInt(x, (ushort) (y + 1), z));   //Check block above
                                    }
                                    C.time = 255;
                                    break;

                                case Block.sponge:    //SPONGE
                                    PhysSponge(C.b);
                                    C.time = 255;
                                    break;

                                //Adv physics updating anything placed next to water or lava
                                case Block.wood:     //Wood to die in lava
                                case Block.shrub:     //Tree and plants follow
                                case Block.trunk:    //Wood to die in lava
                                case Block.leaf:    //Bushes die in lava
                                case Block.yellowflower:
                                case Block.redflower:
                                case Block.mushroom:
                                case Block.redmushroom:
                                case Block.bookcase:    //bookcase
                                    if (Physics == Physics.Advanced)   //Adv physics kills flowers and mushroos in water/lava
                                    {
                                        PhysAir(PosToInt((ushort) (x + 1), y, z));
                                        PhysAir(PosToInt((ushort) (x - 1), y, z));
                                        PhysAir(PosToInt(x, y, (ushort) (z + 1)));
                                        PhysAir(PosToInt(x, y, (ushort) (z - 1)));
                                        PhysAir(PosToInt(x, (ushort) (y + 1), z));   //Check block above
                                    }
                                    C.time = 255;
                                    break;

                                case Block.staircasestep:
                                    PhysStair(C.b);
                                    C.time = 255;
                                    break;

                                case Block.wood_float:   //wood_float
                                    PhysFloatwood(C.b);
                                    C.time = 255;
                                    break;

                                case Block.lava_fast:         //lava_fast
                                    //initialy checks if block is valid
                                    PhysLava(PosToInt((ushort) (x + 1), y, z), Block.lava_fast);
                                    PhysLava(PosToInt((ushort) (x - 1), y, z), Block.lava_fast);
                                    PhysLava(PosToInt(x, y, (ushort) (z + 1)), Block.lava_fast);
                                    PhysLava(PosToInt(x, y, (ushort) (z - 1)), Block.lava_fast);
                                    PhysLava(PosToInt(x, (ushort) (y - 1), z), Block.lava_fast);
                                    C.time = 255;
                                    break;

                                //Special blocks that are not saved
                                case Block.air_flood:   //air_flood
                                    if (C.time < 1)
                                    {
                                        PhysAirFlood(PosToInt((ushort) (x + 1), y, z), Block.air_flood);
                                        PhysAirFlood(PosToInt((ushort) (x - 1), y, z), Block.air_flood);
                                        PhysAirFlood(PosToInt(x, y, (ushort) (z + 1)), Block.air_flood);
                                        PhysAirFlood(PosToInt(x, y, (ushort) (z - 1)), Block.air_flood);
                                        PhysAirFlood(PosToInt(x, (ushort) (y - 1), z), Block.air_flood);
                                        PhysAirFlood(PosToInt(x, (ushort) (y + 1), z), Block.air_flood);

                                        C.time++;
                                    }
                                    else
                                    {
                                        AddUpdate(C.b, 0);    //Turn back into normal air
                                        C.time = 255;
                                    }
                                    break;

                                case Block.air_flood_layer:   //air_flood_layer
                                    if (C.time < 1)
                                    {
                                        PhysAirFlood(PosToInt((ushort) (x + 1), y, z), Block.air_flood_layer);
                                        PhysAirFlood(PosToInt((ushort) (x - 1), y, z), Block.air_flood_layer);
                                        PhysAirFlood(PosToInt(x, y, (ushort) (z + 1)), Block.air_flood_layer);
                                        PhysAirFlood(PosToInt(x, y, (ushort) (z - 1)), Block.air_flood_layer);

                                        C.time++;
                                    }
                                    else
                                    {
                                        AddUpdate(C.b, 0);    //Turn back into normal air
                                        C.time = 255;
                                    }
                                    break;

                                case Block.air_flood_down:   //air_flood_down
                                    if (C.time < 1)
                                    {
                                        PhysAirFlood(PosToInt((ushort) (x + 1), y, z), Block.air_flood_down);
                                        PhysAirFlood(PosToInt((ushort) (x - 1), y, z), Block.air_flood_down);
                                        PhysAirFlood(PosToInt(x, y, (ushort) (z + 1)), Block.air_flood_down);
                                        PhysAirFlood(PosToInt(x, y, (ushort) (z - 1)), Block.air_flood_down);
                                        PhysAirFlood(PosToInt(x, (ushort) (y - 1), z), Block.air_flood_down);

                                        C.time++;
                                    }
                                    else
                                    {
                                        AddUpdate(C.b, 0);    //Turn back into normal air
                                        C.time = 255;
                                    }
                                    break;

                                case Block.air_flood_up:   //air_flood_up
                                    if (C.time < 1)
                                    {
                                        PhysAirFlood(PosToInt((ushort) (x + 1), y, z), Block.air_flood_up);
                                        PhysAirFlood(PosToInt((ushort) (x - 1), y, z), Block.air_flood_up);
                                        PhysAirFlood(PosToInt(x, y, (ushort) (z + 1)), Block.air_flood_up);
                                        PhysAirFlood(PosToInt(x, y, (ushort) (z - 1)), Block.air_flood_up);
                                        PhysAirFlood(PosToInt(x, (ushort) (y + 1), z), Block.air_flood_up);

                                        C.time++;
                                    }
                                    else
                                    {
                                        AddUpdate(C.b, 0);    //Turn back into normal air
                                        C.time = 255;
                                    }
                                    break;
                                default:    //non special blocks are then ignored, maybe it would be better to avoid getting here and cutting down the list
                                    //C.time = 255;
                                    extraPhysicsCheck = true;
                                    // HANDLE DOORS - Otherwise, this would require ~50 case statements

                                    if (extraPhysicsCheck)
                                    {
                                        for (int i = 0; i < doors.doorBlocks.Length; i++)
                                        {
                                            extraPhysicsCheck = false;
                                            if (blocks[C.b] == doors.doorAirBlocks[i])
                                            {
                                                if (C.time == 0)
                                                {
                                                    PhysReplace(PosToInt((ushort) (x + 1), y, z), doors.doorBlocks[i], doors.doorAirBlocks[i]);
                                                    PhysReplace(PosToInt((ushort) (x - 1), y, z), doors.doorBlocks[i], doors.doorAirBlocks[i]);
                                                    PhysReplace(PosToInt(x, y, (ushort) (z + 1)), doors.doorBlocks[i], doors.doorAirBlocks[i]);
                                                    PhysReplace(PosToInt(x, y, (ushort) (z - 1)), doors.doorBlocks[i], doors.doorAirBlocks[i]);
                                                    PhysReplace(PosToInt(x, (ushort) (y - 1), z), doors.doorBlocks[i], doors.doorAirBlocks[i]);
                                                    PhysReplace(PosToInt(x, (ushort) (y + 1), z), doors.doorBlocks[i], doors.doorAirBlocks[i]);
                                                }
                                                if (C.time < 16)
                                                {
                                                    C.time++;
                                                }
                                                else
                                                {
                                                    AddUpdate(C.b, doors.doorBlocks[i]);    //turn back into door
                                                    C.time = 255;
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    else { C.time = 255; } // Ignore the block, no physics required
                                    break;
                            }


                        }
                        catch
                        {
                            ListCheck.Remove(C);
                            Player.GlobalMessage("Phys check issue");
                        }

                    });

                    ListCheck.RemoveAll(Check => Check.time == 255);  //Remove all that are finished with 255 time

                    ListUpdate.ForEach(delegate(Update C)    //checks though each block to be updated and does so
                    {
                        try
                        {
                            IntToPos(C.b, out x, out y, out z);
                            Blockchange(x, y, z, C.type);
                        }
                        catch
                        {
                            Logger.Log("Phys update issue", LogType.Error);
                        }
                        System.Threading.Thread.Sleep(1); // Attempt to slow down physics just a tad
                    });

                    ListUpdate.Clear();

                }
            }
            catch
            {
                Logger.Log("Map physics error", LogType.Error);
            }
        }

        private void AddCheck (int b)
        {
            try
            {
                if (!ListCheck.Exists(Check => Check.b == b))  //Checks to see if block is already due for a check
                {

                    ListCheck.Add(new Check(b));    //Adds block to list to be updated
                }
            }
            catch
            {
                //s.Log("Warning-PhysicsCheck");
                //ListCheck.Add(new Check(b));    //Lousy back up plan
            }


        }

        private void AddUpdate (int b, int type)
        {
            try
            {
                if (!ListUpdate.Exists(Update => Update.b == b))  //Checks to see if block is already due for an update
                {
                    ListUpdate.Add(new Update(b, (byte) type));
                }
                else
                {
                    if (type == 12 || type == 13)   //Sand and gravel overide
                    {
                        ListUpdate.RemoveAll(Update => Update.b == b);
                        ListUpdate.Add(new Update(b, (byte) type));
                    }
                }
            }
            catch
            {
                //s.Log("Warning-PhysicsUpdate");
                //ListUpdate.Add(new Update(b, (byte)type));    //Lousy back up plan
            }
        }

        public void ClearPhysics ()
        {
            ushort x, y, z;
            ListCheck.ForEach(delegate(Check C)    //checks though each block
            {
                IntToPos(C.b, out x, out y, out z);
                //attempts on shutdown to change blocks back into normal selves that are active, hopefully without needing to send into to clients.
                switch (blocks[C.b])
                {
                    case 200:
                    case 202:
                    case 203:
                        blocks[C.b] = 0;
                        break;
                    case 201:
                        //blocks[C.b] = 111;
                        Blockchange(x, y, z, 111);
                        break;
                    case 205:
                        //blocks[C.b] = 113;
                        Blockchange(x, y, z, 113);
                        break;
                    case 206:
                        //blocks[C.b] = 114;
                        Blockchange(x, y, z, 114);
                        break;
                    case 208: //doorair_white
                        Blockchange(x, y, z, 207);
                        break;
                    case 209:
                        Blockchange(x, y, z, 50);
                        break;
                    case 210:
                        Blockchange(x, y, z, 51);
                        break;
                    case 211:
                        Blockchange(x, y, z, 52);
                        break;
                    case 213:
                        Blockchange(x, y, z, 54);
                        break;
                    case 214:
                        Blockchange(x, y, z, 55);
                        break;
                    case 215:
                        Blockchange(x, y, z, 56);
                        break;
                    case 216:
                        Blockchange(x, y, z, 57);
                        break;
                    case 217:
                        Blockchange(x, y, z, 58);
                        break;
                    case 218:
                        Blockchange(x, y, z, 59);
                        break;
                    case 219:
                        Blockchange(x, y, z, 60);
                        break;
                    case 220:
                        Blockchange(x, y, z, 61);
                        break;
                    case 221:
                        Blockchange(x, y, z, 62);
                        break;
                    case 222:
                        Blockchange(x, y, z, 63);
                        break;
                    case 223:
                        Blockchange(x, y, z, 64);
                        break;
                    case 224:
                        Blockchange(x, y, z, 65);
                        break;
                    case 225:
                        Blockchange(x, y, z, 66);
                        break;
                    case 226:
                        Blockchange(x, y, z, 67);
                        break;
                    case 227:
                        Blockchange(x, y, z, 68);
                        break;
                    case 228:
                        Blockchange(x, y, z, 69);
                        break;
                    case 229:
                        Blockchange(x, y, z, 70);
                        break;
                    case 230:
                        Blockchange(x, y, z, 71);
                        break;
                    case 231:
                        Blockchange(x, y, z, 72);
                        break;
                    case 232:
                        Blockchange(x, y, z, 73);
                        break;
                    case 233:
                        Blockchange(x, y, z, 74);
                        break;
                    case 234:
                        Blockchange(x, y, z, 75);
                        break;
                    case 235:
                        Blockchange(x, y, z, 76);
                        break;
                    case 236:
                        Blockchange(x, y, z, 77);
                        break;
                    case 237:
                        Blockchange(x, y, z, 78);
                        break;
                    case 238:
                        Blockchange(x, y, z, 79);
                        break;
                    case 239:
                        Blockchange(x, y, z, 80);
                        break;
                    case 240:
                        Blockchange(x, y, z, 81);
                        break;
                    case 241:
                        Blockchange(x, y, z, 82);
                        break;
                    case 242:
                        Blockchange(x, y, z, 83);
                        break;
                    case 243:
                        Blockchange(x, y, z, 84);
                        break;
                    case 244:
                        Blockchange(x, y, z, 85);
                        break;
                    case 245:
                        Blockchange(x, y, z, 87);
                        break;
                    case 246:
                        Blockchange(x, y, z, 86);
                        break;
                    case 247:
                        Blockchange(x, y, z, 88);
                        break;
                    case 248:
                        Blockchange(x, y, z, 89);
                        break;
                    case 249:
                        Blockchange(x, y, z, 90);
                        break;
                }
            });

            ListCheck.Clear();
            ListUpdate.Clear();
        }

        private void PhysWater (int b)
        {
            if (b == -1) { return; }
            switch (blocks[b])
            {
                case Block.air:
                    if (!PhysSpongeCheck(b))
                    {
                        AddUpdate(b, 8);
                    }
                    break;

                case Block.lava:    //hit active_lava
                case Block.lava_fast:    //hit lava_fast
                    if (!PhysSpongeCheck(b)) { AddUpdate(b, 1); }
                    break;

                case Block.shrub:
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
                    if (Physics == Physics.Advanced)   //Adv physics kills flowers and mushrooms in water
                    {
                        if (!PhysSpongeCheck(b)) { AddUpdate(b, 0); }
                    }
                    break;

                case Block.sand:    //sand
                case Block.gravel:    //gravel
                case Block.wood_float:   //woodfloat
                    AddCheck(b);
                    break;

                default:
                    break;
            }
        }

        private void PhysLava (int b, byte type)
        {
            if (b == -1) { return; }
            switch (blocks[b])
            {
                case Block.air:
                    AddUpdate(b, type);
                    break;

                case Block.water:    //hit active_water
                    AddUpdate(b, 1);
                    break;

                case Block.sand:    //sand
                    if (Physics == Physics.Advanced)   //Adv physics changes sand to glass next to lava
                    {
                        AddUpdate(b, 20);
                    }
                    else
                    {
                        AddCheck(b);
                    }
                    break;

                case Block.gravel:    //gravel
                    AddCheck(b);
                    break;

                case Block.wood:
                case Block.shrub:
                case Block.trunk:
                case Block.leaf:
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
                    if (Physics == Physics.Advanced)   //Adv physics kills flowers and mushrooms plus wood in lava
                    {
                        AddUpdate(b, 0);
                    }
                    break;

                default:
                    break;
            }
        }

        private void PhysAir (int b)
        {
            if (b == -1) { return; }
            switch (blocks[b])
            {
                case Block.water:     //active water
                case Block.lava:    //active_lava
                case Block.sand:    //sand
                case Block.gravel:    //gravel
                case Block.wood_float:   //wood_float
                case Block.lava_fast:   //lava_fast
                    AddCheck(b);
                    break;

                default:
                    break;
            }
        }

        private bool PhysSand (int b, byte type)   //also does gravel
        {
            if (b == -1) { return false; }
            int tempb = b;
            bool blocked = false;
            bool moved = false;

            do
            {
                tempb = IntOffset(tempb, 0, -1, 0);     //Get block below each loop
                if (GetTile(tempb) != Block.Zero)
                {
                    switch (blocks[tempb])
                    {
                        case Block.air:         //air lava water
                        case Block.water:
                        case Block.lava:
                            moved = true;
                            break;

                        case Block.shrub:
                        case Block.yellowflower:
                        case Block.redflower:
                        case Block.mushroom:
                        case Block.redmushroom:
                            if (Physics == Physics.Advanced)   //Adv physics crushes plants with sand
                            {
                                moved = true;
                            }
                            else
                            {
                                blocked = true;
                            }
                            break;

                        default:
                            blocked = true;
                            break;
                    }
                    if (Physics == Physics.Advanced)
                    {
                        blocked = true;
                    }
                }
                else
                { blocked = true; }
            }
            while (!blocked);

            if (moved)
            {
                AddUpdate(b, 0);
                if (Physics == Physics.Advanced)
                { AddUpdate(tempb, type); }
                else
                { AddUpdate(IntOffset(tempb, 0, 1, 0), type); }
            }

            return moved;
        }

        private void PhysSandCheck (int b)   //also does gravel
        {
            if (b == -1) { return; }
            switch (blocks[b])
            {
                case Block.sand:    //sand
                case Block.gravel:    //gravel
                case Block.wood_float:   //wood_float
                    AddCheck(b);
                    break;

                default:
                    break;
            }
        }

        private void PhysStair (int b)
        {
            int tempb = IntOffset(b, 0, -1, 0);     //Get block below
            if (GetTile(tempb) != Block.Zero)
            {
                if (GetTile(tempb) == Block.staircasestep)
                {
                    AddUpdate(b, 0);
                    AddUpdate(tempb, 43);
                }
            }
        }

        private bool PhysSpongeCheck (int b)         //return true if sponge is near
        {
            int temp = 0;
            for (int x = -2; x <= +2; ++x)
            {
                for (int y = -2; y <= +2; ++y)
                {
                    for (int z = -2; z <= +2; ++z)
                    {
                        temp = IntOffset(b, x, y, z);
                        if (GetTile(temp) != Block.Zero)
                        {
                            if (GetTile(temp) == 19) { return true; }
                        }
                    }
                }
            }
            return false;
        }

        private void PhysSponge (int b)         //turn near water into air when placed
        {
            int temp = 0;
            for (int x = -2; x <= +2; ++x)
            {
                for (int y = -2; y <= +2; ++y)
                {
                    for (int z = -2; z <= +2; ++z)
                    {
                        temp = IntOffset(b, x, y, z);
                        if (GetTile(temp) != Block.Zero)
                        {
                            if (GetTile(temp) == 8) { AddUpdate(temp, 0); }
                        }
                    }
                }
            }

        }

        public void PhysSpongeRemoved (int b)         //Reactivates near water
        {
            //TODO: Calculate only the edge area to activate the physics
            int temp = 0;
            for (int x = -3; x <= +3; ++x)
            {
                for (int y = -3; y <= +3; ++y)
                {
                    for (int z = -3; z <= +3; ++z)
                    {
                        temp = IntOffset(b, x, y, z);
                        if (GetTile(temp) != Block.Zero)
                        {
                            if (GetTile(temp) == 8) { AddCheck(temp); }
                        }
                    }
                }
            }

        }

        private void PhysFloatwood (int b)
        {
            int tempb = IntOffset(b, 0, -1, 0);     //Get block below
            if (GetTile(tempb) != Block.Zero)
            {
                if (GetTile(tempb) == 0)
                {
                    AddUpdate(b, 0);
                    AddUpdate(tempb, 110);
                    return;
                }
            }

            tempb = IntOffset(b, 0, 1, 0);     //Get block above
            if (GetTile(tempb) != Block.Zero)
            {
                if (GetTile(tempb) == 8)
                {
                    AddUpdate(b, 8);
                    AddUpdate(tempb, 110);
                    return;
                }
            }
        }

        private void PhysAirFlood (int b, byte type)
        {
            if (b == -1) { return; }
            switch (blocks[b])
            {
                case 8:
                case 10:
                case 112:   //lava_fast
                    AddUpdate(b, type);
                    break;

                default:
                    break;
            }
        }

        private void PhysReplace (int b, byte typeA, byte typeB)     //replace any typeA with typeB
        {
            if (b == -1) { return; }
            if (blocks[b] == typeA)
            {
                AddUpdate(b, typeB);
            }
        }

        #endregion

    }
}
