using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LevelConverter.Level.MCSharp
{
    struct MetadataRecord
    {
        public PrefixedString KeyGroup;    // Can be empty (implies global group). Also see "ReservedKeyGroups" below
        public PrefixedString Key;
        public PrefixedString Value;
    } // 6+ bytes
}
