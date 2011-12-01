using System;
using System.Collections.Generic;
using System.Text;

namespace MCSharp
{
    public class Block
    {
        public const byte air = (byte)0;
        public const byte rock = (byte)1;
        public const byte grass = (byte)2;
        public const byte dirt = (byte)3;
        public const byte stone = (byte)4;
        public const byte wood = (byte)5;
        public const byte shrub = (byte)6;
        public const byte blackrock = (byte)7;
        public const byte water = (byte)8;
        public const byte waterstill = (byte)9;
        public const byte lava = (byte)10;
        public const byte lavastill = (byte)11;
        public const byte sand = (byte)12;
        public const byte gravel = (byte)13;
        public const byte goldrock = (byte)14;
        public const byte ironrock = (byte)15;
        public const byte coal = (byte)16;
        public const byte trunk = (byte)17;
        public const byte leaf = (byte)18;
        public const byte sponge = (byte)19;
        public const byte glass = (byte)20;
        public const byte red = (byte)21;
        public const byte orange = (byte)22;
        public const byte yellow = (byte)23;
        public const byte lightgreen = (byte)24;
        public const byte green = (byte)25;
        public const byte aquagreen = (byte)26;
        public const byte cyan = (byte)27;
        public const byte lightblue = (byte)28;
        public const byte blue = (byte)29;
        public const byte purple = (byte)30;
        public const byte lightpurple = (byte)31;
        public const byte pink = (byte)32;
        public const byte darkpink = (byte)33;
        public const byte darkgrey = (byte)34;
        public const byte lightgrey = (byte)35;
        public const byte white = (byte)36;
        public const byte yellowflower = (byte)37;
        public const byte redflower = (byte)38;
        public const byte mushroom = (byte)39;
        public const byte redmushroom = (byte)40;
        public const byte goldsolid = (byte)41;
        public const byte iron = (byte)42;
        public const byte staircasefull = (byte)43;
        public const byte staircasestep = (byte)44;
        public const byte brick = (byte)45;
        public const byte tnt = (byte)46;
        public const byte bookcase = (byte)47;
        public const byte stonevine = (byte)48;
        public const byte obsidian = (byte)49;
        public const byte Zero = 0xff;

        //Custom blocks
        public const byte door_brick = (byte)50;
        public const byte door_stone = (byte)51;
        public const byte door_cobblestone = (byte)52;
        //public const byte door_rock = (byte)53;
        public const byte door_grass = (byte)54;
        public const byte door_dirt = (byte)55;
        public const byte door_wood = (byte)56;
        public const byte door_shrub = (byte)57;
        public const byte door_sand = (byte)58;
        public const byte door_gravel = (byte)59;
        public const byte door_goldrock = (byte)60;
        public const byte door_ironrock = (byte)61;
        public const byte door_coal = (byte)62;
        public const byte door_leaf = (byte)63;
        public const byte door_sponge = (byte)64;
        public const byte door_red = (byte)65;
        public const byte door_orange = (byte)66;
        public const byte door_yellow = (byte)67;
        public const byte door_lightgreen = (byte)68;
        public const byte door_green = (byte)69;
        public const byte door_aquagreen = (byte)70;
        public const byte door_cyan = (byte)71;
        public const byte door_lightblue = (byte)72;
        public const byte door_blue = (byte)73;
        public const byte door_purple = (byte)74;
        public const byte door_lightpurple = (byte)75;
        public const byte door_pink = (byte)76;
        public const byte door_darkpink = (byte)77;
        public const byte door_darkgrey = (byte)78;
        public const byte door_lightgrey = (byte)79;
        public const byte door_yellowflower = (byte)80;
        public const byte door_redflower = (byte)81;
        public const byte door_mushroom = (byte)82;
        public const byte door_redmushroom = (byte)83;
        public const byte door_goldsolid = (byte)84;
        public const byte door_iron = (byte)85;
        public const byte door_staircasestep = (byte)87;
        public const byte door_staircasefull = (byte)86;
        public const byte door_tnt = (byte)88;
        public const byte door_bookcase = (byte)89;
        public const byte door_stonevine = (byte)90;

        public const byte op_glass = (byte)100;
        public const byte opsidian = (byte)101;
        public const byte op_brick = (byte)102;
        public const byte op_stone = (byte)103;
        public const byte op_cobblestone = (byte)104;
        public const byte op_air = (byte)105;
        public const byte op_water = (byte)106;

        public const byte wood_float = (byte)110;
        public const byte door_tree = (byte)111;
        public const byte lava_fast = (byte)112;
        public const byte door_obsidian = (byte)113;
        public const byte door_glass = (byte)114;

        // Op materials   (incl: 100 ~ 106)
        public const byte op_white = (byte)115;
        public const byte op_grass = (byte)116;
        public const byte op_dirt = (byte)117;
        public const byte op_wood = (byte)118;
        public const byte op_shrub = (byte)119;
        public const byte op_lava = (byte)120;
        public const byte op_sand = (byte)121;
        public const byte op_gravel = (byte)122;
        public const byte op_goldrock = (byte)123;
        public const byte op_ironrock = (byte)124;
        public const byte op_coal = (byte)125;
        public const byte op_trunk = (byte)126;
        public const byte op_leaf = (byte)127;
        public const byte op_sponge = (byte)128;
        public const byte op_red = (byte)129;
        public const byte op_orange = (byte)130;
        public const byte op_yellow = (byte)131;
        public const byte op_lightgreen = (byte)132;
        public const byte op_green = (byte)133;
        public const byte op_aquagreen = (byte)134;
        public const byte op_cyan = (byte)135;
        public const byte op_lightblue = (byte)136;
        public const byte op_blue = (byte)137;
        public const byte op_purple = (byte)138;
        public const byte op_lightpurple = (byte)139;
        public const byte op_pink = (byte)140;
        public const byte op_darkpink = (byte)141;
        public const byte op_darkgrey = (byte)142;
        public const byte op_lightgrey = (byte)143;
        public const byte op_yellowflower = (byte)144;
        public const byte op_redflower = (byte)145;
        public const byte op_mushroom = (byte)146;
        public const byte op_redmushroom = (byte)147;
        public const byte op_goldsolid = (byte)148;
        public const byte op_iron = (byte)149;
        public const byte op_staircasefull = (byte)150;
        public const byte op_staircasestep = (byte)151;
        public const byte op_tnt = (byte)152;
        public const byte op_bookcase = (byte)153;
        public const byte op_stonevine = (byte)154;

