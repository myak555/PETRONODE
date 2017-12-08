using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Petronode.OilfieldFileAccess.Converters
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct NumberUnion
    {
        [FieldOffset(0)]
        public Int16 i16;
        [FieldOffset(0)]
        public UInt16 ui16;
        [FieldOffset(0)]
        public Int32 i32;
        [FieldOffset(0)]
        public UInt32 ui32;
        [FieldOffset(0)]
        public float f;
        [FieldOffset(0)]
        public double d;
        [FieldOffset(0)]
        public byte b0;
        [FieldOffset(1)]
        public byte b1;
        [FieldOffset(2)]
        public byte b2;
        [FieldOffset(3)]
        public byte b3;
        [FieldOffset(4)]
        public byte b4;
        [FieldOffset(5)]
        public byte b5;
        [FieldOffset(6)]
        public byte b6;
        [FieldOffset(7)]
        public byte b7;
    }
}
