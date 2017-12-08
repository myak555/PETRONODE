using System;
using System.Collections.Generic;
using System.Text;

namespace Petronode.OilfieldFileAccess.LAS
{
    /// <summary>
    /// Descrives the LAS 2.0 information line
    /// </summary>
    public class LAS_InfoLine
    {
        public string Line = "";

        /// <summary>
        /// Constructor - creates a default variable
        /// </summary>
        public LAS_InfoLine()
        {
        }

        /// <summary>
        /// Creates the variable from the input
        /// </summary>
        /// <param name="input">string as in LAS file</param>
        public LAS_InfoLine(string input)
        {
            Line = input;
        }

        /// <summary>
        /// Converts back to LAS file string
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
        {
            return Line;
        }

        /// <summary>
        /// Attempts information retrieval from info line
        /// </summary>
        /// <param name="key">information key</param>
        /// <returns>info string if found, or empty string</returns>
        public string GetInfo(string key)
        {
            int pos = Line.IndexOf(key);
            if (pos < 0) return "";
            string infoSubstring = Line.Substring(pos + key.Length, 12).TrimStart();
            if (infoSubstring.Length <= 0) return "";
            pos = infoSubstring.IndexOf("   ");
            if (pos < 0) return infoSubstring.Trim();
            return infoSubstring.Substring(0, pos).Trim();
        }
    }
}