        public const byte air_flood = (byte)200;
        public const byte doorair_tree = (byte)201;
        public const byte air_flood_layer = (byte)202;
        public const byte air_flood_down = (byte)203;
        public const byte air_flood_up = (byte)204;
        public const byte doorair_obsidian = (byte)205;
        public const byte doorair_glass = (byte)206;
        public const byte door_white = (byte)207;
        public const byte doorair_white = (byte)208;

        public const byte doorair_brick = (byte)209;
        public const byte doorair_stone = (byte)210;
        public const byte doorair_cobblestone = (byte)211;
        //public const byte doorair_rock = (byte)212;
        public const byte doorair_grass = (byte)213;
        public const byte doorair_dirt = (byte)214;
        public const byte doorair_wood = (byte)215;
        public const byte doorair_shrub = (byte)216;
        public const byte doorair_sand = (byte)217;
        public const byte doorair_gravel = (byte)218;
        public const byte doorair_goldrock = (byte)219;
        public const byte doorair_ironrock = (byte)220;
        public const byte doorair_coal = (byte)221;
        public const byte doorair_leaf = (byte)222;
        public const byte doorair_sponge = (byte)223;
        public const byte doorair_red = (byte)224;
        public const byte doorair_orange = (byte)225;
        public const byte doorair_yellow = (byte)226;
        public const byte doorair_lightgreen = (byte)227;
        public const byte doorair_green = (byte)228;
        public const byte doorair_aquagreen = (byte)229;
        public const byte doorair_cyan = (byte)230;
        public const byte doorair_lightblue = (byte)231;
        public const byte doorair_blue = (byte)232;
        public const byte doorair_purple = (byte)233;
        public const byte doorair_lightpurple = (byte)234;
        public const byte doorair_pink = (byte)235;
        public const byte doorair_darkpink = (byte)236;
        public const byte doorair_darkgrey = (byte)237;
        public const byte doorair_lightgrey = (byte)238;
        public const byte doorair_yellowflower = (byte)239;
        public const byte doorair_redflower = (byte)240;
        public const byte doorair_mushroom = (byte)241;
        public const byte doorair_redmushroom = (byte)242;
        public const byte doorair_goldsolid = (byte)243;
        public const byte doorair_iron = (byte)244;
        public const byte doorair_staircasestep = (byte)245;
        public const byte doorair_staircasefull = (byte)246;
        public const byte doorair_tnt = (byte)247;
        public const byte doorair_bookcase = (byte)248;
        public const byte doorair_stonevine = (byte)249;





        // The following array is storing door blocks by byte to allow looping over door blocks easily
        public readonly byte[] doorBlocks = {
                                            door_glass,door_obsidian,door_brick,door_stone,door_cobblestone,0,0,door_white,door_grass,door_dirt,
                                            door_wood,door_shrub,0,door_sand,door_gravel,door_goldrock,door_ironrock,door_coal,door_tree,door_leaf,
                                            door_sponge,door_red,door_orange,door_yellow,door_lightgreen,door_green,door_aquagreen,door_cyan,door_lightblue,
                                            door_blue,door_purple,door_lightpurple,door_pink,door_darkpink,door_darkgrey,door_lightgrey,door_yellowflower,
                                            door_redflower,door_mushroom,door_redmushroom,door_goldsolid,door_iron,door_staircasefull,door_staircasestep,
                                            door_tnt,door_bookcase,door_stonevine
                                           };

        public readonly byte[] doorAirBlocks = {
                                            doorair_glass,doorair_obsidian,doorair_brick,doorair_stone,doorair_cobblestone,0,0,doorair_white,doorair_grass,doorair_dirt,
                                            doorair_wood,doorair_shrub,0,doorair_sand,doorair_gravel,doorair_goldrock,doorair_ironrock,doorair_coal,doorair_tree,doorair_leaf,
                                            doorair_sponge,doorair_red,doorair_orange,doorair_yellow,doorair_lightgreen,doorair_green,doorair_aquagreen,doorair_cyan,doorair_lightblue,
                                            doorair_blue,doorair_purple,doorair_lightpurple,doorair_pink,doorair_darkpink,doorair_darkgrey,doorair_lightgrey,doorair_yellowflower,
                                            doorair_redflower,doorair_mushroom,doorair_redmushroom,doorair_goldsolid,doorair_iron,doorair_staircasefull,doorair_staircasestep,
                                            doorair_tnt,doorair_bookcase,doorair_stonevine
                                           };
        /*public readonly byte[] doorBlocks = { door_tree, door_obsidian, door_glass, door_white, 
                                              door_brick, door_stone, door_cobblestone, //door_rock,
                                              door_grass,door_dirt,door_wood,door_shrub,door_sand, 
                                              door_gravel,door_goldrock,door_ironrock,door_coal,door_leaf,
                                              door_sponge,door_red,door_orange,door_yellow,door_lightgreen,
                                              door_green,door_aquagreen,door_cyan,door_lightblue,door_blue,
                                              door_purple,door_lightpurple,door_pink,door_darkpink,door_darkgrey,
                                              door_lightgrey,door_yellowflower,door_redflower,door_mushroom,door_redmushroom,
                                              door_goldsolid,door_iron,door_staircasefull,door_staircasestep,door_tnt,
                                              door_bookcase,door_stonevine
                                            };*/
        // The following array is storing doorair blocks
        /*public readonly byte[] doorAirBlocks = { doorair_tree, doorair_obsidian, doorair_glass, doorair_white,
                                                 doorair_brick, doorair_stone, doorair_cobblestone, //doorair_rock,
                                                 doorair_grass,doorair_dirt,doorair_wood,doorair_shrub,doorair_sand, 
                                                 doorair_gravel,doorair_goldrock,doorair_ironrock,doorair_coal,doorair_leaf,
                                                 doorair_sponge,doorair_red,doorair_orange,doorair_yellow,doorair_lightgreen,
                                                 doorair_green,doorair_aquagreen,doorair_cyan,doorair_lightblue,doorair_blue,
                                                 doorair_purple,doorair_lightpurple,doorair_pink,doorair_darkpink,doorair_darkgrey,
                                                 doorair_lightgrey,doorair_yellowflower,doorair_redflower,doorair_mushroom,doorair_redmushroom,
                                                 doorair_goldsolid,doorair_iron,doorair_staircasefull,doorair_staircasestep,doorair_tnt,
                                                 doorair_bookcase,doorair_stonevine
                                                };*/

