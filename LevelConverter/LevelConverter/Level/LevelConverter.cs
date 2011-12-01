using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

using LevelConverter.Level.MCSharp;

namespace LevelConverter
{
    public class LevelConverter
    {
        // Properties
        public string FileName
        {
            get { return mFileName; }
            set { mFileName = value; }
        }
        public LevelFormat InputFormat { get { return mInputFormat; } }


        // Private Members
        string mFileName;
        Map myMap = new Map();

        // Map Members
        byte[] blocks;
        byte lvlBuildPermission;
        byte lvlVisitPermission;
        LevelFormat mInputFormat;
        ushort x; // Width
        ushort y; // Height
        ushort z; // Length/Depth
        ushort spawnx, spawny, spawnz;
        byte spawnrotx, spawnroty;
        string mapName;



        // Constructor
        public LevelConverter()
        {
            mFileName = String.Empty;
            mInputFormat = LevelFormat.Null;
            lvlVisitPermission = 0;
            lvlBuildPermission = 0;
        }

        public bool Load()
        {
            bool success = false;
            if (File.Exists(mFileName))
            {
                switch (Path.GetExtension(mFileName).ToLower())
                {
                    case ".lvl":
                        success = LoadMCSharp();
                        break;
                    case ".mlvl":
                        success = LoadNewMCSharp();
                        break;
                    case ".fcm":
                        success = false; // We do not support loading of fCraft Yet
                        break;
                    case ".dat":
                        success = LoadDAT();
                        break;
                }
            }
            return success;
        }

