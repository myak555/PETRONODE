using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Petronode.SlideComposerLibrary
{
    public class Slide
    {
        public string Prefix = "Slide";
        public int x = 6000;
        public int y = 4000;
        public string Format = "png";
        public string bColor = "FFFFFF";
        public double pageX = 21.59; // cm
        public double pageY = 27.94; // cm
        public double pageDPI = 250.0;

        public List<SlideComponent> Components = new List<SlideComponent>();
        public Dictionary<string, string> DefaultParameters = new Dictionary<string,string>();

        /// <summary>
        /// Constructor. Creates slide definition
        /// </summary>
        /// <param name="command"></param>
        public Slide(string[] command)
        {
            Prefix = command[1];
            x = Convert.ToInt32(command[2]);
            y = Convert.ToInt32(command[3]);
            if (command.Length > 4) Format = command[4];
            if (command.Length > 5) bColor = command[5];
            if (command.Length > 6) pageX = Convert.ToDouble(command[6]);
            if (command.Length > 7) pageY = Convert.ToDouble(command[7]);
            if (command.Length > 8) pageDPI = Convert.ToDouble(command[8]);
            DefaultParameters.Add("x", "0");
            DefaultParameters.Add("y", "0");
            DefaultParameters.Add("dx", "0");
            DefaultParameters.Add("dy", "0");
            DefaultParameters.Add("source_x", "0");
            DefaultParameters.Add("source_y", "0");
            DefaultParameters.Add("source_dx", "-1");
            DefaultParameters.Add("source_dy", "-1");
            DefaultParameters.Add("mask", "*.*");
            DefaultParameters.Add("text", "");
            DefaultParameters.Add("fcolor", "000000");
            DefaultParameters.Add("bcolor", "FFFFFF");
            DefaultParameters.Add("ftype", "System Sans Serif");
            DefaultParameters.Add("fsize", "60.0");
            DefaultParameters.Add("fstyle", "normal");
            DefaultParameters.Add("lsize", "1.0");
            DefaultParameters.Add("calloutx", "-1");
            DefaultParameters.Add("callouty", "-1");
            DefaultParameters.Add("calloutellipse", "1");
            DefaultParameters.Add("topcolor", "FFFFFF");
            DefaultParameters.Add("bottomcolor", "FFFFFF");
            DefaultParameters.Add("tcolor", "FFFFFF");
            DefaultParameters.Add("ttolerance", "0");
            DefaultParameters.Add("orientation", "0");
            DefaultParameters.Add("tposition", "left");
            DefaultParameters.Add("ccolor", "FFFFFF");
            DefaultParameters.Add("ctolerance", "0");
            DefaultParameters.Add("newfile", "false");
            DefaultParameters.Add("ltype", "solid");
            DefaultParameters.Add("dtype", "dot");
            DefaultParameters.Add("namex", "1");
            DefaultParameters.Add("namey", "0");
            DefaultParameters.Add("scalex0", "0");
            DefaultParameters.Add("scaley0", "0");
            DefaultParameters.Add("scalex1", "1");
            DefaultParameters.Add("scaley1", "1");
        }

        /// <summary>
        /// Adds an attribute to the Slide
        /// </summary>
        /// <param name="attribute">Attribute string in form Name=Something</param>
        public virtual string[] AddAttribute( string attribute)
        {
            string[] tmp = null;
            int i = attribute.IndexOf('=');
            if( i<=0)
            {
                tmp = new string[1];
                tmp[0] = attribute;
                return tmp;
            }
            tmp = new string[2];
            tmp[0] = attribute.Substring(0, i).Trim().ToLower();
            tmp[1] = attribute.Substring(i + 1);
            try
            {
                switch (tmp[0])
                {
                    case "name": Prefix = tmp[1].Trim(); break;
                    case "prefix": Prefix = tmp[1].Trim(); break;
                    case "x": x = Convert.ToInt32(tmp[1].Trim()); break;
                    case "y": y = Convert.ToInt32(tmp[1].Trim()); break;
                    case "format": Format = tmp[1].Replace( " ", "").ToLower(); break;
                    case "bcolor": bColor = tmp[1].Trim().ToUpper(); break;
                    case "backcolor": bColor = tmp[1].Trim().ToUpper(); break;
                    case "pagex": pageX = Convert.ToDouble(tmp[1].Trim()); break;
                    case "pagey": pageY = Convert.ToDouble(tmp[1].Trim()); break;
                    case "pagedpi": pageDPI = Convert.ToDouble(tmp[1].Trim()); break;
                    default: break;
                }
            }
            catch (Exception) { }
            return tmp;
        }

        /// <summary>
        /// Retrieves the background colour
        /// </summary>
        public Color BkColor
        {
            get
            {
                return Slide.GetColor(bColor);
            }
        }

        /// <summary>
        /// Returns true if the slide is small enough to be acommodated on memory
        /// </summary>
        public bool OnMemory
        {
            get
            {
                if (x > 64000) return false;
                if (y > 64000) return false;
                //if (x > 999) return false;
                //if (y > 999) return false;
                return true;
            }
        }

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
            return Color.FromArgb( 255, r, g, b);
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
        /// Converts to 3-byte colour using RGB
        /// </summary>
        public static Color GetBytesColor(byte[] t)
        {
            Color c = Color.FromArgb( 255, (int)t[0], (int)t[1], (int)t[2]);
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
        /// Converts color to RGB
        /// </summary>
        /// <param name="c">Color to convert</param>
        /// <returns>string as RRGGBB</returns>
        public static string GetColorString(Color c)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ConvertPairS((int)c.R));
            sb.Append(ConvertPairS((int)c.G));
            sb.Append(ConvertPairS((int)c.B));
            return sb.ToString();
        }

        /// <summary>
        /// Converts color to ARGB
        /// </summary>
        /// <param name="c">Color to convert</param>
        /// <returns>string as AARRGGBB</returns>
        public static string GetColorStringARGB(Color c)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ConvertPairS((int)c.A));
            sb.Append(ConvertPairS((int)c.R));
            sb.Append(ConvertPairS((int)c.G));
            sb.Append(ConvertPairS((int)c.B));
            return sb.ToString();
        }

        private static int ConvertPair(string s, int position)
        {
            int i0 = s.Length - 1;
            int i1 = s.Length - 2;
            while (position > 0)
            {
                i0-=2;
                i1-=2;
                position--;
            }
            return ConvertOne(s, i0) + ConvertOne(s, i1)*16;
        }

        private static string ConvertPairS( int n)
        {
            int i0 = n >> 4;
            int i1 = n - (i0 << 4);
            return ConvertOneS( i0) + ConvertOneS( i1);
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

        private static string ConvertOneS(int n)
        {
            string sret = "0123456789ABCDEF";
            if (n < 0) return "0";
            if (n >= 16) return "F";
            return sret.Substring(n, 1);
        }
    }
}