        // The following array is storing op blocks for use with /viewop
        public readonly byte[] opBlocks = { op_glass,opsidian,op_brick,op_stone,op_cobblestone,op_air,op_water,op_white,op_grass,op_dirt,
                                            op_wood,op_shrub,op_lava,op_sand,op_gravel,op_goldrock,op_ironrock,op_coal,op_trunk,op_leaf,
                                            op_sponge,op_red,op_orange,op_yellow,op_lightgreen,op_green,op_aquagreen,op_cyan,op_lightblue,
                                            op_blue,op_purple,op_lightpurple,op_pink,op_darkpink,op_darkgrey,op_lightgrey,op_yellowflower,
                                            op_redflower,op_mushroom,op_redmushroom,op_goldsolid,op_iron,op_staircasefull,op_staircasestep,
                                            op_tnt,op_bookcase,op_stonevine
                                          };
        // This array must be kept in sync with opBlocks. lock, unlock, unopview, and opview all rely on this.
        // In order to save processing power, if the user enters  "air" or "op_air" it automatically uses an index from opBlocks 
        // from a for loop to find the proper block in that arrays. The index is then added to an ignore list 
        // and the block is not processed. regBlocks is used to account for "air" entry in this case.

        // If the arrays are not in order, the user will enter "air" and it will add op_pink to the ignore list for example.
        public readonly byte[] regBlocks = { glass,obsidian,brick,rock,stone,air,water,white,grass,dirt,
                                            wood,shrub,lava,sand,gravel,goldrock,ironrock,coal,trunk,leaf,
                                            sponge,red,orange,yellow,lightgreen,green,aquagreen,cyan,lightblue,
                                            blue,purple,lightpurple,pink,darkpink,darkgrey,lightgrey,yellowflower,
                                            redflower,mushroom,redmushroom,goldsolid,iron,staircasefull,staircasestep,
                                            tnt,bookcase,stonevine
                                          };

        // convertDoor also converts from door depending on the input. If a door is input, a reg_block is returned.
        // If a reg_block is entered, a door_block is returned.
        public static byte convertDoor(byte block)
        {
            switch (block)
            {
                case trunk: return door_tree;
                case door_tree: return trunk;

                case obsidian: return door_obsidian;
                case door_obsidian: return obsidian;

                case door_glass: return glass;
                case glass: return door_glass;

                case white: return door_white;
                case door_white: return white;

                case brick: return door_brick;
                case door_brick: return brick;

                case rock: return door_stone;
                case door_stone: return rock;

                case stone: return door_cobblestone;
                case door_cobblestone: return stone;

                //case rock: return door_rock;

                case grass: return door_grass;
                case door_grass: return grass;
                
                case dirt: return door_dirt;
                case door_dirt: return dirt;                

                case wood: return door_wood;
                case door_wood: return wood;

                case shrub: return door_shrub;
                case door_shrub: return shrub;
                
                case sand: return door_sand;
                case door_sand: return sand;
                
                case gravel: return door_gravel;
                case door_gravel: return gravel;
                
                case goldrock: return door_goldrock;
                case door_goldrock: return goldrock;

                case ironrock: return door_ironrock;
                case door_ironrock: return ironrock;
                
                case coal: return door_coal;
                case door_coal: return coal;
                
                case leaf: return door_leaf;
                case door_leaf: return leaf;
                
                case sponge: return door_sponge;
                case door_sponge: return sponge;
                
                case red: return door_red;
                case door_red: return red;

                case orange: return door_orange;
                case door_orange: return orange;
                
                case yellow: return door_yellow;
                case door_yellow: return yellow;

                case lightgreen: return door_lightgreen;
                case door_lightgreen: return lightgreen;
                
                case green: return door_green;
                case door_green: return green;

                case aquagreen: return door_aquagreen;
                case door_aquagreen: return aquagreen;
                
                case cyan: return door_cyan;
                case door_cyan: return cyan;
                
                case lightblue: return door_lightblue;
                case door_lightblue: return lightblue;
                
                case blue: return door_blue;
                case door_blue: return blue;
                
                case purple: return door_purple;
                case door_purple: return purple;
                
                case lightpurple: return door_lightpurple;
                case door_lightpurple: return lightpurple;

                case pink: return door_pink;
                case door_pink: return pink;

                case darkpink: return door_darkpink;
                case door_darkpink: return darkpink;

                case darkgrey: return door_darkgrey;
                case door_darkgrey: return darkgrey;

                case lightgrey: return door_lightgrey;
                case door_lightgrey: return lightgrey;
                
                case yellowflower: return door_yellowflower;
                case door_yellowflower: return yellowflower;
                
                case redflower: return door_redflower;
                case door_redflower: return redflower;

                case mushroom: return door_mushroom;
                case door_mushroom: return mushroom;

                case redmushroom: return door_redmushroom;
                case door_redmushroom: return redmushroom;
                
                case goldsolid: return door_goldsolid;
                case door_goldsolid: return goldsolid;
                
                case iron: return door_iron;
                case door_iron: return iron;
                
                case staircasefull: return door_staircasefull;
                case door_staircasefull: return staircasefull;
                
                case staircasestep: return door_staircasestep;
                case door_staircasestep: return staircasestep;
                
                case tnt: return door_tnt;
                case door_tnt: return tnt;

                case bookcase: return door_bookcase;
                case door_bookcase: return bookcase;

                case stonevine: return door_stonevine;
                case door_stonevine: return stonevine;

                default: return block;      // if the block is not "doorifiable" keep it as is.
            }
        }

        public static bool Placable(byte type)
        {
            switch (type)
            {
                case Block.air:
                case Block.grass:
                case Block.blackrock:
                case Block.water:
                case Block.waterstill:
                case Block.lava:
                case Block.lavastill:
                    return false;
            }

            if (type > 49) { return false; }
            return true;
        }