        public bool LoadNewMCSharp()
        {
            bool success = false;
            try
            {
                FileStream fs = File.OpenRead(mFileName);
                GZipStream gs = new GZipStream(fs, CompressionMode.Decompress);
                byte[] format = new byte[4];
                byte[] header = new byte[3128];
                fs.Read(format, 0, format.Length);
                Buffer.BlockCopy(format, 0, header, 0, format.Length);
                // Check format here
                // Do read of the rest of the header
                fs.Read(header, format.Length, header.Length - format.Length);
                
                // Load into the map format
                myMap.FormatID = BitConverter.ToUInt32(header, 0);
                myMap.DimX = BitConverter.ToUInt16(header, 4);                      // Horizontal "Width" Dimension
                myMap.DimY = BitConverter.ToUInt16(header, 6);                      // Map Height Dimension
                myMap.DimZ = BitConverter.ToUInt16(header, 8);                      // Map Length dimension
                myMap.SpawnX = BitConverter.ToUInt16(header, 10);                   // Spawn X
                myMap.SpawnZ = BitConverter.ToUInt16(header, 12);                   // Spawn Z
                myMap.SpawnY = BitConverter.ToUInt16(header, 14);                   // Spawn Y
                myMap.SpawnRot = header[16];                                        // Spawn Rotation X
                myMap.SpawnYaw = header[17];                                        // Spawn Rotation Y
                myMap.Modified = ConvertFromUnixTimestamp((double)BitConverter.ToUInt64(header, 18));                  // Date Modified
                myMap.Created = ConvertFromUnixTimestamp((double)BitConverter.ToUInt64(header, 26));                   // Date Created
                myMap.MapGUID = BitConverter.ToInt64(header, 34);                   // Map GUID
                myMap.DataLayerFlags = BitConverter.ToInt64(header, 42);            // Data Layer Flags
                myMap.DataLayerCount = header[50];                                  // How many data layers are used
                myMap.MetadataRecordCount = BitConverter.ToUInt32(header, 51);      // Meta Data Record Count
                // Index Records (all 256 = 3072 bytes)
                for (int i = 0; i < myMap.Index.Length; i++)
                {
                    myMap.Index[i].Offset = BitConverter.ToInt64(header, 55 + (i * 12));
                    myMap.Index[i].CompressedLength = BitConverter.ToInt32(header, 55 + (i * 12 + 8));                    
                }


                gs.Close();
                success = true;
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Loading the new MCSharp format");
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public bool LoadMCSharp()
        {
            bool success = false;
            FileStream fs = File.OpenRead(mFileName);
            try
            {
                // Open a GZIP Stream
                GZipStream gs = new GZipStream(fs, CompressionMode.Decompress);
                byte[] ver = new byte[2];
                gs.Read(ver, 0, ver.Length);
                ushort version = BitConverter.ToUInt16(ver, 0);

                if (version == 1900)
                {

                }
                else if (version == 1874)
                {
                    // Read in the header
                    byte[] header = new byte[16];
                    gs.Read(header, 0, header.Length);

                    // Map Dimensions
                    x = BitConverter.ToUInt16(header, 0);
                    y = BitConverter.ToUInt16(header, 2);
                    z = BitConverter.ToUInt16(header, 4);
                    //
                    spawnx = BitConverter.ToUInt16(header, 6);
                    spawnz = BitConverter.ToUInt16(header, 8);
                    spawny = BitConverter.ToUInt16(header, 10);
                    spawnrotx = header[12];
                    spawnroty = header[13];
                    lvlVisitPermission = header[14];
                    lvlBuildPermission = header[15];

                    // Read in block data
                    blocks = new byte[x * y * z];
                    gs.Read(blocks, 0, blocks.Length);
                }
                else
                {
                    // Read in the header
                    byte[] header = new byte[12];
                    gs.Read(header, 0, header.Length);

                    // Map Dimensions
                    x = version;
                    y = BitConverter.ToUInt16(header, 0);
                    z = BitConverter.ToUInt16(header, 2);

                    // Spawn
                    spawnx = BitConverter.ToUInt16(header, 4);
                    spawnz = BitConverter.ToUInt16(header, 6);
                    spawny = BitConverter.ToUInt16(header, 8);
                    spawnrotx = header[10];
                    spawnroty = header[11];

                    // Read in block data
                    blocks = new byte[x * y * z];
                    gs.Read(blocks, 0, blocks.Length);
                }

                // Close the GZIP stream
                gs.Close();
                success = true;
                mInputFormat = LevelFormat.MCSharp;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while loading the old MCSharp level.");
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public bool LoadDAT()
        {
            bool success = false;
            com.mojang.minecraft.level.Level dat;
            try
            {
                // Load file
                dat = com.mojang.minecraft.level.Level.Load(mFileName);

                // Read in levelname
                mapName = dat.name;

                // Read in dimensions
                x = (ushort)dat.width;
                y = (ushort)dat.height;
                z = (ushort)dat.depth;

                // Read in spawn
                spawnx = (ushort)dat.xSpawn;
                spawny = (ushort)dat.ySpawn;
                spawnz = (ushort)dat.zSpawn;
                spawnrotx = 0;
                spawnroty = 0;

                // Read in blocks
                blocks = new byte[dat.blocks.Length];
                blocks = dat.blocks;


                success = true;
                mInputFormat = LevelFormat.MinecraftDAT;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error loading dat");
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }

            return success;
        }

        public bool SaveMCSharp()
        {
            bool success = false;
            try
            {
                string strFileName = Path.GetFileNameWithoutExtension(mFileName) + ".lvl";

                FileStream fs = File.Create(strFileName);
                GZipStream gs = new GZipStream(fs, CompressionMode.Compress);

                // Build Header
                byte[] header = new byte[18];
                BitConverter.GetBytes(1874).CopyTo(header, 0);      // Map version number
                BitConverter.GetBytes(x).CopyTo(header, 2);         // Horizontal "Width" Dimension
                BitConverter.GetBytes(y).CopyTo(header, 4);         // Map Height Dimension
                BitConverter.GetBytes(z).CopyTo(header, 6);         // Map Length dimension
                BitConverter.GetBytes(spawnx).CopyTo(header, 8);    // Spawn X
                BitConverter.GetBytes(spawnz).CopyTo(header, 10);    // Spawn Z
                BitConverter.GetBytes(spawny).CopyTo(header, 12);   // Spawn Y
                header[14] = spawnrotx;                             // Spawn Rotation X
                header[15] = spawnroty;                             // Spawn Rotation Y
                header[16] = (byte)lvlVisitPermission;              // Map Visit Permission
                header[17] = (byte)lvlBuildPermission;              // Map Build Permission

                // Write out the header
                gs.Write(header, 0, header.Length);

                // Write out the block array
                gs.Write(blocks, 0, blocks.Length);

                gs.Close();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Saving the old MCSharp format");
                Console.WriteLine(e.Message);
            }
            return success;
        }

        public bool SaveNewMCSharp()
        {
            bool success = false;
            try
            {
                string strFileName = Path.GetFileNameWithoutExtension(mFileName) + ".mlvl";
                FileStream fs = File.Create(strFileName);
                GZipStream gs = new GZipStream(fs, CompressionMode.Compress);
                
                // Header
                byte[] header = new byte[3128];
                
                BitConverter.GetBytes(myMap.FormatID).CopyTo(header, 0);                // Format ID
                BitConverter.GetBytes(myMap.DimX).CopyTo(header, 4);                    // Horizontal "Width" Dimension
                BitConverter.GetBytes(myMap.DimY).CopyTo(header, 6);                    // Map Height Dimension
                BitConverter.GetBytes(myMap.DimZ).CopyTo(header, 8);                    // Map Length dimension
                BitConverter.GetBytes(myMap.SpawnX).CopyTo(header, 10);                 // Spawn X
                BitConverter.GetBytes(myMap.SpawnZ).CopyTo(header, 12);                 // Spawn Z
                BitConverter.GetBytes(myMap.SpawnY).CopyTo(header, 14);                 // Spawn Y
                header[16] = myMap.SpawnRot;                                            // Spawn Rotation X
                header[17] = myMap.SpawnYaw;                                            // Spawn Rotation Y
                ulong modifiedDate = (ulong)ConvertToUnixTimestamp(myMap.Created);      // Date Modified
                BitConverter.GetBytes(modifiedDate).CopyTo(header, 18);                 // **
                ulong createDate = (ulong)ConvertToUnixTimestamp(myMap.Created);        // Date Created
                BitConverter.GetBytes(createDate).CopyTo(header, 26);                   // **
                BitConverter.GetBytes(myMap.MapGUID).CopyTo(header, 34);                // Map GUID
                BitConverter.GetBytes(myMap.DataLayerFlags).CopyTo(header, 42);         // Data Layer Flags
                header[50] = myMap.DataLayerCount;                                      // How many data layers are used
                BitConverter.GetBytes(myMap.MetadataRecordCount).CopyTo(header, 51);    // Meta Data Record Count
                // Index Records (all 256 = 3072 bytes)
                if (myMap.Index.Length != 256)
                {
                    throw new Exception("The length of the Index IndexRecord was not 256!");
                }
                for (int i = 0; i < myMap.Index.Length; i++)
                {
                    BitConverter.GetBytes(myMap.Index[i].Offset).CopyTo(header, 55 + (i * 12));
                    BitConverter.GetBytes(myMap.Index[i].CompressedLength).CopyTo(header, 55 + (i * 12 + 8));
                }
                
                // Write out the header
                fs.Write(header, 0, header.Length);
                gs.Close();
                success = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Saving the new MCSharp format");
                Console.WriteLine(e.Message);
            }
            return success;
        }

        static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }


        static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}
