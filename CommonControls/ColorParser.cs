using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Petronode.CommonControls
{
    public class ColorParser
    {
        /// <summary>
        /// Converts to colour using RGB only (A presumed 255)
        /// </summary>
        /// <param name="c">ARGB string</param>
        /// <returns>Colour</returns>
        public static Color GetColor(string c)
        {
            int r = ConvertPair(c, 2);
            int g = ConvertPair(c, 1);
            int b = ConvertPair(c, 0);
            return Color.FromArgb(255, r, g, b);
        }

        /// <summary>
        /// Converts to colour using ARGB
        /// </summary>
        /// <param name="c">ARGB string</param>
        /// <returns>Colour</returns>
        public static Color GetColorARGB(string c)
        {
            int a = ConvertPair(c, 3);
            int r = ConvertPair(c, 2);
            int g = ConvertPair(c, 1);
            int b = ConvertPair(c, 0);
            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Converts to 3-byte colour using RGB
        /// </summary>
        public static byte[] GetColorBytes(Color c)
        {
            byte[] tmp = new byte[3];
            tmp[0] = c.R;
            tmp[1] = c.G;
            tmp[2] = c.B;
            return tmp;
        }

        /// <summary>
        /// Converts to 6-letter colour using RGB
        /// </summary>
        public static string GetColorString(Color c)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append( ConvertTwoLetters( c.R));
            sb.Append( ConvertTwoLetters( c.G));
            sb.Append( ConvertTwoLetters( c.B));
            return sb.ToString();
        }

        /// <summary>
        /// Converts to 3-byte colour using RGB
        /// </summary>
        public static Color GetBytesColor(byte[] t)
        {
            Color c = Color.FromArgb(255, (int)t[0], (int)t[1], (int)t[2]);
            return c;
        }

        /// <summary>
        /// Converts to 3-byte colour using RGB
        /// </summary>
        public static Color GetBytesColor(byte[] t, int offset)
        {
            Color c = Color.FromArgb(255, (int)t[offset + 0], (int)t[offset + 1], (int)t[offset + 2]);
            return c;
        }

        /// <summary>
        /// Sets the transparency based on the transparent color and tolerance
        /// </summary>
        public static Color GetTransparencyColor(Color pixel, Color transparent, int tolerance)
        {
            int R = (int)pixel.R;
            int G = (int)pixel.G;
            int B = (int)pixel.B;
            int dR = R - (int)transparent.R;
            int dG = G - (int)transparent.G;
            int dB = B - (int)transparent.B;
            if (dR < -tolerance || dR > tolerance) return Color.FromArgb(255, R, G, B);
            if (dG < -tolerance || dG > tolerance) return Color.FromArgb(255, R, G, B);
            if (dB < -tolerance || dB > tolerance) return Color.FromArgb(255, R, G, B);
            return Color.FromArgb(0, R, G, B);
        }

        /// <summary>
        /// returns true if pixel is in tolerance with pattern
        /// </summary>
        public static bool InTolerance(Color pixel, Color pattern, int tolerance)
        {
            int R = (int)pixel.R;
            int G = (int)pixel.G;
            int B = (int)pixel.B;
            int dR = R - (int)pattern.R;
            int dG = G - (int)pattern.G;
            int dB = B - (int)pattern.B;
            if (dR < -tolerance || dR > tolerance) return false;
            if (dG < -tolerance || dG > tolerance) return false;
            if (dB < -tolerance || dB > tolerance) return false;
            return true;
        }

        /// <summary>
        /// returns true if pixel1 is in tolerance with pattern and pixel2 is not
        /// </summary>
        public static bool IsEdge(Color pixel1, Color pixel2, Color pattern, int tolerance)
        {
            if (!InTolerance(pixel1, pattern, tolerance)) return false;
            if ( InTolerance(pixel2, pattern, tolerance)) return false;
            return true;
        }

        /// <summary>
        /// returns tolerance between two pixels
        /// </summary>
        public static int GetTolerance(Color pixel1, Color pixel2)
        {
            int dC = (int)pixel1.R - (int)pixel2.R;
            if (dC < 0) dC = -dC;
            int dG = (int)pixel1.G - (int)pixel2.G;
            if (dG < 0) dG = -dG;
            if (dG < dC) dC = dG;
            int dB = (int)pixel1.B - (int)pixel2.B;
            if (dB < 0) dB = -dB;
            if (dB < dC) dC = dB;
            if (dC < 0) return 0;
            return dC;
        }

        /// <summary>
        /// returns tolerance between two pixels
        /// </summary>
        public static int GetTolerance(string pixel1, string pixel2)
        {
            return GetTolerance(GetColor(pixel1), GetColor(pixel2));
        }

        /// <summary>
        /// returns quadratic difference between two pixels
        /// </summary>
        public static long GetColorDiff2(Color pixel1, Color pixel2)
        {
            long dC = (long)pixel1.R - (long)pixel2.R;
            long tmp = dC * dC;
            dC = (long)pixel1.G - (long)pixel2.G;
            tmp += dC * dC;
            dC = (long)pixel1.B - (long)pixel2.B;
            tmp += dC * dC;
            return tmp;
        }

        /// <summary>
        /// Quick unchecked conversion of color to string
        /// </summary>
        public static string ColorToString(Color c)
        {
            char[] c_Hex = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            StringBuilder sb = new StringBuilder();
            sb.Append(c_Hex[c.R >> 4]);
            sb.Append(c_Hex[c.R & 15]);
            sb.Append(c_Hex[c.G >> 4]);
            sb.Append(c_Hex[c.G & 15]);
            sb.Append(c_Hex[c.B >> 4]);
            sb.Append(c_Hex[c.B & 15]);
            return sb.ToString();
        }

        /// <summary>
        /// Converts Color to a 3-digit string
        /// </summary>
        public static string ColorTo_000_String(Color c)
        {
            StringBuilder sb = new StringBuilder();
            int r = (10 * Convert.ToInt32(c.R)) >> 8;
            sb.Append(r.ToString("0"));
            int g = (10 * Convert.ToInt32(c.G)) >> 8;
            sb.Append(g.ToString("0"));
            int b = (10 * Convert.ToInt32(c.B)) >> 8;
            sb.Append(b.ToString("0"));
            return sb.ToString();
        }

        /// <summary>
        /// Converts Color to a 6-digit string
        /// </summary>
        public static string ColorTo_000000_String(Color c)
        {
            StringBuilder sb = new StringBuilder();
            int r = (100 * Convert.ToInt32(c.R)) >> 8;
            string s = r.ToString("0");
            sb.Append( (s.Length==2)? s: "0" + s);
            int g = (100 * Convert.ToInt32(c.G)) >> 8;
            s = g.ToString("0");
            sb.Append( (s.Length==2)? s: "0" + s);
            int b = (100 * Convert.ToInt32(c.B)) >> 8;
            s = b.ToString("0");
            sb.Append( (s.Length==2)? s: "0" + s);
            return sb.ToString();
        }

        private static int ConvertPair(string s, int position)
        {
            int i0 = s.Length - 1;
            int i1 = s.Length - 2;
            while (position > 0)
            {
                i0 -= 2;
                i1 -= 2;
                position--;
            }
            return ConvertOne(s, i0) + (ConvertOne(s, i1) << 4);
        }

        private static int ConvertOne(string s, int position)
        {
            if (position < 0) return 0;
            switch (s[position])
            {
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'a': return 10;
                case 'A': return 10;
                case 'b': return 11;
                case 'B': return 11;
                case 'c': return 12;
                case 'C': return 11;
                case 'd': return 13;
                case 'D': return 13;
                case 'e': return 14;
                case 'E': return 14;
                case 'f': return 15;
                case 'F': return 15;
                default: return 0;
            }
        }

        private static string ConvertTwoLetters(int c)
        {
            int c1 = c >> 4;
            int c0 = c % 16;
            return ConvertOneLetter(c1) + ConvertOneLetter(c0);
        }
        
        private static string ConvertOneLetter(int c)
        {
            string[] t = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
            if (c < 0 || c > 15) return "0";
            return t[c];
        }
    }
}
