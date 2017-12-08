using System;
using System.Collections.Generic;
using System.Text;
using Petronode.OilfieldFileAccess;

namespace Petronode.OilfieldFileAccess.LAS
{

    /// <summary>
    /// Descrives the LAS 2.0 curve type
    /// </summary>
    public class LAS_Channel:Oilfield_Channel
    {
        public string API_Code = "";
        char[] _dot_split = { '.' };
        char[] _space_split = { ' ' };

        /// <summary>
        /// Constructor - creates a default variable
        /// </summary>
        public LAS_Channel(): base()
        {
        }

        /// <summary>
        /// Constructor - creates from the parameters
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="unit">Variable unit</param>
        /// <param name="description">Description</param>
        public LAS_Channel(string name, string unit, string description): base( name, unit, description)
        {
        }

        /// <summary>
        /// Creates the variable from the LAS input string
        /// </summary>
        /// <param name="input">string as in LAS file</param>
        public LAS_Channel(string input)
        {
            // first, split by decimal to get the name
            string[] ss = input.Split(_dot_split, StringSplitOptions.RemoveEmptyEntries);
            if (ss.Length < 2) return;
            string name = ss[0].Trim();
            if (name.Length <= 0) return;
            Name = name;

            // the second substring must start with a unit or space
            if (!ss[1].StartsWith(" "))
            {
                string[] ss2 = ss[1].Split(_space_split);
                Unit = ss2[0];
            }

            // Now the rest of the string may contain API code and description
            string find = ss[0] + "." + Unit;
            int pos = input.IndexOf(find);
            if (pos < 0) return;
            input = input.Substring(pos + find.Length).Trim();

            // Separate description and API by locating the colon
            pos = input.LastIndexOf(':');
            if (pos >= 0)
            {
                API_Code = input.Substring(0, pos).TrimEnd();
                Description = input.Substring(pos + 1).Trim();
            }
        }

        /// <summary>
        /// Creates a deep copy of a given channel
        /// </summary>
        /// <param name="c"></param>
        public LAS_Channel(Oilfield_Channel c): base( (Oilfield_Channel)c)
        {
        }

        /// <summary>
        /// Converts back to LAS file string
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(" ");

            // name is padded to 5 symbols, then dot and unit added and padded to 14 symbols
            string s1 = Name.PadRight(5) + "." + Unit;
            sb.Append(s1);

            // then, API code is added at position 20
            s1 = sb.ToString().PadRight(20) + API_Code;

            // then, if the description is present,
            // the string is padded to 34 symbols, and the description is added
            if (Description.Length > 0) return s1.PadRight( 34) + ":" + Description;
            else return sb.ToString();
        }

        /// <summary>
        /// Locates the upper and lower boundaries for the channel
        /// </summary>
        /// <param name="index">index channel or null</param>
        /// <returns>true is boundearies are located</returns>
        public bool LocateDataBoundaries( LAS_Channel index)
        {
            return base.LocateDataBoundaries((Oilfield_Channel)index);
        }

        /// <summary>
        /// Returns the channel average from start to end depth inclusive.
        /// </summary>
        /// <param name="start">start index</param>
        /// <param name="end">end index</param>
        /// <param name="index">index channel</param>
        /// <returns>ariphmetic average or NaN</returns>
        public double GetAverage(double start, double end, LAS_Channel index)
        {
            return base.GetAverage(start, end, (Oilfield_Channel)index);
        }
    }
}
