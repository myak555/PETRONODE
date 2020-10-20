using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.SlideComposerLibrary
{
    /// <summary>
    /// Encapsulates the buffer operations
    /// </summary>
    public class BufferConverter
    {
        #region Int16
        /// <summary>
        /// Returns two-byte integer from the buffer (Little-Endian)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Int16 GetBytesInt16(byte[] Buffer, NumberUnion fu, int offset)
        {
            if (Buffer == null) return 0;
            fu.b0 = Buffer[offset];
            fu.b1 = Buffer[offset + 1];
            fu.b2 = 0;
            fu.b3 = 0;
            fu.b4 = 0;
            fu.b5 = 0;
            fu.b6 = 0;
            fu.b7 = 0;
            return fu.i16;
        }

        /// <summary>
        /// Returns two-byte integer from the buffer (Little-Endian)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static UInt16 GetBytesUInt16(byte[] Buffer, NumberUnion fu, int offset)
        {
            if (Buffer == null) return 0;
            fu.b0 = Buffer[offset];
            fu.b1 = Buffer[offset + 1];
            fu.b2 = 0;
            fu.b3 = 0;
            fu.b4 = 0;
            fu.b5 = 0;
            fu.b6 = 0;
            fu.b7 = 0;
            return fu.ui16;
        }

        /// <summary>
        /// Returns two-byte integer from the buffer (Big-Endian)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Int16 GetBytesInt16_BE(byte[] Buffer, NumberUnion fu, int offset)
        {
            if (Buffer == null) return 0;
            fu.b0 = Buffer[offset+1];
            fu.b1 = Buffer[offset];
            fu.b2 = 0;
            fu.b3 = 0;
            fu.b4 = 0;
            fu.b5 = 0;
            fu.b6 = 0;
            fu.b7 = 0;
            return fu.i16;
        }

        /// <summary>
        /// Returns two-byte integer from the buffer (Big-Endian)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static UInt16 GetBytesUInt16_BE(byte[] Buffer, NumberUnion fu, int offset)
        {
            if (Buffer == null) return 0;
            fu.b0 = Buffer[offset + 1];
            fu.b1 = Buffer[offset];
            fu.b2 = 0;
            fu.b3 = 0;
            fu.b4 = 0;
            fu.b5 = 0;
            fu.b6 = 0;
            fu.b7 = 0;
            return fu.ui16;
        }

        /// <summary>
        /// Sets two-byte integer to the buffer (Little-Endian)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetBytesInt16(byte[] Buffer, NumberUnion fu, Int16 value, int offset)
        {
            if (Buffer == null) return;
            fu.i32 = 0;
            fu.i16 = value;
            Buffer[offset] = fu.b0;
            Buffer[offset + 1] = fu.b1;
        }

        /// <summary>
        /// Sets two-byte integer to the buffer (Little-Endian)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetBytesUInt16(byte[] Buffer, NumberUnion fu, UInt16 value, int offset)
        {
            if (Buffer == null) return;
            fu.i32 = 0;
            fu.ui16 = value;
            Buffer[offset] = fu.b0;
            Buffer[offset + 1] = fu.b1;
        }

        /// <summary>
        /// Sets two-byte integer to the buffer (Big-Endian)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetBytesInt16_BE(byte[] Buffer, NumberUnion fu, Int16 value, int offset)
        {
            if (Buffer == null) return;
            fu.i32 = 0;
            fu.i16 = value;
            Buffer[offset + 1] = fu.b0;
            Buffer[offset] = fu.b1;
        }

        /// <summary>
        /// Sets two-byte integer to the buffer (Big-Endian)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetBytesUInt16_BE(byte[] Buffer, NumberUnion fu, UInt16 value, int offset)
        {
            if (Buffer == null) return;
            fu.i32 = 0;
            fu.ui16 = value;
            Buffer[offset + 1] = fu.b0;
            Buffer[offset] = fu.b1;
        }
        #endregion

        #region Int32
        /// <summary>
        /// Returns four-byte integer from the buffer (Little-Endian)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Int32 GetBytesInt32(byte[] Buffer, NumberUnion fu, int offset)
        {
            if (Buffer == null) return 0;
            fu.b0 = Buffer[offset];
            fu.b1 = Buffer[offset + 1];
            fu.b2 = Buffer[offset + 2];
            fu.b3 = Buffer[offset + 3];
            fu.b4 = 0;
            fu.b5 = 0;
            fu.b6 = 0;
            fu.b7 = 0;
            return fu.i32;
        }

        /// <summary>
        /// Returns four-byte integer from the buffer (Little-Endian)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static UInt32 GetBytesUInt32(byte[] Buffer, NumberUnion fu, int offset)
        {
            if (Buffer == null) return 0;
            fu.b0 = Buffer[offset];
            fu.b1 = Buffer[offset + 1];
            fu.b2 = Buffer[offset + 2];
            fu.b3 = Buffer[offset + 3];
            fu.b4 = 0;
            fu.b5 = 0;
            fu.b6 = 0;
            fu.b7 = 0;
            return fu.ui32;
        }

        /// <summary>
        /// Returns four-byte integer from the buffer (Big-Endian)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Int32 GetBytesInt32_BE(byte[] Buffer, NumberUnion fu, int offset)
        {
            if (Buffer == null) return 0;
            fu.b0 = Buffer[offset + 3];
            fu.b1 = Buffer[offset + 2];
            fu.b2 = Buffer[offset + 1];
            fu.b3 = Buffer[offset];
            fu.b4 = 0;
            fu.b5 = 0;
            fu.b6 = 0;
            fu.b7 = 0;
            return fu.i32;
        }

        /// <summary>
        /// Returns four-byte integer from the buffer (Big-Endian)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static UInt32 GetBytesUInt32_BE(byte[] Buffer, NumberUnion fu, int offset)
        {
            if (Buffer == null) return 0;
            fu.b0 = Buffer[offset + 3];
            fu.b1 = Buffer[offset + 2];
            fu.b2 = Buffer[offset + 1];
            fu.b3 = Buffer[offset];
            fu.b4 = 0;
            fu.b5 = 0;
            fu.b6 = 0;
            fu.b7 = 0;
            return fu.ui32;
        }

        /// <summary>
        /// Sets four-byte integer to the buffer (Little-Endian)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetBytesInt32(byte[] Buffer, NumberUnion fu, Int32 value, int offset)
        {
            if (Buffer == null) return;
            fu.i32 = value;
            Buffer[offset] = fu.b0;
            Buffer[offset + 1] = fu.b1;
            Buffer[offset + 2] = fu.b2;
            Buffer[offset + 3] = fu.b3;
        }

        /// <summary>
        /// Sets four-byte integer to the buffer (Little-Endian)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetBytesUInt32(byte[] Buffer, NumberUnion fu, UInt32 value, int offset)
        {
            if (Buffer == null) return;
            fu.ui32 = value;
            Buffer[offset] = fu.b0;
            Buffer[offset + 1] = fu.b1;
            Buffer[offset + 2] = fu.b2;
            Buffer[offset + 3] = fu.b3;
        }

        /// <summary>
        /// Sets four-byte integer to the buffer (Big-Endian)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetBytesInt32_BE(byte[] Buffer, NumberUnion fu, Int32 value, int offset)
        {
            if (Buffer == null) return;
            fu.i32 = value;
            Buffer[offset + 3] = fu.b0;
            Buffer[offset + 2] = fu.b1;
            Buffer[offset + 1] = fu.b2;
            Buffer[offset] = fu.b3;
        }

        /// <summary>
        /// Sets four-byte integer to the buffer (Big-Endian)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetBytesUInt32_BE(byte[] Buffer, NumberUnion fu, UInt32 value, int offset)
        {
            if (Buffer == null) return;
            fu.ui32 = value;
            Buffer[offset + 3] = fu.b0;
            Buffer[offset + 2] = fu.b1;
            Buffer[offset + 1] = fu.b2;
            Buffer[offset] = fu.b3;
        }
        #endregion

        #region Float
        /// <summary>
        /// Returns four-byte float from the buffer (Little-Endian)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static float GetBytesFloat(byte[] Buffer, NumberUnion fu, int offset)
        {
            if (Buffer == null) return 0.0f;
            fu.b0 = Buffer[offset];
            fu.b1 = Buffer[offset + 1];
            fu.b2 = Buffer[offset + 2];
            fu.b3 = Buffer[offset + 3];
            fu.b4 = 0;
            fu.b5 = 0;
            fu.b6 = 0;
            fu.b7 = 0;
            return fu.f;
        }

        /// <summary>
        /// Returns four-byte float from the buffer (Big-Endian)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static float GetBytesFloat_BE(byte[] Buffer, NumberUnion fu, int offset)
        {
            if (Buffer == null) return 0.0f;
            fu.b0 = Buffer[offset + 3];
            fu.b1 = Buffer[offset + 2];
            fu.b2 = Buffer[offset + 1];
            fu.b3 = Buffer[offset];
            fu.b4 = 0;
            fu.b5 = 0;
            fu.b6 = 0;
            fu.b7 = 0;
            return fu.f;
        }

        /// <summary>
        /// Sets four-byte float to the buffer (Little-Endian)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetBytesFloat(byte[] Buffer, NumberUnion fu, float value, int offset)
        {
            if (Buffer == null) return;
            fu.f = value;
            Buffer[offset] = fu.b0;
            Buffer[offset + 1] = fu.b1;
            Buffer[offset + 2] = fu.b2;
            Buffer[offset + 3] = fu.b3;
        }

        /// <summary>
        /// Sets four-byte float to the buffer (Big-Endian)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetBytesFloat_BE(byte[] Buffer, NumberUnion fu, float value, int offset)
        {
            if (Buffer == null) return;
            fu.f = value;
            Buffer[offset + 3] = fu.b0;
            Buffer[offset + 2] = fu.b1;
            Buffer[offset + 1] = fu.b2;
            Buffer[offset] = fu.b3;
        }
        #endregion

        #region Double
        /// <summary>
        /// Returns eight-byte double from the buffer (Little-Endian)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static double GetBytesDouble(byte[] Buffer, NumberUnion fu, int offset)
        {
            if (Buffer == null) return 0.0f;
            fu.b0 = Buffer[offset];
            fu.b1 = Buffer[offset + 1];
            fu.b2 = Buffer[offset + 2];
            fu.b3 = Buffer[offset + 3];
            fu.b4 = Buffer[offset + 4];
            fu.b5 = Buffer[offset + 5];
            fu.b6 = Buffer[offset + 6];
            fu.b7 = Buffer[offset + 7];
            return fu.d;
        }

        /// <summary>
        /// Returns eight-byte double from the buffer (Big-Endian)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static double GetBytesDouble_BE(byte[] Buffer, NumberUnion fu, int offset)
        {
            if (Buffer == null) return 0.0f;
            fu.b0 = Buffer[offset + 7];
            fu.b1 = Buffer[offset + 6];
            fu.b2 = Buffer[offset + 5];
            fu.b3 = Buffer[offset + 4];
            fu.b4 = Buffer[offset + 3];
            fu.b5 = Buffer[offset + 2];
            fu.b6 = Buffer[offset + 1];
            fu.b7 = Buffer[offset];
            return fu.d;
        }

        /// <summary>
        /// Sets eight-byte double to the buffer (Little-Endian)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetBytesDouble(byte[] Buffer, NumberUnion fu, double value, int offset)
        {
            if (Buffer == null) return;
            fu.d = value;
            Buffer[offset] = fu.b0;
            Buffer[offset + 1] = fu.b1;
            Buffer[offset + 2] = fu.b2;
            Buffer[offset + 3] = fu.b3;
            Buffer[offset + 4] = fu.b4;
            Buffer[offset + 5] = fu.b5;
            Buffer[offset + 6] = fu.b6;
            Buffer[offset + 7] = fu.b7;
        }

        /// <summary>
        /// Sets eight-byte double to the buffer (Big-Endian)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetBytesDouble_BE(byte[] Buffer, NumberUnion fu, double value, int offset)
        {
            if (Buffer == null) return;
            fu.d = value;
            Buffer[offset + 7] = fu.b0;
            Buffer[offset + 6] = fu.b1;
            Buffer[offset + 5] = fu.b2;
            Buffer[offset + 4] = fu.b3;
            Buffer[offset + 3] = fu.b4;
            Buffer[offset + 2] = fu.b5;
            Buffer[offset + 1] = fu.b6;
            Buffer[offset] = fu.b7;
        }
        #endregion

        #region IBM
        /// <summary>
        /// Returns four-byte IBM float from the buffer (IBM is always Big-endian)
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static float GetBytesIBM(byte[] Buffer, NumberUnion fu, int offset)
        {
            if (Buffer == null) return 0.0f;

            // swap bytes from the IBM order ("big endian") to the PC order
            fu.b3 = Buffer[offset];
            fu.b2 = Buffer[offset + 1];
            fu.b1 = Buffer[offset + 2];
            fu.b0 = Buffer[offset + 3];

            // if the content of the Buffer 4 bytes is already zeros, skip the pass
            if (fu.i32 == 0) return 0.0f;

            // detect mantissa and exponent
            uint mantissa = 0x00ffffff & fu.ui32;
            uint exponent = ((0x7f000000 & fu.ui32) >> 22) - 130;
            while (mantissa != 0 && (mantissa & 0x00800000) == 0)
            {
                --exponent;
                mantissa <<= 1;
            }

            // perform number checks
            if (exponent > 254 && mantissa != 0)
                fu.ui32 = (0x80000000 & fu.ui32) | 0x7f7fffff;
            else if (exponent <= 0 || mantissa == 0)
                fu.ui32 = 0;
            else
                fu.ui32 = (0x80000000 & fu.ui32) | (exponent << 23) | (0x007fffff & mantissa);
            return fu.f;
        }

        /// <summary>
        /// Sets four-byte IBM float to the buffer (IBM is always Big-Endian)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetBytesIBM(byte[] Buffer, NumberUnion fu, float value, int offset)
        {
            if (Buffer == null) return;
            fu.f = value;
            Buffer[offset] = 0x00;
            Buffer[offset + 1] = 0x00;
            Buffer[offset + 2] = 0x00;
            Buffer[offset + 3] = 0x00;

            // if the content of the source 4 bytes is already zeros, skip the pass
            if (fu.i32 == 0) return;

            // perform calculations on mantissa and exponent
            uint mantissa = (0x007fffff & fu.ui32) | 0x00800000;
            uint exponent = ((0x7f800000 & fu.ui32) >> 23) - 126;
            while ((exponent & 0x3) != 0)
            {
                ++exponent;
                mantissa >>= 1;
            }//while
            fu.ui32 = (0x80000000 & fu.ui32) | (((exponent >> 2) + 64) << 24) | mantissa;

            // write to the destination, maintaining the "big-endian" order
            Buffer[offset] = fu.b3;
            Buffer[offset + 1] = fu.b2;
            Buffer[offset + 2] = fu.b1;
            Buffer[offset + 3] = fu.b0;
        }
        #endregion

        #region Strings
        /// <summary>
        /// Gets the string from the buffer
        /// </summary>
        /// <param name="length"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static string GetBytesString(byte[] Buffer, NumberUnion fu, int length, int offset)
        {
            if (Buffer == null) return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                byte b = Buffer[offset + i];
                if (b == 0) break;
                sb.Append(Convert.ToChar(b));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Sets the bytes in the buffer to the string values
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <param name="offset"></param>
        public static void SetBytesString(byte[] Buffer, NumberUnion fu, string value, int length, int offset)
        {
            if (Buffer == null) return;
            for (int i = 0; i < length; i++)
            {
                Buffer[offset + i] = (byte)0x00;
            }
            for (int i = 0; i < value.Length; i++)
            {
                Buffer[offset + i] = Convert.ToByte(value[i]);
            }
        }

        /// <summary>
        /// Gets the string from the buffer
        /// </summary>
        /// <param name="length"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static string GetEBCDICBytesString(byte[] Buffer, NumberUnion fu, int length, int offset)
        {
            if (Buffer == null) return "";
            Decoder dec = Encoding.GetEncoding(37).GetDecoder();
            char[] buff = new char[length];
            dec.GetChars(Buffer, offset, length, buff, 0, true);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < buff.Length; i++) sb.Append(buff[i]);
            return sb.ToString();
        }

        /// <summary>
        /// Sets the bytes in the buffer to the string values
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <param name="offset"></param>
        public static void SetEBCDICBytesString(byte[] Buffer, NumberUnion fu, string value, int length, int offset)
        {
            if (Buffer == null) return;
            char[] buff = new char[length];
            for (int i = 0; i < length; i++)
            {
                buff[i] = ' ';
            }
            for (int i = 0; i < value.Length && i <length; i++)
            {
                buff[i] = value[i];
            }
            Encoder enc = Encoding.GetEncoding(37).GetEncoder();
            enc.GetBytes(buff, 0, length, Buffer, offset, true);
        }
        #endregion

        #region Other Sets
        /// <summary>
        /// Sets one byte
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        public static void SetByte(byte[] Buffer, NumberUnion fu, byte value, int offset)
        {
            if (Buffer == null) return;
            Buffer[offset] = value;
        }
        #endregion
    }
}
