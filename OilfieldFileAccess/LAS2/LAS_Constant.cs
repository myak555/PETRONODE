using System;
using System.Collections.Generic;
using System.Text;
using Petronode.OilfieldFileAccess;

namespace Petronode.OilfieldFileAccess.LAS
{
    /// <summary>
    /// Describes the LAS 2.0 constant type
    /// </summary>
    public class LAS_Constant: Oilfield_Constant
    {
        char[] _dot_split = { '.' };
        char[] _space_split = { ' ' };

        /// <summary>
        /// Constructor - creates a default variable
        /// </summary>
        public LAS_Constant(): base()
        {
        }

        /// <summary>
        /// Constructor - creates from the parameters
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="unit">Variable unit</param>
        /// <param name="value">Variable value</param>
        /// <param name="description">Description</param>
        public LAS_Constant(string name, string unit, string value, string description):
            base(name, unit, value, description)
        {
        }

        /// <summary>
        /// Creates the variable from the input
        /// </summary>
        /// <param name="input">string as in LAS file</param>
        public LAS_Constant(string input)
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

            // Now the rest of the string contains the value and possibly description
            string find = ss[0] + "." + Unit;
            int pos = input.IndexOf(find);
            if (pos < 0) return;
            input = input.Substring( pos+find.Length).Trim();

            // Separate description by locating the colon
            pos = input.LastIndexOf(':');
            if (pos < 0)
            {
                Value = input.Trim();
                return;
            }
            Value = input.Substring(0, pos).Trim();
            Description = input.Substring(pos + 1).Trim();
        }

        /// <summary>
        /// Converts back to LAS file string
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(" ");

            // name is padded to 4 symbols, then dot and unit added and padded to 14 symbols
            string s1 = Name.PadRight( 4) + "." + Unit;
            sb.Append( s1.PadRight( 14));

            // value. If the string contains dot, it is padded from the left (number)
            // date is also padded from the left; all others are padded from the right
            s1 = ( Value.Contains( ".") || Name.StartsWith( "DATE"))?
                Value.PadLeft( 11) : Value.PadRight( 11);
            sb.Append( s1);

            // then, if the description is present,
            // the string is padded to 46 symbols, and the description is added
            if (Description.Length > 0) return sb.ToString().PadRight(46) + ":" + Description;
            else return sb.ToString();
        }
    }
}
