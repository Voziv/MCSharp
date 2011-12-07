using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
///WARNING! DO NOT CHANGE THE WAY THE LEVEL IS SAVED/LOADED!
///You MUST make it able to save and load as a new version other wise you will make old levels incompatible!


namespace MCSharp.World
{
    public sealed partial class Map
    {

        /// <summary>
        /// Old public class variables
        /// </summary>
        private static string backupPath { get { return "levels/backups/"; } }
        private string levelPath { get { return "levels/" + name + ".lvl"; } }
        public string name;
        public ushort width; // x
        public ushort depth; // y
        public ushort height; // z


        public ushort spawnx;
        public ushort spawny;
        public ushort spawnz;
        public byte rotx;
        public byte roty;

        //Added by bman for jail system
        public ushort jailedX = 0;
        public ushort jailedY = 0;
        public ushort jailedZ = 0;
        public byte jailedRotX = 0;
        public byte jailedRotY = 0;
        public List<Player> jailedPlayers = new List<Player>();

        
        public int emptyCount = 0;
        public LevelPermission permissionvisit = LevelPermission.Builder;
        public LevelPermission permissionbuild = LevelPermission.Builder; // What ranks can go to this map (excludes banned)

        public byte[] blocks;
        List<Check> ListCheck = new List<Check>();  //A list of blocks that need to be updated
        List<Update> ListUpdate = new List<Update>();  //A list of block to change after calculation

        public Block doors = new Block(); // Is there a better way to do this? need access to doorAirBlocks and doorBlocks from Block

        public bool changed = false;
        private bool backedup = false;
        //public List<Edit> edits = new List<Edit>(1024);
        public Map(string n, ushort x, ushort y, ushort z, string type)
        {
            this.Physics = Physics.Off;


            width = x; 
            depth = y; 
            height = z;

            // Make sure the level isn't too small
            if (width < 16) { width = 16; }
            if (depth < 16) { depth = 16; }
            if (height < 16) { height = 16; }

            name = n;
            blocks = new byte[width * depth * height];


            switch (type)
            {
                case "flat":
                case "pixel":
                    ushort half = (ushort)(depth / 2);
                    for (x = 0; x < width; ++x)
                    {
                        for (z = 0; z < height; ++z)
                        {
                            for (y = 0; y < depth; ++y)
                            {
                                //Block b = new Block();
                                switch (type)
                                {
                                    case "flat":
                                        if (y != half)
                                        {
                                            SetTile(x, y, z, (byte)((y >= half) ? Block.air : Block.dirt));
                                        }
                                        else
                                        {
                                            SetTile(x, y, z, Block.grass);
                                        }
                                        break;

                                    case "pixel":
                                        if (y == 0)
                                        {
                                            SetTile(x, y, z, Block.blackrock);
                                        }
                                        else
                                            if (x == 0 || x == width - 1 || z == 0 || z == height - 1)
                                            {
                                                SetTile(x, y, z, Block.white);
                                            }

                                        break;
                                }
                                //blocks[x + width * z + width * height * y] = b;
                            }
                        }
                    }
                    break;

                case "island":
                case "mountains":
                case "ocean":
                case "forest":
                    Server.MapGen.GenerateMap(this, type);
                    break;

                case "empty":
                default:
                    break;
            }

            spawnx = (ushort)(width / 2);
            spawny = (ushort)(depth * 0.75f);
            spawnz = (ushort)(height / 2);
            rotx = 0; roty = 0;
            Logger.Log("Map initialized.");

            if (type != "empty")
            {
                changed = true;
                Save();
                if (!Directory.Exists("levels/backups/" + name + "/1"))
                {
                    Directory.CreateDirectory("levels/backups/" + name + "/1");
                }
                Backup();
            }
        }

        public bool PlayerCanVisit (Player p)
        {
            bool blnCanVisit = false;
            if (p.group.Permission >= permissionvisit)
            {
                blnCanVisit = true;
            }
            return blnCanVisit;
        }

        public byte GetTile(ushort x, ushort y, ushort z)
        {
            //if (PosToInt(x, y, z) >= blocks.Length) { return null; }
            //Avoid internal overflow
            if (x < 0) { return Block.Zero; }
            if (x >= width) { return Block.Zero; }
            if (y < 0) { return Block.Zero; }
            if (y >= depth) { return Block.Zero; }
            if (z < 0) { return Block.Zero; }
            if (z >= height) { return Block.Zero; }
            return blocks[PosToInt(x, y, z)];
        }