        public static int convertOp(byte block) // return opposite of op or reg mats
        {
            switch (block)
            {
                case Block.air: return Block.op_air;
                case Block.op_air: return Block.air;

                case Block.rock: return Block.op_stone;
                case Block.op_stone: return Block.rock;

                case Block.grass: return Block.op_grass;
                case Block.op_grass: return Block.grass;
                    
                case Block.dirt: return Block.op_dirt;
                case Block.op_dirt: return Block.dirt;

                case Block.stone: return Block.op_cobblestone;
                case Block.op_cobblestone: return Block.stone;

                case Block.wood: return Block.op_wood;
                case Block.op_wood: return Block.wood;

                case Block.shrub: return Block.op_shrub;
                case Block.op_shrub: return Block.shrub;

                case Block.water: return Block.op_water;
                case Block.op_water: return Block.waterstill;

                case Block.waterstill: return Block.op_water;

                case Block.lava: return Block.op_lava;
                case Block.op_lava: return Block.lavastill;

                case Block.lavastill: return Block.op_lava;
                    
                case Block.sand: return Block.op_sand;
                case Block.op_sand: return Block.sand;

                case Block.gravel: return Block.op_gravel;
                case Block.op_gravel: return Block.gravel;
                
                case Block.goldrock: return Block.op_goldrock;
                case Block.op_goldrock: return Block.goldrock;

                case Block.ironrock: return Block.op_ironrock;
                case Block.op_ironrock: return Block.ironrock;

                case Block.coal: return Block.op_coal;
                case Block.op_coal: return Block.coal;

                case Block.trunk: return Block.op_trunk;
                case Block.op_trunk: return Block.trunk;

                case Block.leaf: return Block.op_leaf;
                case Block.op_leaf: return Block.leaf;

                case Block.sponge: return Block.op_sponge;
                case Block.op_sponge: return Block.sponge;

                case Block.glass: return Block.op_glass;
                case Block.op_glass: return Block.glass;

                case Block.red: return Block.op_red;
                case Block.op_red: return Block.red;

                case Block.orange: return Block.op_orange;
                case Block.op_orange: return Block.orange;

                case Block.yellow: return Block.op_yellow;
                case Block.op_yellow: return Block.yellow;
                
                case Block.lightgreen: return Block.op_lightgreen;
                case Block.op_lightgreen: return Block.lightgreen;

                case Block.green: return Block.op_green;
                case Block.op_green: return Block.green;

                case Block.aquagreen: return Block.op_aquagreen;
                case Block.op_aquagreen: return Block.aquagreen;

                case Block.cyan: return Block.op_cyan;
                case Block.op_cyan: return Block.cyan;

                case Block.lightblue: return Block.op_lightblue;
                case Block.op_lightblue: return Block.lightblue;

                case Block.blue: return Block.op_blue;
                case Block.op_blue: return Block.blue;

                case Block.purple: return Block.op_purple;
                case Block.op_purple: return Block.purple;

                case Block.lightpurple: return Block.op_lightpurple;
                case Block.op_lightpurple: return Block.lightpurple;

                case Block.pink: return Block.op_pink;
                case Block.op_pink: return Block.pink;

                case Block.darkpink: return Block.op_darkpink;
                case Block.op_darkpink: return Block.darkpink;

                case Block.darkgrey: return Block.op_darkgrey;
                case Block.op_darkgrey: return Block.darkgrey;

                case Block.lightgrey: return Block.op_lightgrey;
                case Block.op_lightgrey: return Block.lightgrey;

                case Block.white: return Block.op_white;
                case Block.op_white: return Block.white;

                case Block.yellowflower: return Block.op_yellowflower;
                case Block.op_yellowflower: return Block.yellowflower;

                case Block.redflower: return Block.op_redflower;
                case Block.op_redflower: return Block.redflower;

                case Block.mushroom: return Block.op_mushroom;
                case Block.op_mushroom: return Block.mushroom;

                case Block.redmushroom: return Block.op_redmushroom;
                case Block.op_redmushroom: return Block.redmushroom;

                case Block.goldsolid: return Block.op_goldsolid;
                case Block.op_goldsolid: return Block.goldsolid;

                case Block.iron: return Block.op_iron;
                case Block.op_iron: return Block.iron;

                case Block.staircasefull: return Block.op_staircasefull;
                case Block.op_staircasefull: return Block.staircasefull;

                case Block.staircasestep: return Block.op_staircasestep;
                case Block.op_staircasestep: return Block.staircasestep;

                case Block.brick: return Block.op_brick;
                case Block.op_brick: return Block.brick;

                case Block.tnt: return Block.op_tnt;
                case Block.op_tnt: return Block.tnt;

                case Block.bookcase: return Block.op_bookcase;
                case Block.op_bookcase: return Block.bookcase;

                case Block.stonevine: return Block.op_stonevine;
                case Block.op_stonevine: return Block.stonevine;

                case Block.obsidian: return Block.opsidian;
                case Block.opsidian: return Block.obsidian;

                case Block.lava_fast: return Block.op_lava;

                default:
                    return block;       // Example:  Don't convert door_tree into op_tree, leave it as is
            }
        }

        public static bool AdvPlacable(byte type)   //returns true if ADV Builder is allowed to use these unplacable blocks
        {
            switch (type)
            {
                case Block.air:
                case Block.grass:
                case Block.waterstill:
                case Block.water:
                case Block.lavastill:
                case Block.door_tree:
                case Block.door_obsidian:
                case Block.door_glass:
                case Block.door_white:
                case Block.door_brick:
                case Block.door_stone:
                case Block.door_cobblestone:
                //case Block.door_rock:
                case Block.door_grass:
                case Block.door_dirt:
                case Block.door_wood:
                case Block.door_shrub:
                case Block.door_sand:
                case Block.door_gravel:
                case Block.door_goldrock:
                case Block.door_ironrock:
                case Block.door_coal:
                case Block.door_leaf:
                case Block.door_sponge:
                case Block.door_red:
                case Block.door_orange:
                case Block.door_yellow:
                case Block.door_lightgreen:
                case Block.door_green:
                case Block.door_aquagreen:
                case Block.door_cyan:
                case Block.door_lightblue:
                case Block.door_blue:
                case Block.door_purple:
                case Block.door_lightpurple:
                case Block.door_pink:
                case Block.door_darkpink:
                case Block.door_darkgrey:
                case Block.door_lightgrey:
                case Block.door_yellowflower:
                case Block.door_redflower:
                case Block.door_mushroom:
                case Block.door_redmushroom:
                case Block.door_goldsolid:
                case Block.door_iron:
                case Block.door_staircasefull:
                case Block.door_staircasestep:
                case Block.door_tnt:
                case Block.door_bookcase:
                case Block.door_stonevine:

                    return true;
            }
            return false;
        }

