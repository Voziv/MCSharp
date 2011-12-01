using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCSharp
{
    public class BlockBuffer
    {
        // Properties
        public ushort DimX { get { return mDimX; } }
        public ushort DimY { get { return mDimY; } }
        public ushort DimZ { get { return mDimZ; } }
        public byte[,,] Buffer { get { return mBuffer; } }

        // Private Members
        byte[,,] mBuffer;
        ushort mDimX, mDimY, mDimZ;
        
        public BlockBuffer()
        {
            mDimX = 1;
            mDimX = 1;
            mDimX = 1;
            mBuffer = new byte[mDimX, mDimY, mDimZ];
        }

        /// <summary>
        /// Sets the Block Buffer to the given set of blocks. _blocks size MUST be _x * _y * _z
        /// </summary>
        /// <param name="_blocks">Block Array</param>
        /// <param name="_x">X dimension of buffer</param>
        /// <param name="_y">Y dimension of buffer</param>
        /// <param name="_z">Z dimension of buffer</param>
        public virtual void SetBuffer(byte[,,] _blocks, ushort _x, ushort _y, ushort _z)
        {
            if (_blocks.Length == _x * _y * _z)
            {
                mDimX = _x;
                mDimY = _y;
                mDimZ = _z;
                mBuffer = _blocks;
            }
        }

        public byte GetTile(ushort _x, ushort _y, ushort _z)
        {
            return mBuffer[_x, _y, _z];
        }
    }
}
