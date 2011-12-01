using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelConverter.Level.MCSharp
{
    internal class Map
    {
        public uint FormatID;   // 0xFC000003

        public ushort DimX;    // 1st horizontal ("width") map dimension
        public ushort DimY;    // vertical ("depth") map dimension
        public ushort DimZ;    // 2nd horizontal ("height") map dimension

        public ushort SpawnX;  // in player coordinates. Divide by 32 to get block coordinates.
        public ushort SpawnY;
        public ushort SpawnZ;
        public byte SpawnRot;    // Rotation
        public byte SpawnYaw;    // Look/Yaw

        
        public DateTime Modified;   // UTC unix timestamp (time of last save)
        public DateTime Created;    // UTC unix timestamp (time of initial generation/import)
        public long MapGUID;   // random identifier (created at generation/import time)

        public long DataLayerFlags;    // 1 bit for each reserved DataLayerType, only bits 0 & 1 are currently used
        public byte DataLayerCount;    // how many layers are used total (reserved+custom)

        public uint MetadataRecordCount;   // How many meta data records do we have?

        public IndexRecord[] Index;    // collection of IndexRecords, ALL 256 MUST BE LISTED to ensure a fixed length
        // === Fixed size header ends here ===
        // First metadata record is located at offset = 3120

        public MetadataRecord[] Metadata;

        public DataLayer[] DataLayers;

        public Map()
        {
            FormatID = 0xFC000003;
            Index = new IndexRecord[256];
            DimX = 0;
            DimY = 0;
            DimZ = 0;
            SpawnX = 0;
            SpawnY = 0;
            SpawnZ = 0;
            SpawnRot = 0;
            SpawnYaw = 0;
            Modified = new DateTime();
            Created = new DateTime();
            MapGUID = 0;
            DataLayerFlags = 0;
            DataLayerCount = 0;
            MetadataRecordCount = 0;
        }
    }
}
/* Reserved metadata keys names. These are not required, but suggested.
 * All non-server-specific reserved groups start with an underscore.
 * 
 *      Group           Key                 Purpose
 *      ----------------------------------------------------------------------------------
 *      _Origin         Author              Name of the person/group/organization who created the map
 *      _Origin         Server              Name of the server (not server software) where map originated
 *      _Origin         FileName            Filename from which this map was converted (if any)
 *      _Origin         WorldName           Name of a world/level from which this map was saved (if any)
 *      _Origin         GeneratorName       Identifier of the software that generated this map (if any)
 *      _Origin         GeneratorVersion    Version of the software that generated this map
 *      _Origin         GeneratorParams     Serialized generation parameters. Format is generator-specific
 *      _CustomBlocks   "0" - "255"         list of custom blocks and their mappings to standard types
 *      fCraft.*        (all)               Reserved for fCraft custom server
 *      SpaceCraft.*    (all)               Reserved for SpaceCraft custom server
 */