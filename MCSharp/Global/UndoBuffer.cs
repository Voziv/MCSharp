using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSharp
{
    public class UndoBuffer : BlockBuffer
    {
        // Properties
        public bool Confirmed { get { return blnConfirmed; } }
        public bool HasUndo { get { return blnHasUndo; } }

        // Private Members
        bool blnConfirmed, blnHasUndo;
        BlockPos firstPos, secondPos;
        string strLevelName;

        // Constructor
        public UndoBuffer()
        {
            blnConfirmed = false;
            blnHasUndo = false;
            strLevelName = "";
        }

        /// <summary>
        /// Sets the Block Buffer to the given set of blocks. _blocks size MUST be _x * _y * _z
        /// </summary>
        /// <param name="_blocks">Block Array</param>
        /// <param name="_x">X dimension of buffer</param>
        /// <param name="_y">Y dimension of buffer</param>
        /// <param name="_z">Z dimension of buffer</param>
        /// <param name="cornerA">First position of the cuboid</param>
        /// <param name="cornerB">Second position of the cuboid</param>
        /// <param name="level">What level this was done on</param>
        public void SetBuffer(byte[,,] _blocks, ushort _x, ushort _y, ushort _z, BlockPos cornerA, BlockPos cornerB, string level)
        {
            if (Math.Abs(cornerA.x - cornerB.x) * Math.Abs(cornerA.y - cornerB.y) * Math.Abs(cornerA.z - cornerB.z) == _blocks.Length)
            {
                SetBuffer(_blocks, _x, _y, _z);
                firstPos = cornerA;
                secondPos = cornerB;
                strLevelName = level;
            }
            else
            {
                Logger.Log("Undo Buffer Size did not match coordinates. Buffer Size: " + _blocks.Length + 
                           ", Coordinate Size: " + 
                           Math.Abs(cornerA.x - cornerB.x) * Math.Abs(cornerA.y - cornerB.y) * Math.Abs(cornerA.z - cornerB.z)
                           ,LogType.Debug);
            }
        }
    }
}