        public static bool LightPass(byte type)
        {
            switch (type)
            {
                case Block.air:
                case Block.glass:
                case Block.op_air:
                case Block.op_glass:
                case Block.leaf:
                case Block.op_leaf:
                case Block.door_leaf:
                case Block.redflower:
                case Block.op_redflower:
                case Block.door_redflower:
                case Block.yellowflower:
                case Block.door_yellowflower:
                case Block.op_yellowflower:
                case Block.mushroom:
                case Block.op_mushroom:
                case Block.door_mushroom:
                case Block.redmushroom:
                case Block.op_redmushroom:
                case Block.door_redmushroom:
                case Block.shrub:
                case Block.door_shrub:
                case Block.op_shrub:
                case Block.door_glass:
                    return true;

                default:
                    return false;
            }
        }

        public static bool Physics(byte type)   //returns false if placing block cant actually cause any physics to happen
        {
            switch (type)
            {
                case Block.rock:
                case Block.stone:
                case Block.blackrock:
                case Block.waterstill:
                case Block.lavastill:
                case Block.goldrock:
                case Block.ironrock:
                case Block.coal:
                case Block.red:
                case Block.orange:
                case Block.yellow:
                case Block.lightgreen:
                case Block.green:
                case Block.aquagreen:
                case Block.cyan:
                case Block.lightblue:
                case Block.blue:
                case Block.purple:
                case Block.lightpurple:
                case Block.pink:
                case Block.darkpink:
                case Block.darkgrey:
                case Block.lightgrey:
                case Block.white:
                case Block.goldsolid:
                case Block.iron:
                case Block.staircasefull:
                case Block.brick:
                case Block.tnt:
                case Block.stonevine:
                case Block.obsidian:

                case Block.op_glass:
                case Block.opsidian:
                case Block.op_brick:
                case Block.op_stone:
                case Block.op_cobblestone:
                case Block.op_air:
                case Block.op_water:
                case Block.op_white:
                case Block.op_grass:
                case Block.op_dirt:
                case Block.op_wood:
                case Block.op_shrub:
                case Block.op_lava:
                case Block.op_sand:
                case Block.op_gravel:
                case Block.op_goldrock:
                case Block.op_ironrock:
                case Block.op_coal:
                case Block.op_trunk:
                case Block.op_leaf:
                case Block.op_sponge:
                case Block.op_red:
                case Block.op_orange:
                case Block.op_yellow:
                case Block.op_lightgreen:
                case Block.op_green:
                case Block.op_aquagreen:
                case Block.op_cyan:
                case Block.op_lightblue:
                case Block.op_blue:
                case Block.op_purple:
                case Block.op_lightpurple:
                case Block.op_pink:
                case Block.op_darkpink:
                case Block.op_darkgrey:
                case Block.op_lightgrey:
                case Block.op_yellowflower:
                case Block.op_redflower:
                case Block.op_mushroom:
                case Block.op_redmushroom:
                case Block.op_goldsolid:
                case Block.op_iron:
                case Block.op_staircasefull:
                case Block.op_staircasestep:
                case Block.op_tnt:
                case Block.op_bookcase:
                case Block.op_stonevine:

                case Block.door_tree:
                case Block.door_obsidian:
                case Block.door_glass:
                case Block.door_white:
                case Block.door_brick:
                case Block.door_stone:
                case Block.door_cobblestone:
                //case Block.door_rock:
                case Block.door_grass:
                case Block.door_dirt:
                case Block.door_wood:
                case Block.door_shrub:
                case Block.door_sand:
                case Block.door_gravel:
                case Block.door_goldrock:
                case Block.door_ironrock:
                case Block.door_coal:
                case Block.door_leaf:
                case Block.door_sponge:
                case Block.door_red:
                case Block.door_orange:
                case Block.door_yellow:
                case Block.door_lightgreen:
                case Block.door_green:
                case Block.door_aquagreen:
                case Block.door_cyan:
                case Block.door_lightblue:
                case Block.door_blue:
                case Block.door_purple:
                case Block.door_lightpurple:
                case Block.door_pink:
                case Block.door_darkpink:
                case Block.door_darkgrey:
                case Block.door_lightgrey:
                case Block.door_yellowflower:
                case Block.door_redflower:
                case Block.door_mushroom:
                case Block.door_redmushroom:
                case Block.door_goldsolid:
                case Block.door_iron:
                case Block.door_staircasefull:
                case Block.door_staircasestep:
                case Block.door_tnt:
                case Block.door_bookcase:
                case Block.door_stonevine:

                    return false;

                default:
                    return true;
            }
        }

