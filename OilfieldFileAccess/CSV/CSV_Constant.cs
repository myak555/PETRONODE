using System;
using System.Collections.Generic;
using System.Text;
using Petronode.OilfieldFileAccess;

namespace Petronode.OilfieldFileAccess.CSV
{
    /// <summary>
    /// Describes the CSV constant type
    /// The constant is presented as:
    /// #NAME,VALUE,UNIT,"Description"
    /// </summary>
    public class CSV_Constant: Oilfield_Constant
    {
        /// <summary>
        /// Constructor - creates a default variable
        /// </summary>
        public CSV_Constant(): base()
        {
        }

        /// <summary>
        /// Constructor - creates from the parameters
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="unit">Variable unit</param>
        /// <param name="value">Variable value</param>
        /// <param name="description">Description</param>
        public CSV_Constant(string name, string unit, string value, string description):
            base(name, unit, value, description)
        {
        }

        /// <summary>
        /// Creates the constant from the string input
        /// </summary>
        /// <param name="input">string as in CSV file</param>
        public CSV_Constant(string input)
        {
            if( !input.StartsWith( "#")) return;
            input = input.Substring(1);
            CSV_Line line = new CSV_Line(input);
            string name = line.GetString(0).Trim();
            if (name.Length <= 0) return;
            Name = name;
            Value = line.GetString(1);
            Unit = line.GetString(2);
            Description = line.GetString(3);
        }

        /// <summary>
        /// Converts back to CSV comment string
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
        {
            CSV_Line line = new CSV_Line( 4);
            line.SetString(0, "#" + Name); 
            line.SetString(1, Value); 
            line.SetString(2, Unit); 
            line.SetString(3, Description);
            return line.ToString();
        }

        /// <summary>
        /// Creates a deep copy of this constant
        /// </summary>
        /// <returns></returns>
        public override Oilfield_Constant Clone()
        {
            CSV_Constant tmp = new CSV_Constant();
            tmp.Name = this.Name;
            tmp.Unit = this.Unit;
            tmp.Value = this.Value;
            tmp.Description = this.Description;
            return tmp;
        }

    }
}
