using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Petronode.CommonControls
{
    public class ColorConverter
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
