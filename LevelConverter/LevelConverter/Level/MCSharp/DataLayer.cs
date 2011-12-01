using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelConverter.Level.MCSharp
{
    // one layer of binary data 
    struct DataLayer
    {
        public DataLayerType Type;     // see "DataLayerType" below
        public DataLayerCompressionType CompressionType;   // see "DataLayerCompressionType" below
        public int GeneralPurposeField;                    // 32 bits that can be used in implementation-specific ways
        public int ElementSize;        // size of each data element (if elements are variable-size, set this to 1)
        public int ElementCount;       // number of fixed-sized elements (if elements are variable-size, set this to total number of bytes)
        // uncompressed length = (element size * element count)
        public byte[] CompressedData;
    } // 14+ bytes
}
