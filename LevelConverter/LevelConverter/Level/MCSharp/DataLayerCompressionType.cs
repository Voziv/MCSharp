using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelConverter.Level.MCSharp
{
    enum DataLayerCompressionType : byte
    {
        None = 0,           // raw, uncompressed data
        Deflate = 1,        // deflate with no header
        DeflateGZip = 2     // deflate with gzip header
        // 3-255 reserved
    } // 1 byte
}
