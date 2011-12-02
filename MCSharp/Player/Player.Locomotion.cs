using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSharp
{
    public sealed partial class Player
    {
        /// <summary>
        /// Handles a players movement. Not sure what M is at the moment.
        /// </summary>
        /// <param name="m">THIS NEEDS TO BE DOCUMENTED!!</param>
        void HandleInput (object m)
        {
            byte[] message = (byte[]) m;
            if (!loggedIn)
                return;

            byte thisid = message[0];
            ushort x = NTHO(message, 1);
            ushort y = NTHO(message, 3);
            ushort z = NTHO(message, 5);
            byte rotx = message[7];
            byte roty = message[8];
            pos = new ushort[3] { x, y, z };
            rot = new byte[2] { rotx, roty };
        }

    }
}
