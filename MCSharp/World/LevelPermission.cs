using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSharp
{
    public enum LevelPermission
    {
        Null = 0x99,
        Guest = 0x00,
        Builder = 0x01,
        AdvBuilder = 0x02,
        Moderator = 0x03,
        Operator = 0x04,
        Admin = 0x05
    }
}
