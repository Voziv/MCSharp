using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelConverter.Level.MCSharp
{
    // same as FCM v1-v2
    struct PrefixedString
    {
        public ushort StringLength;    // can be 0 (meaning "empty string")
        public byte[] StringBytes;      // UTF-8 encoded
    } // 2+ bytes
}
