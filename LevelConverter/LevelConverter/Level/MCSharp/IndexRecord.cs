using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelConverter.Level.MCSharp
{
    struct IndexRecord
    {
        public long Offset;            // if 0, means "not defined"  - 8 Bytes
        public int CompressedLength;   // length of data INCLUDING 9-byte DataLayer header - 4 Bytes
    } // 12 bytes
}
