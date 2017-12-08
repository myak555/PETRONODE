using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Petronode.OilfieldFileAccess;
using Petronode.OilfieldFileAccess.Converters;

namespace Petronode.OilfieldFileAccess.SEGY
{
    /// <summary>
    /// Describes the SEGY constant type
    /// The constant is presented as:
    /// #NAME,VALUE,UNIT,"Description"
    /// </summary>
    public class SEGY_Constant: Oilfield_Constant
    {
        public int Type = 0; // Undefined
        public int Location = 0; // Undefined

        /// <summary>
        /// Constructor - creates a default variable
        /// </summary>
        public SEGY_Constant(): base()
        {
        }

        /// <summary>
        /// Constructor - creates from the parameters
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="unit">Variable unit</param>
        /// <param name="value">Variable value</param>
        /// <param name="description">Description</param>
        public SEGY_Constant(string name, string unit, string value, string description):
            base(name, unit, value, description)
        {
        }

        /// <summary>
        /// Creates the constant from the string input
        /// </summary>
        /// <param name="input">string as in SEG-Y EBCDIC Header file</param>
        public SEGY_Constant(int line, byte[] input)
        {
            int offset = line * 80;
            NumberUnion nu = new NumberUnion();
            string s = BufferConverter.GetEBCDICBytesString(input, nu, 3, offset);
            if (s.Length < 3)
            {
                line++;
                s = "C" + line.ToString().PadLeft(2);
            }
            Name = s;
            Value = BufferConverter.GetEBCDICBytesString(input, nu, 77, offset);
            Unit = "";
            Description = "Comment line " + s + ".";
        }

        /// <summary>
        /// Creates the constant from the string input
        /// </summary>
        /// <param name="input">string as in SEG-Y EBCDIC Header file</param>
        public SEGY_Constant(string name, string unit, int location, int type, byte[] input, string description)
        {
            this.Name = name;
            this.Unit = unit;
            this.Location = location;
            this.Type = type;
            this.Description = description;
            NumberUnion nu = new NumberUnion();
            switch (type)
            {
                case SEGY_Constant.Int16:
                    this.Value = BufferConverter.GetBytesInt16_BE(input, nu, location).ToString();
                    break;
                case SEGY_Constant.UInt16:
                    this.Value = BufferConverter.GetBytesUInt16_BE(input, nu, location).ToString();
                    break;
                case SEGY_Constant.Int32:
                    this.Value = BufferConverter.GetBytesInt32_BE(input, nu, location).ToString();
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Converts back to SEG-Y comment string
        /// </summary>
        /// <returns>Converted string</returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Converts the Constant to strings represenetation
        /// </summary>
        /// <returns></returns>
        public override string[] ToStringsSimple()
        {
            string[] tmp = new string[1];
            tmp[0] = Value;
            return tmp;
        }

        /// <summary>
        /// Creates a deep copy of this constant
        /// </summary>
        /// <returns></returns>
        public override Oilfield_Constant Clone()
        {
            SEGY_Constant tmp = new SEGY_Constant();
            tmp.Name = this.Name;
            tmp.Unit = this.Unit;
            tmp.Value = this.Value;
            tmp.Description = this.Description;
            return tmp;
        }

        /// <summary>
        /// Undefined
        /// </summary>
        public const int Undefined = 0;
        
        /// <summary>
        /// Short Int
        /// </summary>
        public const int Int16 = 1;

        /// <summary>
        /// Short UInt
        /// </summary>
        public const int UInt16 = 2;

        /// <summary>
        /// Int
        /// </summary>
        public const int Int32 = 3;
    }
}
