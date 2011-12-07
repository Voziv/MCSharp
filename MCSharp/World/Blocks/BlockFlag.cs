using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSharp
{
    [Flags]
    public enum BlockFlag
    {
        None = 0,
        Locked = 1,
        Door = 2,
        DoorOpened = 4,
        PhysicsLiquid = 8,
        PhysicsGravity = 16
    }
}