        public static string Name(byte type)
        {
            switch (type)
            {
                case 0: return "air";
                case 1: return "stone";
                case 2: return "grass";
                case 3: return "dirt";
                case 4: return "cobblestone";
                case 5: return "wood";
                case 6: return "plant";
                case 7: return "adminium";
                case 8: return "active_water";
                case 9: return "water";
                case 10: return "active_lava";
                case 11: return "lava";
                case 12: return "sand";
                case 13: return "gravel";
                case 14: return "gold_ore";
                case 15: return "iron_ore";
                case 16: return "coal";
                case 17: return "tree";
                case 18: return "leaves";
                case 19: return "sponge";
                case 20: return "glass";
                case 21: return "red";
                case 22: return "orange";
                case 23: return "yellow";
                case 24: return "greenyellow";
                case 25: return "green";
                case 26: return "springgreen";
                case 27: return "cyan";
                case 28: return "blue";
                case 29: return "blueviolet";
                case 30: return "indigo";
                case 31: return "purple";
                case 32: return "magenta";
                case 33: return "pink";
                case 34: return "black";
                case 35: return "gray";
                case 36: return "white";
                case 37: return "yellow_flower";
                case 38: return "red_flower";
                case 39: return "brown_shroom";
                case 40: return "red_shroom";
                case 41: return "gold";
                case 42: return "iron";
                case 43: return "double_stair";
                case 44: return "stair";
                case 45: return "brick";
                case 46: return "tnt";
                case 47: return "bookcase";
                case 48: return "mossy_cobblestone";
                case 49: return "obsidian";
                case 50: return "door_brick";
                case 51: return "door_stone";
                case 52: return "door_cobblestone";
                case 54: return "door_grass";
                case 55: return "door_dirt";
                case 56: return "door_wood";
                case 57: return "door_plant";
                case 58: return "door_sand";
                case 59: return "door_gravel";
                case 60: return "door_gold_ore";
                case 61: return "door_iron_ore";
                case 62: return "door_coal";
                case 63: return "door_leaves";
                case 64: return "door_sponge";
                case 65: return "door_red";
                case 66: return "door_orange";
                case 67: return "door_yellow";
                case 68: return "door_greenyellow";
                case 69: return "door_green";
                case 70: return "door_springgreen";
                case 71: return "door_cyan";
                case 72: return "door_blue";
                case 73: return "door_blueviolet";
                case 74: return "door_indigo";
                case 75: return "door_purple";
                case 76: return "door_magenta";
                case 77: return "door_pink";
                case 78: return "door_black";
                case 79: return "door_gray";
                case 80: return "door_yellow_flower";
                case 81: return "door_red_flower";
                case 82: return "door_brown_shroom";
                case 83: return "door_red_shroom";
                case 84: return "door_gold";
                case 85: return "door_iron";
                case 86: return "door_double_stair";
                case 87: return "door_stair";
                case 88: return "door_tnt";
                case 89: return "door_bookcase";
                case 90: return "door_mossy_cobblestone";
                case 100: return "op_glass";
                case 101: return "opsidian";              //TODO Add command or just use bind?
                case 102: return "op_brick";              //TODO
                case 103: return "op_stone";              //TODO
                case 104: return "op_cobblestone";        //TODO
                case 105: return "op_air";                //TODO
                case 106: return "op_water";              //TODO
                case 115: return "op_white";
                case 116: return "op_grass";
                case 117: return "op_dirt";
                case 118: return "op_wood";
                case 119: return "op_plant";
                case 120: return "op_lava";
                case 121: return "op_sand";
                case 122: return "op_gravel";
                case 123: return "op_gold_ore";
                case 124: return "op_iron_ore";
                case 125: return "op_coal";
                case 126: return "op_tree";
                case 127: return "op_leaves";
                case 128: return "op_sponge";
                case 129: return "op_red";
                case 130: return "op_orange";
                case 131: return "op_yellow";
                case 132: return "op_greenyellow";
                case 133: return "op_green";
                case 134: return "op_springgreen";
                case 135: return "op_cyan";
                case 136: return "op_blue";
                case 137: return "op_blueviolet";
                case 138: return "op_indigo";
                case 139: return "op_purple";
                case 140: return "op_magenta";
                case 141: return "op_pink";
                case 142: return "op_black";
                case 143: return "op_gray";
                case 144: return "op_yellow_flower";
                case 145: return "op_red_flower";
                case 146: return "op_brown_shroom";
                case 147: return "op_red_shroom";
                case 148: return "op_gold";
                case 149: return "op_iron";
                case 150: return "op_double_stair";
                case 151: return "op_stair";
                case 152: return "op_tnt";
                case 153: return "op_bookcase";
                case 154: return "op_mossy_cobblestone";


                case 110: return "wood_float";            //TODO
                case 111: return "door_tree";
                case 112: return "lava_fast";
                case 113: return "door_obsidian";
                case 114: return "door_glass";
                case 207: return "door_white";
                case 209: return "doorair_brick";
                case 210: return "doorair_stone";
                case 211: return "doorair_cobblestone";
                case 213: return "doorair_grass";
                case 214: return "doorair_dirt";
                case 215: return "doorair_wood";
                case 216: return "doorair_plant";
                case 217: return "doorair_sand";
                case 218: return "doorair_gravel";
                case 219: return "doorair_gold_ore";
                case 220: return "doorair_iron_ore";
                case 221: return "doorair_coal";
                case 222: return "doorair_leaf";
                case 223: return "doorair_sponge";
                case 224: return "doorair_red";
                case 225: return "doorair_orange";
                case 226: return "doorair_yellow";
                case 227: return "doorair_greenyellow";
                case 228: return "doorair_green";
                case 229: return "doorair_springgreen";
                case 230: return "doorair_cyan";
                case 231: return "doorair_blue";
                case 232: return "doorair_blueviolet";
                case 233: return "doorair_indigo";
                case 234: return "doorair_purple";
                case 235: return "doorair_magenta";
                case 236: return "doorair_pink";
                case 237: return "doorair_black";
                case 238: return "doorair_gray";
                case 239: return "doorair_yellow_flower";
                case 240: return "doorair_red_flower";
                case 241: return "doorair_brown_shroom";
                case 242: return "doorair_red_shroom";
                case 243: return "doorair_gold";
                case 244: return "doorair_iron";
                case 245: return "doorair_double_stair";
                case 246: return "doorair_stair";
                case 247: return "doorair_tnt";
                case 248: return "doorair_bookcase";
                case 249: return "doorair_mossy_cobblestone";


                //Blocks after this are converted before saving
                case 200: return "air_flood";
                case 201: return "doorair_tree";
                case 202: return "air_flood_layer";
                case 203: return "air_flood_down";
                case 204: return "air_flood_up";
                case 205: return "doorair_obsidian";
                case 206: return "doorair_glass";
                case 208: return "doorair_white";

                default: return "unknown";
            }
        }
        public static byte Byte(string type)
        {
            switch (type)
            {
                case "air": return 0;
                case "stone": return 1;
                case "grass": return 2;
                case "dirt": return 3;
                case "cobblestone": return 4;
                case "wood": return 5;
                case "plant": return 6;
                case "adminium": return 7;
                case "active_water": return 8;
                case "water": return 9;
                case "active_lava": return 10;
                case "lava": return 11;
                case "sand": return 12;
                case "gravel": return 13;
                case "gold_ore": return 14;
                case "iron_ore": return 15;
                case "coal": return 16;
                case "tree": return 17;
                case "leaves": return 18;
                case "sponge": return 19;
                case "glass": return 20;
                case "red": return 21;
                case "orange": return 22;
                case "yellow": return 23;
                case "greenyellow": return 24;
                case "green": return 25;
                case "springgreen": return 26;
                case "cyan": return 27;
                case "blue": return 28;
                case "blueviolet": return 29;
                case "indigo": return 30;
                case "purple": return 31;
                case "magenta": return 32;
                case "pink": return 33;
                case "black": return 34;
                case "gray": return 35;
                case "white": return 36;
                case "yellow_flower": return 37;
                case "red_flower": return 38;
                case "brown_shroom": return 39;
                case "red_shroom": return 40;
                case "gold": return 41;
                case "iron": return 42;
                case "double_stair": return 43;
                case "stair": return 44;
                case "brick": return 45;
                case "tnt": return 46;
                case "bookcase": return 47;
                case "mossy_cobblestone": return 48;
                case "obsidian": return 49;

                case "op_glass": return 100;
                case "opsidian": return 101;              //TODO Add command or just use bind?
                case "op_brick": return 102;              //TODO
                case "op_stone": return 103;              //TODO
                case "op_cobblestone": return 104;        //TODO
                case "op_air": return 105;                //TODO
                case "op_water": return 106;              //TODO
                case "op_white": return 115;
                case "op_grass": return 116;
                case "op_dirt": return 117;
                case "op_wood": return 118;
                case "op_plant": return 119;
                case "op_lava": return 120;
                case "op_sand": return 121;
                case "op_gravel": return 122;
                case "op_gold_ore": return 123;
                case "op_iron_ore": return 124;
                case "op_coal": return 125;
                case "op_tree": return 126;
                case "op_leaves": return 127;
                case "op_sponge": return 128;
                case "op_red": return 129;
                case "op_orange": return 130;
                case "op_yellow": return 131;
                case "op_greenyellow": return 132;
                case "op_green": return 133;
                case "op_springgreen": return 134;
                case "op_cyan": return 135;
                case "op_blue": return 136;
                case "op_blueviolet": return 137;
                case "op_indigo": return 138;
                case "op_purple": return 139;
                case "op_magenta": return 140;
                case "op_pink": return 141;
                case "op_black": return 142;
                case "op_gray": return 143;
                case "op_yellow_flower": return 144;
                case "op_red_flower": return 145;
                case "op_brown_shroom": return 146;
                case "op_red_shroom": return 147;
                case "op_gold": return 148;
                case "op_iron": return 149;
                case "op_double_stair": return 150;
                case "op_stair": return 151;
                case "op_tnt": return 152;
                case "op_bookcase": return 153;
                case "op_mossy_cobblestone": return 154;


                case "wood_float": return 110;            //TODO
                case "door_tree": return 111;
                case "lava_fast": return 112;
                case "door_obsidian": return 113;
                case "door_glass": return 114;
                case "door_white": return 207;
                case "door_brick": return 50;
                case "door_stone": return 51;
                case "door_cobblestone": return 52;
                case "door_grass": return 54;
                case "door_dirt": return 55;
                case "door_wood": return 56;
                case "door_plant": return 57;
                case "door_sand": return 58;
                case "door_gravel": return 59;
                case "door_gold_ore": return 60;
                case "door_iron_ore": return 61;
                case "door_coal": return 62;
                case "door_leaves": return 63;
                case "door_sponge": return 64;
                case "door_red": return 65;
                case "door_orange": return 66;
                case "door_yellow": return 67;
                case "door_greenyellow": return 68;
                case "door_green": return 69;
                case "door_springgreen": return 70;
                case "door_cyan": return 71;
                case "door_blue": return 72;
                case "door_blueviolet": return 73;
                case "door_indigo": return 74;
                case "door_purple": return 75;
                case "door_magenta": return 76;
                case "door_pink": return 77;
                case "door_black": return 78;
                case "door_gray": return 79;
                case "door_yellow_flower": return 80;
                case "door_red_flower": return 81;
                case "door_brown_shroom": return 82;
                case "door_red_shroom": return 83;
                case "door_gold": return 84;
                case "door_iron": return 85;
                case "door_double_stair": return 86;
                case "door_stair": return 87;
                case "door_tnt": return 88;
                case "door_bookcase": return 89;
                case "door_mossy_cobblestone": return 90;

                case "doorair_brick": return 209;
                case "doorair_stone": return 210;
                case "doorair_cobblestone": return 211;
                case "doorair_grass": return 212;
                case "doorair_dirt": return 213;
                case "doorair_wood": return 214;
                case "doorair_plant": return 215;
                case "doorair_sand": return 216;
                case "doorair_gravel": return 217;
                case "doorair_gold_ore": return 218;
                case "doorair_iron_ore": return 219;
                case "doorair_coal": return 220;
                case "doorair_leaf": return 221;
                case "doorair_sponge": return 222;
                case "doorair_red": return 223;
                case "doorair_orange": return 224;
                case "doorair_yellow": return 225;
                case "doorair_greenyellow": return 226;
                case "doorair_green": return 227;
                case "doorair_springgreen": return 228;
                case "doorair_cyan": return 229;
                case "doorair_blue": return 230;
                case "doorair_blueviolet": return 231;
                case "doorair_indigo": return 232;
                case "doorair_purple": return 233;
                case "doorair_magenta": return 234;
                case "doorair_pink": return 235;
                case "doorair_black": return 236;
                case "doorair_gray": return 237;
                case "doorair_yellow_flower": return 238;
                case "doorair_red_flower": return 239;
                case "doorair_brown_shroom": return 240;
                case "doorair_red_shroom": return 241;
                case "doorair_gold": return 242;
                case "doorair_iron": return 243;
                case "doorair_double_stair": return 244;
                case "doorair_stair": return 245;
                case "doorair_tnt": return 247;
                case "doorair_bookcase": return 248;
                case "doorair_mossy_cobblestone": return 249;


                //Blocks after this are converted before saving
                case "air_flood": return 200;
                case "doorair_tree": return 201;
                case "air_flood_layer": return 202;
                case "air_flood_down": return 203;
                case "air_flood_up": return 204;
                case "doorair_obsidian": return 205;
                case "doorair_glass": return 206;
                case "doorair_white": return 208;
                default: return 255;
            }
        }

