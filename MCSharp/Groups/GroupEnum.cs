using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSharp
{
    public enum GroupEnum
    {
        Null = 0, 
        Banned = 1,
        Guest = 2,
        Builder = 3,
        AdvBuilder = 4,
        Moderator = 5,
        Operator = 6,
        Administrator = 7,
        Console = 9,
        Disabled = 255
    }
}
