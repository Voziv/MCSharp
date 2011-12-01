using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelConverter.Level.MCSharp
{
    enum DataLayerType : byte
    {
        PlayerTable = 0,                    // Table of players that have been on the map
        BlockPhysicsCode = 1,               // Definition of all the phyiscs. Blocks should reference these
        BlockArray = 2,                     // Array of blocks that make up the world
        BlocksUndo = 3,                     // Last change (per-block) ***Not Used by MCSharp***
        BlockProperties = 4,                // Paralell array to block array, defining what physics code to run on specific blocks
        BlockAccessLevel = 5,               // Paralell array of block access levels
        BlockOwner = 6                      // Paralell array of PlayerIDs
        
        // 2-63 reserved
        // 64-255 custom
    } // 1 byte
}