        public byte GetTile(int b)
        {
            ushort x = 0, y = 0, z = 0;
            IntToPos(b, out x, out y, out z);
            return GetTile(x, y, z);
        }

        public void SetTile(ushort x, ushort y, ushort z, byte type)
        {
            blocks[x + width * z + width * height * y] = type;
        }



        public void Blockchange(Player p, ushort x, ushort y, ushort z, byte type)
        {
            try
            {
                if (x >= width || y > depth || z >= height) { p.Kick("Building outside boundaries!"); return; }
                if (y == depth) { return; }
                byte b = GetTile(x, y, z);

                if (Block.Convert(b) != Block.Convert(type))    //Should save bandwidth sending identical looking blocks, like air/op_air changes.
                {
                    Player.GlobalBlockchange(this, x, y, z, type);
                }

                if (b == Block.sponge && (Physics == Physics.Normal || Physics == Physics.Advanced) && type != Block.sponge)
                {
                    PhysSpongeRemoved(PosToInt(x, y, z));
                }

                SetTile(x, y, z, type);               //Updates server level blocks

                if (Physics > 0)
                {
                    if (Block.Physics(type))
                    {
                        AddCheck(PosToInt(x, y, z));
                    }
                }
                changed = true;
            }
            catch (Exception e)
            {
                Logger.Log(p.name + " has triggered a block change error in Map on " + name, LogType.Error);
                Logger.Log(e.Message, LogType.ErrorMessage);
                Player.GlobalMessageOps(p.name + " has triggered a block change error in level.cs on " + name);
                IRCBot.Say(p.name + " has triggered a block change error in level.cs on " + name);
            }
        }

        public void Blockchange(ushort x, ushort y, ushort z, byte type)    //Block change made by physics
        {
            if (y == depth) { return; }
            byte b = GetTile(x, y, z);
            if (Block.Convert(b) != Block.Convert(type))    //Should save bandwidth sending identical looking blocks, like air/op_air changes.
            {
                Player.GlobalBlockchange(this, x, y, z, type);
            }

            if (b == Block.sponge && Physics > 0 && type != Block.sponge)
            {
                PhysSpongeRemoved(PosToInt(x, y, z));
            }

            SetTile(x, y, z, type);               //Updates server level blocks

            if (Physics > 0)
            {
                if (Block.Physics(type))
                {
                    AddCheck(PosToInt(x, y, z));
                }
            }
        }

