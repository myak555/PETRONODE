using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Petronode.OilfieldFileAccess;
using Petronode.OilfieldFileAccess.Converters;

namespace Petronode.OilfieldFileAccess.LDF
{
    /// <summary>
    /// Describes the LDF constant type
    /// The constant is presented as:
    /// #NAME,VALUE,UNIT,"Description"
    /// </summary>
    public class LDF_Constant: Oilfield_Constant
    {
        public int Type = 0; // Undefined
        public int Location = 0; // Undefined
        public int Length = 0; // Undefined
        public float ConversionFactor = 1.0f; // set to 0.3048f for the lengths in trace headers

        /// <summary>
        /// Constructor - creates a default variable
        /// </summary>
        public LDF_Constant(): base()
        {
        }

        /// <summary>
        /// Constructor - creates from the parameters
        /// </summary>
        /// <param name="name">Variable name</param>
        /// <param name="unit">Variable unit</param>
        /// <param name="value">Variable value</param>
        /// <param name="description">Description</param>
        public LDF_Constant(string name, string unit, string value, string description):
            base(name, unit, value, description)
        {
        }

        /// <summary>
        /// Creates the fixed constant
        /// </summary>
        /// <param name="name">Name of the constant</param>
        /// <param name="unit">Unit</param>
        /// <param name="location">Location in the buffer</param>
        /// <param name="length">Length (for strings only)</param>
        /// <param name="type">LDF Constant type</param>
        /// <param name="value">default value</param>
        /// <param name="description">Description</param>
        public LDF_Constant(string name, string unit, int location, int length, int type, string value, string description)
        {
            this.Name = name;
            this.Unit = unit;
            this.Location = location;
            this.Length = length;
            this.Type = type;
            this.Value = value;
            this.Description = description;
        }

        /// <summary>
        /// Converts back to LDF comment string
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
            LDF_Constant tmp = new LDF_Constant();
            tmp.Name = this.Name;
            tmp.Unit = this.Unit;
            tmp.Value = this.Value;
            tmp.Description = this.Description;
            return tmp;
        }

        /// <summary>
        /// Sets value to the buffer
        /// </summary>
        /// <param name="Buffer"></param>
        public void GetBuffer(byte[] Buffer)
        {
            NumberUnion nu = new NumberUnion();
            switch (Type)
            {
                case LDF_Constant.Int16:
                    this.Value = BufferConverter.GetBytesInt16_BE(Buffer, nu, Location).ToString();
                    break;
                case LDF_Constant.UInt16:
                    this.Value = BufferConverter.GetBytesUInt16_BE(Buffer, nu, Location).ToString();
                    break;
                case LDF_Constant.Int32:
                    this.Value = BufferConverter.GetBytesInt32_BE(Buffer, nu, Location).ToString();
                    break;
                case LDF_Constant.Float32:
                    this.Value = BufferConverter.GetBytesFloat_BE(Buffer, nu, Location).ToString();
                    break;
                case LDF_Constant.String:
                    this.Value = BufferConverter.GetBytesString(Buffer, nu, Length, Location);
                    break;
                default: break;
            }
        }

        /// <summary>
        /// Sets value to the buffer
        /// </summary>
        /// <param name="Buffer"></param>
        public void SetBuffer(byte[] Buffer)
        {
            NumberUnion nu = new NumberUnion();
            switch (Type)
            {
                case LDF_Constant.Int16:
                    BufferConverter.SetBytesInt16_BE(Buffer, nu, Convert.ToInt16(Value), Location);
                    break;
                case LDF_Constant.UInt16:
                    BufferConverter.SetBytesUInt16_BE(Buffer, nu, Convert.ToUInt16(Value), Location);
                    break;
                case LDF_Constant.Int32:
                    BufferConverter.SetBytesInt32_BE(Buffer, nu, Convert.ToInt32(Value), Location);
                    break;
                case LDF_Constant.Float32:
                    BufferConverter.SetBytesFloat_BE(Buffer, nu, Convert.ToSingle(Value)/ConversionFactor, Location);
                    break;
                case LDF_Constant.String:
                    string v = (Value.Length > Length) ? Value.Substring(0, Length) : Value;
                    BufferConverter.SetBytesString(Buffer, nu, v, Length+1, Location);
                    break;
                default: break;
            }
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

        /// <summary>
        /// Short float
        /// </summary>
        public const int Float32 = 4;

        /// <summary>
        /// String
        /// </summary>
        public const int String = 5;
    }
}
