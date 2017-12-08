using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.VendorFileAccess.CSV
{
    /// <summary>
    /// This class deals with the correct CSV line processing
    /// </summary>
    public class CSV_Line
    {
        char[] c_Separatiors = { ',' };
        char[] c_TimeSeparatiors = { ':' };

        /// <summary>
        /// Valid substrings within the line
        /// </summary>
        public string[] Substrings = null;

        /// <summary>
        /// Constructor, creates substrings from a line given
        /// </summary>
        /// <param name="s"></param>
        public CSV_Line(string s)
        {
            StringBuilder sb = new StringBuilder();
            bool inquote = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (!inquote && c == '"')
                {
                    inquote = true;
                    continue;
                }
                if (inquote && c == ',')
                {
                    sb.Append("&comma");
                    continue;
                }
                if (inquote && c == '"')
                {
                    inquote = false;
                    continue;
                }
                sb.Append(c);
            }
            Substrings = sb.ToString().Split(c_Separatiors);
            for (int i = 0; i < Substrings.Length; i++) 
                Substrings[i] = Substrings[i].Replace("&comma", ",").Replace("&quot", "\"");
        }

        /// <summary>
        /// Constructor, creates a class with empty substrings
        /// </summary>
        /// <param name="n"></param>
        public CSV_Line(int n)
        {
            Substrings = new string[n];
            for (int i = 0; i < Substrings.Length; i++)
                Substrings[i] = "";
        }

        /// <summary>
        /// Returns a string
        /// </summary>
        /// <param name="i">address</param>
        /// <returns></returns>
        public string GetString( int i)
        {
            if (i < 0 || i >= Substrings.Length) return "";
            return Substrings[i];
        }

        /// <summary>
        /// Returns a double
        /// </summary>
        /// <param name="i">address</param>
        /// <returns></returns>
        public double GetDouble(int i)
        {
            if (i < 0 || i >= Substrings.Length) return Double.NaN;
            try
            {
                string s = Substrings[i];
                if (s.Contains(":"))
                {
                    string[] ss = s.Split(c_TimeSeparatiors);
                    switch (ss.Length)
                    {
                        case 1: return Convert.ToDouble(ss[0]);
                        case 2: return Convert.ToDouble(ss[0]) * 60.0 + Convert.ToDouble(ss[1]);
                        case 3: return Convert.ToDouble(ss[0]) * 3600.0 + Convert.ToDouble(ss[1]) * 60.0 + Convert.ToDouble(ss[2]);
                        default: return Double.NaN;
                    }
                }
                double d = Convert.ToDouble(Substrings[i]);
                return d;
            }
            catch (Exception) { }
            return Double.NaN;
        }


        /// <summary>
        /// Returns a boolean
        /// </summary>
        /// <param name="i">address</param>
        /// <returns></returns>
        public bool GetBoolean(int i)
        {
            if (i < 0 || i >= Substrings.Length) return false;
            return Substrings[i].ToUpper().StartsWith("TRU");
        }

        /// <summary>
        /// Sets a substring
        /// </summary>
        /// <param name="i">address</param>
        public void SetString(int i, string s)
        {
            if (i < 0 || i >= Substrings.Length) return;
            Substrings[i] = s;
        }

        /// <summary>
        /// Sets a boolean
        /// </summary>
        /// <param name="i">address</param>
        /// <returns></returns>
        public void SetBoolean(int i, bool b)
        {
            if (i < 0 || i >= Substrings.Length) return;
            Substrings[i] = b? "TRUE": "FALSE";
        }

        /// <summary>
        /// Converts the list to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Substrings.Length; i++)
            {
                string s = Substrings[i].Replace("\"", "&quot");
                if( s.Contains( ",")) sb.Append( "\"" + s + "\",");
                else sb.Append(s);
                if( i<Substrings.Length-1) sb.Append(",");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts boolean to string
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string GetBoolString(bool b)
        {
            return b ? "TRUE" : "FALSE";
        }
    }
}