        public void Save()
        {
            if (changed)
            {
                string path = "levels/" + name + ".lvl";
                try
                {

                    if (!Directory.Exists("levels")) { Directory.CreateDirectory("levels"); }

                    FileStream fs = File.Create(path);
                    GZipStream gs = new GZipStream(fs, CompressionMode.Compress);

                    byte[] header = new byte[16];
                    BitConverter.GetBytes(1874).CopyTo(header, 0);
                    gs.Write(header, 0, 2);

                    // Compile the header
                    BitConverter.GetBytes(width).CopyTo(header, 0);
                    BitConverter.GetBytes(height).CopyTo(header, 2);
                    BitConverter.GetBytes(depth).CopyTo(header, 4);
                    BitConverter.GetBytes(spawnx).CopyTo(header, 6);
                    BitConverter.GetBytes(spawnz).CopyTo(header, 8);
                    BitConverter.GetBytes(spawny).CopyTo(header, 10);
                    header[12] = rotx; header[13] = roty;
                    header[14] = (byte)permissionvisit;
                    header[15] = (byte)permissionbuild;

                    // Write the header
                    gs.Write(header, 0, header.Length);

                    // Get the block information
                    byte[] level = new byte[blocks.Length];
                    for (int i = 0; i < blocks.Length; ++i)
                    {
                        if (blocks[i] < 200)
                        {
                            level[i] = blocks[i];
                        }
                        else
                        {
                            level[i] = Block.SaveConvert(blocks[i]);
                        }
                    }
                    gs.Write(level, 0, level.Length);
                    gs.Close();

                    Logger.Log("SAVED: Map \"" + name + "\". " + Player.players.Count + "/" + Properties.MaxPlayers);

                    // Set changed to false as we've saved
                    // And backup to false because it now needs to be backed up!
                    changed = false;
                    backedup = false;

                    try
                    {
                        File.Copy(path, path + ".backup", true);
                        Logger.Log("And backed up");
                    }
                    catch (Exception e)
                    {
                        Logger.Log("Failed to make backup", LogType.Error);
                        Logger.Log(e.Message, LogType.ErrorMessage);
                    }

                }
                catch (Exception e)
                {
                    Logger.Log("FAILED TO SAVE :" + name, LogType.Error);
                    Logger.Log(e.Message, LogType.ErrorMessage);
                    Player.GlobalMessage("FAILED TO SAVE :" + name);
                    return;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        // Deprecated - We should garuntee that this is gone by 216
        public bool Backup(string path)
        {
            if (!backedup)
            {
                string BackPath = path + "/" + name + ".lvl";
                string current = "levels/" + name + ".lvl";
                try
                {
                    File.Copy(current, BackPath, true);
                    backedup = true;
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Log("FAILED TO INCREMENTAL BACKUP :" + name, LogType.Error);
                    Logger.Log(e.Message, LogType.ErrorMessage);
                    Player.GlobalMessage("FAILED TO INCREMENTAL BACKUP :" + name);
                    return false;
                }
            }
            else
            {
                Logger.Log("Map unchanged, skipping backup", LogType.Debug);
                return false;
            }
        }

        // This function should not need a path, and should check to see if the level has been backed up
        public bool Backup()
        {
            Int16 backupNumber;
            string nextBackupPath;
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
                backupNumber = 1;

                // If we have to make the backup path, we should ensure it gets backed up!
                backedup = false;
            }
            else
            {
                // Count how many backups we have
                backupNumber = Convert.ToInt16(Directory.GetDirectories(backupPath + name).Length + 1);
            }

            // Only make a backup if we need it.
            if (!backedup)
            {
                try
                {
                    nextBackupPath = backupPath + name + "/" + backupNumber + "/";
                    if (!Directory.Exists(nextBackupPath))
                    {
                        Directory.CreateDirectory(nextBackupPath);
                    }
                    File.Copy(levelPath, nextBackupPath + name + ".lvl", true);
                    backedup = true;
                    foreach (Player p in Player.players)
                    {
                        if (p.level == this)
                            p.SendMessage("Backup " + backupNumber + " saved.");
                    }
                    Logger.Log("Backup " + backupNumber + " saved for " + this.name);
                }
                catch (Exception e)
                {
                    Logger.Log("Failed to backup: " + name, LogType.Error);
                    Logger.Log(e.Message, LogType.ErrorMessage);
                    Player.GlobalMessage("Failed to backup: " + name);
                }
            }
            else
            {
                Logger.Log("Map unchanged, skipping backup", LogType.Debug);
            }
            return backedup;
        }

        public static Map Load(string name) { return Load(name, Physics.Off); }

        public static Map Load(string name, Physics phys)
        {
            Map level = null;
            string path = "levels/" + name + ".lvl";
            if (File.Exists(path))
            {

                FileStream fs = File.OpenRead(path);
                try
                {
                    GZipStream gs = new GZipStream(fs, CompressionMode.Decompress);
                    byte[] ver = new byte[2];
                    gs.Read(ver, 0, ver.Length);
                    ushort version = BitConverter.ToUInt16(ver, 0);
                    if (version == 1875)
                    {

                    }
                    else if (version == 1874)
                    {
                        byte[] header = new byte[16]; gs.Read(header, 0, header.Length);
                        ushort width = BitConverter.ToUInt16(header, 0);
                        ushort height = BitConverter.ToUInt16(header, 2);
                        ushort depth = BitConverter.ToUInt16(header, 4);
                        level = new Map(name, width, depth, height, "empty");
                        level.spawnx = BitConverter.ToUInt16(header, 6);
                        level.spawnz = BitConverter.ToUInt16(header, 8);
                        level.spawny = BitConverter.ToUInt16(header, 10);
                        level.rotx = header[12]; level.roty = header[13];
                        level.permissionvisit = (LevelPermission)header[14];
                        level.permissionbuild = (LevelPermission)header[15];
                    }
                    else
                    {
                        byte[] header = new byte[12]; gs.Read(header, 0, header.Length);
                        ushort width = version;
                        ushort height = BitConverter.ToUInt16(header, 0);
                        ushort depth = BitConverter.ToUInt16(header, 2);
                        level = new Map(name, width, depth, height, "empty");
                        level.spawnx = BitConverter.ToUInt16(header, 4);
                        level.spawnz = BitConverter.ToUInt16(header, 6);
                        level.spawny = BitConverter.ToUInt16(header, 8);
                        level.rotx = header[10]; level.roty = header[11];
                    }

                    level.Physics = phys;

                    byte[] blocks = new byte[level.width * level.height * level.depth];
                    gs.Read(blocks, 0, blocks.Length);
                    for (int i = 0; i < level.width * level.height * level.depth; ++i)
                    {
                        level.blocks[i] = blocks[i];
                    }
                    gs.Close();
                    Logger.Log("LOADED: Map \"" + name + "\".");
                    level.backedup = true;
                }
                catch (Exception ex)
                {
                    Logger.Log("ERROR loading level \"" + name + "\".", LogType.Error);
                    Logger.Log(ex.StackTrace, LogType.ErrorMessage);
                    level = null;
                }
                finally { fs.Close(); }
            }
            else
            {
                Logger.Log("ERROR loading level \"" + name + "\".", LogType.Error);
                level = null;
            }

            return level;
        }

        public int PosToInt(ushort x, ushort y, ushort z)
        {
            if (x < 0) { return -1; }
            if (x >= width) { return -1; }
            if (y < 0) { return -1; }
            if (y >= depth) { return -1; }
            if (z < 0) { return -1; }
            if (z >= height) { return -1; }
            return x + z * width + y * width * height;
            //alternate method: (h * widthY + y) * widthX + x;
        }

        public void IntToPos(int pos, out ushort x, out ushort y, out ushort z)
        {
            y = (ushort)(pos / width / height); pos -= y * width * height;
            z = (ushort)(pos / width); pos -= z * width; x = (ushort)pos;
        }

        public int IntOffset(int pos, int x, int y, int z)
        {
            return pos + x + z * width + y * width * height;
        }

        public static LevelPermission PermissionFromName(string name)
        {
            LevelPermission permission = LevelPermission.Null;
            switch (name.ToLower())
            {
                case "guest":
                    permission = LevelPermission.Guest;
                    break;
                case "builder":
                    permission = LevelPermission.Builder;
                    break;
                case "advbuilder":
                    permission = LevelPermission.AdvBuilder;
                    break;
                case "moderator":
                    permission = LevelPermission.Moderator;
                    break;
                case "operator":
                    permission = LevelPermission.Operator;
                    break;
                case "admin":
                    permission = LevelPermission.Admin;
                    break;
            }

            return permission;
        }

        public static string PermissionToName(LevelPermission perm)
        {
            string strPermission = "Null";
            switch (perm)
            {
                case LevelPermission.Guest:
                    strPermission = "Guest";
                    break;
                case LevelPermission.Builder:
                    strPermission = "Builder";
                    break;
                case LevelPermission.AdvBuilder:
                    strPermission = "AdvBuilder";
                    break;
                case LevelPermission.Moderator:
                    strPermission = "Moderator";
                    break;
                case LevelPermission.Operator:
                    strPermission = "Operator";
                    break;
                case LevelPermission.Admin:
                    strPermission = "Admin";
                    break;
            }

            return strPermission;
        }

        public static bool Loaded(string name)
        {
            bool blnLoaded = false;
            foreach (Map level in Server.levels)
            {
                if (level.name == name)
                {
                    blnLoaded = true;
                    break;
                }
            }

            return blnLoaded;
        }

        public static bool Exists(string name)
        {
            return (File.Exists("levels/" + name + ".lvl") || File.Exists("levels/" + name + ".lvl.backup"));
        }

        
    }

    public class Check
    {
        public int b;
        public byte time;
        public Check(int b)
        {
            this.b = b;
            time = 0;
        }
    }

    public class Update
    {
        public int b;
        public byte type;
        public Update(int b, byte type)
        {
            this.b = b;
            this.type = type;
        }
    }
 }
