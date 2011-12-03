using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSharp
{
    public sealed partial class Player
    {
        /// <summary>
        /// Handles a players movement.
        /// </summary>
        /// <param name="packet">A byte array containing the input packet information</param>
        void HandleInput (byte[] packet)
        {
            // Only process the movement packet if the user is logged in
            if (loggedIn)
            {
                byte thisid = packet[0];
                ushort x = NTHO(packet, 1);
                ushort y = NTHO(packet, 3);
                ushort z = NTHO(packet, 5);
                byte rotx = packet[7];
                byte roty = packet[8];
                pos = new ushort[3] { x, y, z };
                rot = new byte[2] { rotx, roty };
            }
        }

    }
}