        public static byte Convert(byte b)
        {
            switch (b)
            {
                case 100: return (byte)20; //Op_glass
                case 101: return (byte)49; //Opsidian
                case 102: return (byte)45; //Op_brick
                case 103: return (byte)1; //Op_stone
                case 104: return (byte)4; //Op_cobblestone
                case 105: return (byte)0; //Op_air - Must be cuboided / replaced
                case 106: return (byte)9; //Op_water
                case 115: return (byte)36; //op_white
                case 116: return (byte)2; // op_grass ~~~ various op_materials
                case 117: return (byte)3;
                case 118: return (byte)5;
                case 119: return (byte)6;  // op_shrub / plant
                case 120: return (byte)11; // op_lava  
                case 121: return (byte)12;
                case 122: return (byte)13;
                case 123: return (byte)14;
                case 124: return (byte)15;
                case 125: return (byte)16;
                case 126: return (byte)17;  //op_trunk / tree
                case 127: return (byte)18;
                case 128: return (byte)19;
                case 129: return (byte)21;
                case 130: return (byte)22;
                case 131: return (byte)23;
                case 132: return (byte)24;
                case 133: return (byte)25;
                case 134: return (byte)26;
                case 135: return (byte)27;
                case 136: return (byte)28;
                case 137: return (byte)29;
                case 138: return (byte)30;
                case 139: return (byte)31;
                case 140: return (byte)32;
                case 141: return (byte)33;
                case 142: return (byte)34;
                case 143: return (byte)35;
                case 144: return (byte)37;
                case 145: return (byte)38;
                case 146: return (byte)39;
                case 147: return (byte)40;
                case 148: return (byte)41;
                case 149: return (byte)42;
                case 150: return (byte)43;
                case 151: return (byte)44;
                case 152: return (byte)46;
                case 153: return (byte)47;
                case 154: return (byte)48; // ~~

                case 50: return (byte)45;
                case 51: return (byte)1;
                case 52: return (byte)4;
                case 54: return (byte)2;
                case 55: return (byte)3;
                case 56: return (byte)5;
                case 57: return (byte)6;
                case 58: return (byte)12;
                case 59: return (byte)13;
                case 60: return (byte)14;
                case 61: return (byte)15;
                case 62: return (byte)16;
                case 63: return (byte)18;
                case 64: return (byte)19;
                case 65: return (byte)21;
                case 66: return (byte)22;
                case 67: return (byte)23;
                case 68: return (byte)24;
                case 69: return (byte)25;
                case 70: return (byte)26;
                case 71: return (byte)27;
                case 72: return (byte)28;
                case 73: return (byte)29;
                case 74: return (byte)30;
                case 75: return (byte)31;
                case 76: return (byte)32;
                case 77: return (byte)33;
                case 78: return (byte)34;
                case 79: return (byte)35;
                case 80: return (byte)37;
                case 81: return (byte)38;
                case 82: return (byte)39;
                case 83: return (byte)40;
                case 84: return (byte)41;
                case 85: return (byte)42;
                case 86: return (byte)43;
                case 87: return (byte)44;
                case 88: return (byte)46;
                case 89: return (byte)47;
                case 90: return (byte)48;





                case 110: return (byte)5; //wood_float
                case 111: return (byte)17;//door show by treetype
                case 112: return (byte)10;

                case 113: return (byte)49;//door show by obsidian
                case 114: return (byte)20;//door show by glass
                case 207: return (byte)36; //door_white

                case 200: //air_flood
                case 201: //door_air
                case 202: //air_flood_layer
                case 203: //air_flood_down
                case 204: //air_flood_up
                case 205: //door2_air
                case 206: //door3_air                
                case 208: //doorair_white
                case 209:
                case 210:
                case 211:
                case 212:
                case 213:
                case 214:
                case 215:
                case 216:
                case 217:
                case 218:
                case 219:
                case 220:
                case 221:
                case 222:
                case 223:
                case 224:
                case 225:
                case 226:
                case 227:
                case 228:
                case 229:
                case 230:
                case 231:
                case 232:
                case 233:
                case 234:
                case 235:
                case 236:
                case 237:
                case 238:
                case 239:
                case 240:
                case 241:
                case 242:
                case 243:
                case 244:
                case 245:
                case 246:
                case 247:
                case 248:
                case 249:
                    return (byte)0;
                default: return b;
            }
        }
        public static byte SaveConvert(byte b)
        {
            switch (b)
            {
                case 200:
                case 202:
                case 203:
                case 204:
                    return (byte)0; //air_flood must be converted to air on save to prevent issues
                case 201: return (byte)111; //door_air back into door
                case 205: return (byte)113; //door_air back into door
                case 206: return (byte)114; //door_air back into door
                case 208: return (byte)207; //doorair_white to door_white
                case 209: return (byte)50;
                case 210: return (byte)51;
                case 211: return (byte)52;
                case 213: return (byte)54;
                case 214: return (byte)55;
                case 215: return (byte)56;
                case 216: return (byte)57;
                case 217: return (byte)58;
                case 218: return (byte)59;
                case 219: return (byte)60;
                case 220: return (byte)61;
                case 221: return (byte)62;
                case 222: return (byte)63;
                case 223: return (byte)64;
                case 224: return (byte)65;
                case 225: return (byte)66;
                case 226: return (byte)67;
                case 227: return (byte)68;
                case 228: return (byte)69;
                case 229: return (byte)70;
                case 230: return (byte)71;
                case 231: return (byte)72;
                case 232: return (byte)73;
                case 233: return (byte)74;
                case 234: return (byte)75;
                case 235: return (byte)76;
                case 236: return (byte)77;
                case 237: return (byte)78;
                case 238: return (byte)79;
                case 239: return (byte)80;
                case 240: return (byte)81;
                case 241: return (byte)82;
                case 242: return (byte)83;
                case 243: return (byte)84;
                case 244: return (byte)85;
                case 245: return (byte)87;
                case 246: return (byte)86;
                case 247: return (byte)88;
                case 248: return (byte)89;
                case 249: return (byte)90;

                default: return b;
            }
        }
    }



}